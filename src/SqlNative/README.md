# SqlNative

A light-weight ORM for .NET that doesn't hide the database. You write the SQL — tables, columns and expressions are C# identifiers, so a renamed column is a compile error instead of a 500 at 3am — and SqlNative maps rows to types, generates your data-access classes, and runs on any ADO.NET `DbConnection`, so SQL Server, MySQL, PostgreSQL and Oracle all work through the same code.

This is the core package. It contains the query builder, the table/column model, result mapping and CRUD. Everything else in the toolkit builds on it.

---

## Contents

- [What kind of library is this?](#what-kind-of-library-is-this)
- [Install](#install)
- [Quick start](#quick-start)
- [Core concepts](#core-concepts)
- [Building and executing queries](#building-and-executing-queries)
- [CRUD](#crud)
- [How parameters are handled](#how-parameters-are-handled)
- [Mapping results](#mapping-results)
- [Generating a row class](#generating-a-row-class)
- [Typed repositories](#typed-repositories)
- [Configuration and lifecycle](#configuration-and-lifecycle)
- [Logging](#logging)
- [Supported databases](#supported-databases)
- [Related packages](#related-packages)
- [Current status](#current-status)

---

## What kind of library is this?

SqlNative is an ORM in the sense that it maps your tables to C# types and gives you CRUD — and not much more than that:

| | |
| --- | --- |
| Tables and columns as typed classes | Yes — hand-write them once, or generate them |
| Row → POCO mapping | Yes — reflection, or explicit ordinals for full control |
| Query builder (`BuildQuery`) | Yes — returns a `Query` you can inspect before it runs |
| Per-table CRUD (`ctx.Emp.Insert(row)`) | Yes — via `TableRepository<Row, Table>` |
| Code generation from a live schema | Yes — `SqlNative.ModelGenerator` |
| Change tracking | No — you call `Insert` or `Update` yourself |
| Lazy loading / proxies | No |
| Migrations | No — you write the DDL |
| Unit of work / identity map | No — every `Query` stands alone |
| LINQ provider | No — you write SQL |

The trade is deliberate. Nothing sits between you and the wire, so you keep every feature the database has: window functions, CTEs, `OUTPUT`, query hints, `MERGE`, vendor extensions. The cost is that SqlNative will not save you from writing SQL, and there is no query builder more elaborate than the one you can see in full on screen.

If you want an ORM that generates SQL for you and gets out of the way, use EF Core or Dapper. If you want an ORM that stays out of your SQL, use this.

---

## Install

```bash
dotnet add package SqlNative
```

Targets `net461`, `netstandard2.0`, `net6.0`, `net8.0`, `net9.0` and `net10.0`.

You also need an ADO.NET provider (`Microsoft.Data.SqlClient`, `MySqlConnector`, `Npgsql`, `Oracle.ManagedDataAccess.Client`) — SqlNative does not reference any of them.

---

## Quick start

**1. Describe a table once.**

```csharp
using SqlNative;

public partial class DboEmp : MsSqlTableStructure
{
    public override string TableName { get; } = "Emp";
    public override string QuotedTableName { get; } = "dbo.Emp";
    public override string Schema { get; } = "dbo";

    public TableColumn Id { get; }
    public TableColumn Firstname { get; }
    public TableColumn Lastname { get; }
    public TableColumn Deptid { get; }

    public DboEmp()
    {
        Id        = new MsSqlColumn("Id", false, "bigint", typeof(long), 0, this);
        Firstname = new MsSqlColumn("FirstName", false, "nvarchar", typeof(string), 0, this);
        Lastname  = new MsSqlColumn("LastName", false, "nvarchar", typeof(string), 0, this);
        Deptid    = new MsSqlColumn("DeptId", false, "bigint", typeof(long), 0, this);
    }
}
```

From now on `dbo.Emp` is a C# symbol. Misspell `Lastname` and the compiler stops you.

**2. Derive a context from your connection.**

```csharp
public class Db1Context : SqlNativeContext
{
    public Db1Context(DbConnection connection) => Connection = connection;
}
```

**3. Query it.**

```csharp
using var connection = new SqlConnection(connectionString);
using var ctx = new Db1Context(connection) { OwnsConnection = true };

var rows = ctx.BuildQuery<DboEmp>((q, e) => q
    .Append($"SELECT {e.Id}, {e.Firstname}, {e.Lastname}")
    .Append($"FROM {e}")
    .Append($"WHERE {e.Deptid} = @deptid")
    .SetParameters(new { deptid = 4 }))
    .Execute<EmpRow>();
```

---

## Core concepts

### Tables and columns

`TableStructure` is the base class for a table; `TableColumn` for a column. Each database has its own subclass that knows how to quote identifiers:

| Base class | Column class | `SqlType` | Identifier quoting |
| --- | --- | --- | --- |
| `MsSqlTableStructure` | `MsSqlColumn` | `sqlType` | `[Name]` |
| `MySqlTableStructure` | `MySqlColumn` | `sqlType` | `` `Name` `` |
| `PostgresTableStructure` | `PostgresColumn` | `sqlType` | `"Name"` |
| `OracleTableStructure` | `OracleColumn` | `sqlType` | `"Name"` |

Quoting is opt-in per column via the `needsBrackets` / `needsBackticks` / `needsDoubleQuotes` flag, so identifiers that don't need it stay unquoted. Every column also carries its SQL type and CLR type, which the model generator uses later.

`MsSqlTableStructure`, `PostgresTableStructure` and `OracleTableStructure` also expose an abstract `Schema` property.

### Aliases

`TableStructure.ToString()` renders as `[schema].[Table] as [alias]`. You never set the alias by hand: `BuildQuery` assigns a short one, derived from the table name and guaranteed unique across the query.

```csharp
ctx.BuildQuery<DboEmp, DboEmp>((q, e1, e2) => q
    .Append($"SELECT {e1.Id}, {e1.Lastname}, {e2.Lastname} AS Manager")
    .Append($"FROM {e1}")
    .Append($"LEFT JOIN {e2} ON {e1.Deptid} = {e2.Id}"))
// FROM [dbo].[Emp] as e LEFT JOIN [dbo].[Emp] as e1 ON e.DeptId = e1.Id
```

Set `DisableAlias = true` on a table structure to drop the alias from the generated SQL — useful inside `INSERT`, `UPDATE` and `DELETE`, where the table is named on its own.

### Expressions

`QueryPart` is an operator-overloaded string wrapper for building expressions:

```csharp
QueryPart age = 30;      // "30"
QueryPart active = true; // "TRUE"

var filter = $"{a.Lastname} == NULL";   // a.Lastname = NULL
var range  = $"{a.Id} >= {1}";          // a.Id >= 1
var neg    = !$"{a.Name}.Contains(\"x\")";  // NOT ...
```

`==` emits `IS NULL` when either operand is the literal `"null"` (lower case), and `=` otherwise. Note that `SqlFunctions.NULL` renders as upper-case `NULL`, so comparing against it produces `= NULL` rather than `IS NULL` — compare against the lower-case `"null"` string if you want the null check.

`+` concatenates two `QueryPart`s with a `+`, which is how you write arithmetic like `a.Price * a.Qty`. Note that `TableColumn + TableColumn` is a different overload: it joins with a **space**, so build comma-separated column lists with `SqlFunctions.CList(...)`.

```csharp
var cols = SqlFunctions.CList(c.Name, c.CountryCode, c.Population); // "c.Name, c.CountryCode, c.Population"
```

---

## Building and executing queries

`BuildQuery` returns a `Query` — a builder that has not run yet. You finish it and execute it yourself, which means you can inspect or extend the SQL first:

```csharp
var q = ctx.BuildQuery<DboEmp>((q, e) => q
    .Append($"SELECT {e.Star}")
    .Append($"FROM {e}"));

q.Append("ORDER BY LastName DESC");     // add your own clauses

Console.WriteLine(q.Sql);               // inspect before it hits the database
var rows = q.Execute<EmpRow>();
```

| Method | Purpose |
| --- | --- |
| `Append(string)` | Append a line of SQL. Blank lines are ignored. |
| `AppendIf(bool, string)` | Append only when the condition holds. |
| `AppendFor<T>(items, func)` | Append once per item. |
| `AppendFor<T>(join, items, func)` | Append once per item with a separator — useful for `IN` lists. |
| `SetParameters(object)` | Replace the parameters, from a dictionary or an anonymous object. |
| `AddParameters(object)` | Add parameters to the ones already set. |
| `Execute<T>()` | Run and map to `List<T>`. |
| `ExecuteScalar()` | Run and return the first column of the first row. |
| `ExecuteNonQuery()` | Run and return the affected row count. |
| `ExecuteAsync<T>(ct)` | Awaitable `Execute<T>()`. |
| `ExecuteScalarAsync(ct)` | Awaitable `ExecuteScalar()`. |
| `ExecuteNonQueryAsync(ct)` | Awaitable `ExecuteNonQuery()`. |
| `Model<T>(className)` | Write a row class for this result set. |
| `Config(Action)` | Change the table structure before the SQL is built. |
| `Sql` | The generated SQL, without executing it. |

`BuildQuery` has overloads for one table up to fifteen. The tables are created for you, each with a unique alias.

### Async

Every execution method has an `Async` counterpart that takes an optional `CancellationToken`:

```csharp
var rows = await ctx.BuildQuery<DboEmp>((q, e) => q
        .Append($"SELECT {e.Star}")
        .Append($"FROM {e}"))
    .ExecuteAsync<EmpRow>(ct);

var affected = await ctx.BuildQuery<DboEmp>((q, e) => q
        .Append($"DELETE FROM {e} WHERE {e.Id} = @id")
        .SetParameters(new { id = 1 }))
    .ExecuteNonQueryAsync(ct);
```

The async methods use the provider's real async I/O (`ExecuteReaderAsync`, `ReadAsync`,
`ExecuteScalarAsync`, `ExecuteNonQueryAsync`), so the calling thread is released while the
database works. The synchronous methods delegate to the async ones, so both paths run the
same code.

---

## CRUD

CRUD lives on `TableRepository<Row, Table>`, not on the context. You write a repository for a table — or generate one — and a context exposes it as a property:

```csharp
public class Db1Context : SqlNativeContext
{
    private DboEmpRepository emp;
    public DboEmpRepository Emp => emp ??= new(this);
}
```

### Select

```csharp
var rows = ctx.Emp.Select(e => $"{e.Deptid} = @deptid", new { deptid = 4 });

var all = ctx.Emp.All();
```

`Select` takes a predicate over the table structure and returns `List<Row>`. The predicate may use any column, and the parameters are bound, not interpolated.

### Insert

```csharp
var affected = ctx.Emp.Insert(row, OperationOptions.SkipPrimaryKey);
var affectedMany = ctx.Emp.Insert(rows);
```

The insert runs immediately and returns the affected count. `InsertAsync` is the awaitable counterpart.

`OperationOptions` controls the primary key and whether the row is read back:

| Option | Effect |
| --- | --- |
| `SkipPrimaryKey` | Omits the primary key from the INSERT. The default. |
| `LoadRecord` | Reads the inserted row back and copies it onto the object you passed in. |
| `LoadAndSkip` | Both — omits the key and reads the row back. |
| `None` | Inserts the primary key as-is and does not read the row back. |

### Update

```csharp
var affected = ctx.Emp.Update(row, OperationOptions.LoadRecord);
var affectedMany = ctx.Emp.Update(rows);
```

`LoadRecord` (or `LoadAndSkip`) re-reads the row afterwards, so database-side defaults and triggers land on your object. `None` — the default — just updates.

### Insert or update

```csharp
var affected = ctx.Emp.InsertOrUpdate(rows, OperationOptions.LoadRecord);
```

Rows whose primary key is not set — every key column null or the CLR default — are inserted; the rest are updated. Do the split with `IsPrimaryKeySet` by hand if you need finer control. Composite (multi-column) primary keys are supported: the key counts as set only when every key column is set. Tables without a primary key are not supported.

### Delete

```csharp
ctx.Emp.Delete(row);                                       // by key
ctx.Emp.Delete(rows);                                      // a list
ctx.Emp.Delete(e => $"{e.Deptid} = @deptid");           // by predicate
```

The delete helpers run immediately and return the affected count — they do not return a `Query`.

---

## How parameters are handled

`Query.CreateCommand` treats each parameter differently, and the distinction matters when you read a query plan or a log:

| Parameter value | What happens |
| --- | --- |
| `null` | Bound as a `DbParameter` with `DBNull.Value`. |
| Anything else | Bound as a real `DbParameter` on the command. |

Only strings, dates, GUIDs, arrays and `DBNull` actually reach the database as parameters, so a value is always escaped by the provider.

The `@` prefix is optional — `@deptid` and `deptid` both bind to `@deptid`.

---

## Mapping results

There are two modes. Pick per query, not per project.

### Reflection (POCO)

Give the type a parameterless constructor and public settable properties. Column names are matched to property names case-insensitively, ignoring non-alphanumeric characters, so `first_name` binds to `FirstName` and `firstName` binds to `FirstName`.

```csharp
public class EmpRow
{
    public long Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}
```

Override the match with `[SqlNativeColumn]` when the SQL alias differs from the property name:

```csharp
[SqlNativeColumn("DeptName")]
public string Department { get; set; }
```

`DBNull` columns are skipped rather than throwing, and values are converted with `Convert.ChangeType` when the provider returns a different type than the property.

### Explicit ordinals (`QueryResult`)

Derive from `QueryResult` to take control. Resolve the ordinals once per result set, then read them by index — no per-row name lookup, and duplicate column names are no longer a problem.

```csharp
public partial class EmpRow : QueryResult
{
    public long Id { get; set; }
    public string Lastname { get; set; }
    public string Manager { get; set; }

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("Id"),
        reader.GetOrdinal("LastName"),
        QueryResult.OrdinalN(reader, "LastName", 1)   // the second one, from the join
    };

    public override void SetFields(DbDataReader r, int[] orders)
    {
        if (!r.IsDBNull(orders[0])) Id = r.GetInt64(orders[0]);
        if (!r.IsDBNull(orders[1])) Lastname = r.GetString(orders[1]);
        if (!r.IsDBNull(orders[2])) Manager = r.GetString(orders[2]);
    }
}
```

`TableRow` is an alias for `QueryResult` intended for rows that map to a table; it adds nothing on its own.

`OrdinalN` is a `protected static` helper on `QueryResult`, so it is only callable from inside a `QueryResult` subclass. It finds the *n*-th occurrence of a repeated column name — useful when a join brings back two columns with the same name.

---

## Generating a row class

Rather than hand-writing rows, run the query once and let SqlNative write the class:

```csharp
ctx.BuildQuery<DboEmp, DboEmp>((q, e1, e2) => q
    .Append($"SELECT {e1.Id}, {e1.Lastname}, {e2.Lastname} AS Manager")
    .Append($"FROM {e1}")
    .Append($"LEFT JOIN {e2} ON {e1.Deptid} = {e2.Id}"))
.Model<Program>("EmpWithManager")   // writes EmpWithManager.cs, next to the calling file
.Execute<EmpWithManager>();
```

`.Model<T>()` executes the query against the database, reads the schema of the result set and writes a `QueryResult` subclass for it. The file lands beside the calling source file — `[CallerFilePath]` decides where — and defaults to `<FileName>.<CallerMemberName>Model.cs` unless you pass a class name.

It only runs when `IsDevelopMode` is on, so it is inert in production.

---

## Typed repositories

`TableRepository<Row, Table>` is a higher-level wrapper for the common per-table operations, so a context can expose `ctx.Emp` and get a small repository:

```csharp
public class Db1Context : SqlNativeContext
{
    private DboEmpRepository emp;
    public DboEmpRepository Emp => emp ??= new(this);
}
```

The generated or hand-written repository gives you:

```csharp
ctx.Emp.All()                                             // List<Row>
ctx.Emp.AllAsync(ct)
ctx.Emp.Select(e => $"{e.Deptid} = @deptid", new { deptid = 4 })
ctx.Emp.SelectAsync(e => $"{e.Deptid} = @deptid", new { deptid = 4 }, ct)
ctx.Emp.Insert(row, OperationOptions.SkipPrimaryKey)     // or LoadRecord / LoadAndSkip / None
ctx.Emp.InsertAsync(row, OperationOptions.SkipPrimaryKey, ct)
ctx.Emp.Update(row, OperationOptions.LoadRecord)
ctx.Emp.UpdateAsync(row, OperationOptions.LoadRecord, ct)
ctx.Emp.InsertOrUpdate(rows, OperationOptions.LoadRecord)
ctx.Emp.InsertOrUpdateAsync(rows, OperationOptions.LoadRecord, ct)
ctx.Emp.Delete(row)
ctx.Emp.DeleteAsync(row, ct)
```

The write operations are abstract — a repository implements them for its own table, together with the `IsPrimaryKeySet` predicate that `InsertOrUpdate` uses for its insert/update split (for composite keys, every key column must be set). The row type must derive from `TableRow`:

```csharp
public partial class DboEmpRow : TableRow
{
    public long Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("Id"),
        reader.GetOrdinal("FirstName"),
        reader.GetOrdinal("LastName"),
    };

    public override void SetFields(DbDataReader reader, int[] orders)
    {
        if (!reader.IsDBNull(orders[0])) Id = reader.GetInt64(orders[0]);
        if (!reader.IsDBNull(orders[1])) Firstname = reader.GetString(orders[1]);
        if (!reader.IsDBNull(orders[2])) Lastname = reader.GetString(orders[2]);
    }
}

public partial class DboEmpRepository : TableRepository<DboEmpRow, DboEmp>
{
    private readonly Db1Context ctx;

    public DboEmpRepository(Db1Context ctx) => this.ctx = ctx;

    public override SqlNativeContext Context => ctx;

    public override Task<int> InsertAsync(List<DboEmpRow> rows, OperationOptions options = OperationOptions.SkipPrimaryKey, CancellationToken ct = default)
        => /* build and run the INSERT */;

    public override Task<int> UpdateAsync(List<DboEmpRow> rows, OperationOptions options = OperationOptions.None, CancellationToken ct = default)
        => /* build and run the UPDATE */;

    public override Task<int> DeleteAsync(List<DboEmpRow> rows, CancellationToken ct = default)
        => /* build and run the DELETE */;
}
```

The read operations — `All`, `Select` — are concrete and work out of the box.

---

## Configuration and lifecycle

Configuration lives on the context itself:

```csharp
var ctx = new Db1Context(connection)
{
    IsDevelopMode = true,
    LogSqlCommands = true,
    OwnsConnection = true,
};
```

| Setting | Default | Used for |
| --- | --- | --- |
| `IsDevelopMode` | `false` | Enables row-class generation and turns on SQL logging. |
| `LogSqlCommands` | `false` | Writes executed commands to the logger, or to the console when no logger is set. |
| `OwnsConnection` | `false` | When `true`, the connection is closed and disposed with the context. |
| `Logger` | `null` | The `Microsoft.Extensions.Logging` sink for executed SQL. |

`SqlNativeContext` implements `IDisposable` and `IAsyncDisposable`. It only closes the connection when `OwnsConnection` is `true`, so you can hand it a connection borrowed from an EF `DbContext` or a unit-of-work and leave the lifetime with whoever created it:

```csharp
await using var ctx = new Db1Context(connection) { OwnsConnection = false };
```

`Dispose()` forwards to `DisposeAsync()`, and close/dispose failures are swallowed — a context being torn down shouldn't mask the exception that got you there.

---

## Logging

Set `Logger` on the context to route the executed SQL through `Microsoft.Extensions.Logging` instead of the console:

```csharp
ctx.Logger = loggerFactory.CreateLogger("SqlNative");
ctx.BuildQuery(q => q.Append("SELECT * FROM emp")).ExecuteNonQuery();
// log: Information: SQL command: SELECT * FROM emp
```

The command is written at `Information` level with the SQL as the `Sql` property, so a log sink can pick it out by property name. While `Logger` is `null` the command goes to the console.

What gets written is the command text with the parameter *placeholders* still in place — every value is bound, so nothing is inlined. Treat the log as sensitive anyway, because the table and column names are in it.

Whether anything is written at all is controlled by `LogSqlCommands` on the context (`IsDevelopMode` turns it on too). The `SqlNative.AspNetCore` package wires the host's logger into the context for you.

---

## Supported databases

SqlNative has no provider dependency. You supply the `DbConnection`; the table and column classes control the SQL dialect.

| Database | Table base class | Function catalog |
| --- | --- | --- |
| SQL Server | `MsSqlTableStructure` | `SqlNative.MsSqlFunc` |
| MySQL | `MySqlTableStructure` | `SqlNative.MySqlFunc` |
| PostgreSQL | `PostgresTableStructure` | `SqlNative.PostgresFunc` |
| Oracle | `OracleTableStructure` | — |

The core `SqlFunctions` class currently exposes `SqlFunctions.NULL` and `SqlFunctions.CList`; the per-engine catalogs add the rest of the dialect's built-in functions.

---

## Related packages

| Package | Purpose |
| --- | --- |
| `SqlNative.Core` | Identifier helpers, CLR type mappers and the `DatabaseEngine` enum. Referenced automatically. |
| `SqlNative.MsSqlFunc` / `MySqlFunc` / `PostgresFunc` | Optional catalogs of each dialect's SQL functions, as `QueryPart` factories. |
| `SqlNative.AspNetCore` | DI registration: `builder.Services.AddSqlNativeContext<TContext, TConnection>(...)`. |
| `SqlNative.ModelGenerator` | The full code generator, for producing complete data-access layers from a live schema. |

---

## Current status

Worth knowing before you build on this:

- **Every execution method has an `Async` counterpart.** `Execute`, `ExecuteScalar` and `ExecuteNonQuery` have `ExecuteAsync`, `ExecuteScalarAsync` and `ExecuteNonQueryAsync` that take a `CancellationToken` and use the provider's real async I/O. The synchronous methods delegate to them, so both paths share one implementation. The `TableRepository` members have async counterparts too.
- **There is no `Select`/`Insert`/`Update`/`Delete` builder on the context.** The query surface is `BuildQuery`, and the CRUD surface is `TableRepository`. The model-based builders (`SelectModel`, `InsertModel`, `UpdateModel`, the join types) are not part of this package.
- **The `Query` builder does not add `LIMIT` / `OFFSET`.** Use `Append` to write the clause your engine wants — SQL Server spells it `OFFSET … ROWS FETCH NEXT … ROWS ONLY`.
- **`QueryPart ==` only emits `IS NULL` for the lower-case literal `"null"`.** `SqlFunctions.NULL` is upper case, so it produces `= NULL`.
- **Parameters are always bound**, never inlined — so numeric values are escaped by the provider. See [How parameters are handled](#how-parameters-are-handled).

## License

MIT — see [LICENSE](https://github.com/peymanjahanbazi/SqlNative/blob/main/LICENSE).
