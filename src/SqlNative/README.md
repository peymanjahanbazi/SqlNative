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
- [Generating models](#generating-models)
- [Typed entities](#typed-entities)
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
| `Select` / `Insert` / `Update` / `Delete` | Yes — each returns a `Query` you can inspect before it runs |
| Per-table CRUD (`ctx.Emp.Insert(row)`) | Yes — via `DbEntity<T>` |
| Code generation from a live schema | Yes |
| Change tracking | No — `Store(row)` decides insert vs. update by looking at the key |
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

public partial class DboEmp : MsSqlTableStructure, HasPrimaryKey
{
    public override string TableName { get; } = "Emp";
    protected override string QuotedTableName { get; } = "dbo.Emp";
    public override string Schema { get; } = "dbo";

    public TableColumn Id { get; }
    public TableColumn Firstname { get; }
    public TableColumn Lastname { get; }
    public TableColumn Deptid { get; }

    public override TableColumn PKey => Id;

    public DboEmp()
    {
        Id       = new MsSqlColumn("Id", false, "bigint", typeof(long), this);
        Firstname = new MsSqlColumn("FirstName", false, "nvarchar", typeof(string), this);
        Lastname = new MsSqlColumn("LastName", false, "nvarchar", typeof(string), this);
        Deptid   = new MsSqlColumn("DeptId", false, "bigint", typeof(long), this);
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

var rows = ctx.Select<DboEmp>(e => new SelectModel
{
    From     = e,
    Select   = $"{e.Id}, {e.Firstname}, {e.Lastname}",
    Where    = $"{e.Deptid} = @deptid",
    Parameters = new { deptid = 4 },
    OrderBy  = $"{e.Lastname} ASC",
    Limit    = 50
}).Execute<EmpRow>();
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

`HasPrimaryKey` marks a table that has one, enabling `DeleteById` / `DeleteByIds`.

### Aliases

`TableStructure.ToString()` renders as `[schema].[Table] as [alias]` — except in `Insert`, `Update` and `Delete`, where `DisableAlias = true` strips it. You never set either by hand: `Select` and `BuildQuery` do it.

When you select from more than one table, SqlNative assigns a short alias for each, derived from the table name and guaranteed unique across the query:

```csharp
ctx.Select<DboEmp, DboEmp>((e1, e2) => new SelectModel
{
    From   = e1,
    Select = $"{e1.Id}, {e1.Lastname}, {e2.Lastname} AS Manager",
    Joins  = [new LeftJoin { Table = e2, On = $"{e1.Deptid} = {e2.Id}" }]
})
// FROM [dbo].[Emp] as e LEFT JOIN [dbo].[Emp] as e1 ON e.DeptId = e1.Id
```

### Expressions

`QueryPart` is an operator-overloaded string wrapper for building expressions:

```csharp
QueryPart.IsNull      // "NULL"
QueryPart age = 30;   // "30"
QueryPart active = true; // "TRUE"

var filter = $"{a.Lastname} == NULL";   // a.Lastname IS NULL
var range  = $"{a.Id} >= {1}";          // a.Id >= 1
var neg    = !$"{a.Name}.Contains(\"x\")";  // NOT ...
```

`==` emits `IS NULL` when either operand is the literal `"null"`, and `=` otherwise. `+` concatenates two `QueryPart`s with a `+`, which is how you write arithmetic like `a.Price * a.Qty`; note that `TableColumn + TableColumn` is a different overload and produces a comma-separated select list instead.

---

## Building and executing queries

`Select`, `Insert` and `Update` return a `Query` — a builder that has not run yet. You finish it and execute it yourself, which means you can inspect or extend the SQL first:

```csharp
var q = ctx.Select<DboEmp>(e => new SelectModel { From = e, Select = e.Star });

q.Append("ORDER BY LastName DESC");     // add your own clauses

Console.WriteLine(q.Sql);               // inspect before it hits the database
var rows = q.Execute<EmpRow>();
```

| Method | Purpose |
| --- | --- |
| `Append(string)` | Append a line of SQL. Blank lines are ignored. |
| `AppendIf(bool, string)` | Append only when the condition holds. |
| `AppendFor<T>(items, join, func)` | Append once per item with a separator — useful for `IN` lists. |
| `SetParameters(object)` | Bind parameters, from a dictionary or an anonymous object. |
| `Execute<T>()` | Run and map to `List<T>`. |
| `ExecuteScalar()` | Run and return the first column of the first row. |
| `ExecuteNonQuery()` | Run and return the affected row count. |
| `ExecuteAsync<T>(ct)` | Awaitable `Execute<T>()`. |
| `ExecuteScalarAsync(ct)` | Awaitable `ExecuteScalar()`. |
| `ExecuteNonQueryAsync(ct)` | Awaitable `ExecuteNonQuery()`. |
| `Sql` | The generated SQL, without executing it. |

### Async

Every execution method has an `Async` counterpart that takes an optional `CancellationToken`:

```csharp
var rows = await ctx.Select<DboEmp>(e => new SelectModel { From = e, Select = e.Star })
                     .ExecuteAsync<EmpRow>(ct);

var affected = await ctx.InsertAsync<DboEmp>(e => new InsertModel { /* ... */ }, ct);
var updated  = await ctx.UpdateAsync<DboEmp>(e => new UpdateModel { /* ... */ }, ct);
```

The async methods use the provider's real async I/O (`ExecuteReaderAsync`, `ReadAsync`,
`ExecuteScalarAsync`, `ExecuteNonQueryAsync`), so the calling thread is released while the
database works. The synchronous methods delegate to the async ones, so both paths run the
same code.

---

## CRUD

### Select

```csharp
ctx.Select<DboEmp>(e => new SelectModel
{
    From      = e,
    Select    = $"{e.Id}, {e.Lastname}",
    Joins     = [new InnerJoin { Table = dept, On = $"{e.Deptid} = {dept.Id}" }],
    Where     = $"{e.Lastname} LIKE @name",
    GroupBy   = $"{e.Deptid}",
    Having    = $"COUNT(*) > 1",
    OrderBy   = $"{e.Lastname}",
    Limit     = 20,
    Offset    = 40,
    Parameters = new { name = "%z%" }
});
```

`Select` has overloads for one table up to fifteen. Join types are `LeftJoin`, `RightJoin`, `InnerJoin`, `FullJoin` and `CrossJoin`. `Limit` and `Offset` are emitted as `LIMIT` / `OFFSET` — check the syntax matches your engine, since SQL Server spells it differently.

### Insert

```csharp
var affected = ctx.Insert<DboEmp>(e => new InsertModel
{
    Columns = e.Firstname + e.Lastname + e.Deptid,
    Values  = "@Firstname, @Lastname, @Deptid",
    Parameters = new { Firstname = "Ada", Lastname = "Lovelace", Deptid = 4 }
});
```

The insert executes immediately and returns the affected count. Use `InsertAsync` to await it
with a `CancellationToken`.

Append `OUTPUT INSERTED.*` (or `RETURNING *`) to read the row back.

### Update

```csharp
ctx.Update<DboEmp>(e => new UpdateModel
{
    Set        = $"{e.Lastname} = @lastname",
    Where      = $"{e.Id} = @id",
    Parameters = new { lastname = "Byron", id = 12 }
});
```

Like `Insert`, it executes immediately and returns the affected count; `UpdateAsync` is the
awaitable counterpart.

### Delete

The delete helpers execute immediately and return the affected count — they do not return a `Query`:

```csharp
ctx.DeleteWhere<DboEmp>(e => $"{e.Deptid} = @deptid", new { deptid = 4 });
ctx.DeleteById<DboEmp, long>(12);
ctx.DeleteByIds<DboEmp, long>([12, 13, 14]);   // batched by MaxParametersPerStatement
ctx.DeleteAll<DboEmp>();
```

Each helper also has an awaitable counterpart:

```csharp
await ctx.DeleteWhereAsync<DboEmp>(e => $"{e.Deptid} = @deptid", new { deptid = 4 }, ct);
await ctx.DeleteByIdAsync<DboEmp, long>(12, ct);
await ctx.DeleteByIdsAsync<DboEmp, long>([12, 13, 14], ct);   // each batch is a separate await
await ctx.DeleteAllAsync<DboEmp>(ct);
```

### Arbitrary SQL

When a query doesn't fit the model builders, use `BuildQuery` directly:

```csharp
var rows = ctx.BuildQuery((Query q, DboEmp e) => q
    .Append($"SELECT {e.Id}, {e.Lastname}")
    .Append($"FROM {e}")
    .Append($"WHERE {e.Deptid} = @deptid")
    .SetParameters(new { deptid = 4 }))
    .Execute<EmpRow>();
```

`BuildQuery` handles the aliasing for you and has overloads up to fifteen tables.

---

## How parameters are handled

`Query.CreateCommand` treats each parameter differently, and the distinction matters when you read a query plan or a log:

| Parameter value | What happens |
| --- | --- |
| `null` | Replaced in the SQL with the literal `NULL`. |
| `byte`, `short`, `int`, `long`, `decimal`, `double`, `float`, `bool` | Inlined as a literal, formatted with the invariant culture. |
| Anything else | Bound as a real `DbParameter` on the command. |

So only strings, dates, GUIDs, arrays and `DBNull` actually reach the database as parameters. This keeps the parameter count down for the common case, but it also means an inlined number is not escaped. If you are passing a value that came from user input and is numeric, cast it to `string` to force parameter binding.

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
public partial class EmpRow : TableRow<long>
{
    public long Id { get; set; }
    public string Lastname { get; set; }
    public string Manager { get; set; }
    public override long Key => Id;

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

`TableRow<TPk>` is a `QueryResult` that also exposes `Key`, which is what `DbEntity` uses to decide insert vs. update.

---

## Generating models

Rather than hand-writing rows, run the query once and let SqlNative write the class:

```csharp
ctx.Select<DboEmp, DboEmp>((e1, e2) => new SelectModel
{
    From   = e1,
    Select = $"{e1.Id}, {e1.Lastname}, {e2.Lastname} AS Manager",
    Joins  = [new LeftJoin { Table = e2, On = $"{e1.Deptid} = {e2.Id}" }]
})
.Model<Program>("EmpWithManager")   // writes ./EmpWithManager.cs, next to the calling file
.Execute<EmpWithManager>();
```

`.Model<T>()` executes the query against the database, reads the schema of the result set and writes a `QueryResult` subclass for it. The file lands beside the calling source file — `[CallerFilePath]` decides where — and defaults to `<FileName>.<CallerMemberName>Model.cs` unless you pass a class name. This is the fastest way to get a correct row type for an awkward query, and it's how the checked-in examples in `SqlNative.ConsoleTest` were produced.

---

## Typed entities

`DbEntity<Row, Structure, Pk>` is a higher-level wrapper for the common per-table operations, so a context can expose `ctx.Emp` and get a small repository:

```csharp
public class Db1Context : SqlNativeContext
{
    private DboEmpEntity emp;
    public DboEmpEntity Emp => emp ??= new(this);
}
```

The generated or hand-written entity gives you:

```csharp
ctx.Emp.All()                                             // List<Row>
ctx.Emp.Select(e => $"{e.Deptid} = @deptid", new { deptid = 4 })
ctx.Emp.Insert(row, InsertOptions.SkipPrimaryKey)        // or LoadRecord / LoadAndSkip / both
ctx.Emp.Update(row, reload: true)
ctx.Emp.Delete(row)
ctx.Emp.Store(row)                                        // insert or update, based on whether Key is set
ctx.Emp.Store(rows)                                       // splits the list and does both
```

`Store` is the one worth remembering: give it a row with a default key and it inserts, give it a row with a key and it updates. `InsertOptions.LoadRecord` appends the engine's output clause so the generated key and defaults come back on the object you passed in.

Batch operations chunk by `Config.MaxParametersPerStatement` so a 50,000-row delete doesn't blow past the provider's parameter limit.

---

## Configuration and lifecycle

```csharp
ctx.Config = new SqlNativeContextConfig
{
    IsDevelopMode = true,
    MaxPacketSize = 1_000_000,
    MaxParametersPerStatement = 60_000
};
```

| Setting | Default | Used for |
| --- | --- | --- |
| `MaxParametersPerStatement` | `60_000` | Batch size for `DeleteByIds` and for the batching in generated entities. |
| `IsDevelopMode` | `true` on a bare context | Development flag. The core library does not read it yet — see below. |
| `MaxPacketSize` | `1_000_000` | Development flag. The core library does not read it yet — see below. |

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

The command is written at `Information` level with the SQL as the `Sql` property, so a log sink can pick it out by property name. While `Logger` is `null` the command goes to the console, as it always did.

What gets written is the command text with parameters already substituted — numeric and boolean parameters are inlined as literals, everything else stays a `@parameter` placeholder. Treat the log as sensitive if your inline values are.

Whether anything is written at all is controlled by `LogSqlCommands` on the config (`IsDevelopMode` turns it on too). The `SqlNative.AspNetCore` package wires the host's logger into the context for you.

---

## Supported databases

SqlNative has no provider dependency. You supply the `DbConnection`; the table and column classes control the SQL dialect.

| Database | Table base class | Function catalog |
| --- | --- | --- |
| SQL Server | `MsSqlTableStructure` | `SqlNative.MsSqlFunc` |
| MySQL | `MySqlTableStructure` | `SqlNative.MySqlFunc` |
| PostgreSQL | `PostgresTableStructure` | `SqlNative.PostgresFunc` |
| Oracle | `OracleTableStructure` | — |

The core `SqlFunctions` class currently exposes `SqlFunctions.NULL`; the per-engine catalogs add the rest of the dialect's built-in functions.

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

- **Query execution is synchronous or asynchronous — your choice.** `Execute`, `ExecuteScalar` and `ExecuteNonQuery` have `Async` counterparts (`ExecuteAsync`, `ExecuteScalarAsync`, `ExecuteNonQueryAsync`) that take a `CancellationToken` and use the provider's real async I/O. The synchronous methods delegate to them, so both paths share one implementation. The delete helpers and the `DbEntity` members have async counterparts too.
- **Every executed command is written to the console.** `Query.CreateCommand` calls `Console.WriteLine(command.CommandText)` unconditionally. This is exactly what `IsDevelopMode` is meant to gate, and it is not yet wired up — until it is, plan for that output in production.
- **`Limit` / `Offset` are emitted verbatim** as `LIMIT` / `OFFSET`, which is MySQL and PostgreSQL syntax but not SQL Server's (`OFFSET … ROWS FETCH NEXT … ROWS ONLY`). Use `Append` if you need the other spelling.
- **Inlined parameters are not escaped.** See [How parameters are handled](#how-parameters-are-handled).

## License

MIT — see [LICENSE](https://github.com/peymanjahanbazi/SqlNative/blob/main/LICENSE).