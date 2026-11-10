# SqlNative

Type-safe, low-level SQL toolkit for .NET — write real SQL with C# identifiers for tables and columns (no typos), simple CRUD, and full access to every RDBMS feature. Unlike Entity Framework, it never hides the database.

You describe a table once as a C# class, and from then on `Books.Title` is a symbol the compiler checks. The SQL you write is the SQL that runs — window functions, CTEs, `OUTPUT`, query hints, `MERGE`, vendor extensions all stay available because nothing sits between you and the wire.

```csharp
var rows = ctx.BuildQuery<Books>((q, b) => q
    .Append($"SELECT {b.Title}, {b.PublishYear}")
    .Append($"FROM {b}")
    .Append($"WHERE {b.PublishYear} = @year")
    .Append($"ORDER BY {b.Title}")
    .SetParameters(new { year = 1998 }))
    .Execute<BooksRow>();
```

```sql
SELECT b.Title, b.PublishYear
FROM books as b
WHERE b.PublishYear = @year
ORDER BY b.Title
```

One codebase for **SQL Server, MySQL, PostgreSQL and Oracle** — SqlNative has no provider dependency, you supply the `DbConnection` and the table classes decide the dialect.

---

## Contents

- [Install](#install)
- [The example: the `books` database](#the-example-the-books-database)
- [1. Generate the model](#1-generate-the-model)
- [2. Your first query](#2-your-first-query)
- [3. Load a table](#3-load-a-table)
- [4. Insert](#4-insert)
- [5. Update](#5-update)
- [6. Delete](#6-delete)
- [7. Advanced querying](#7-advanced-querying)
- [API reference](#api-reference)
- [What SqlNative is not](#what-sqlnative-is-not)
- [Supported databases](#supported-databases)
- [Packages](#packages)
- [License](#license)

---

## Install

```bash
dotnet add package SqlNative
```

Targets `net461`, `netstandard2.0`, `net6.0`, `net8.0`, `net9.0` and `net10.0`.

You also need an ADO.NET provider — SqlNative references none of them:

| Provider | Package | Engine |
| --- | --- | --- |
| `MySql.Data.MySqlClient` / `MySqlConnector` | `MySql.Data` / `MySqlConnector` | `MySqlTableStructure` |
| `Microsoft.Data.SqlClient` | `Microsoft.Data.SqlClient` | `MsSqlTableStructure` |
| `Npgsql` | `Npgsql` | `PostgresTableStructure` |
| `Oracle.ManagedDataAccess` | `Oracle.ManagedDataAccess.Core` | `OracleTableStructure` |

The optional [packages](#packages) in this repo add a code generator, per-dialect SQL function catalogs and ASP.NET Core DI registration.

---

## The example: the `books` database

Every snippet below runs against the sample MySQL database used by the test project:

```sql
CREATE TABLE `books` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(200) NOT NULL,
  `publisher_name` varchar(150) NOT NULL,
  `publish_year` smallint(6) NOT NULL,
  `edition` varchar(50) DEFAULT NULL,
  `summary` text DEFAULT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `firstName` varchar(100) NOT NULL,
  `lastName` varchar(100) NOT NULL,
  `registerDate` date NOT NULL,
  PRIMARY KEY (`id`)
);
```

The complete generated output for this database is checked in at [`src/SqlNative.ConsoleTest/Data`](src/SqlNative.ConsoleTest/Data) — [`Books.cs`](src/SqlNative.ConsoleTest/Data/Books.cs), [`Users.cs`](src/SqlNative.ConsoleTest/Data/Users.cs) and [`WorldContext.cs`](src/SqlNative.ConsoleTest/Data/WorldContext.cs). Everything in this README is drawn from those files.

Connection string used throughout:

```csharp
string connStr = "Server=localhost;Database=books;User=root;Password=root;";
```

---

## 1. Generate the model

Point the generator at a live database and it writes the whole data-access layer for you:

```bash
dotnet tool install -g SqlNative.ModelGenerator

sqlnative-gen \
  --cs "Server=localhost;Database=books;User=root;Password=root;" \
  --engine mysql \
  --folder "./Models" \
  --package "Books.Models" \
  --context WorldContext
```

```
Database 'books': generated 5 table model(s) and context 'WorldContext'.
  Books.cs
  UserBookInterest.cs
  UserHistory.cs
  UserRecommandations.cs
  Users.cs
  WorldContext.cs
```

Each table file contains three classes. For `books`:

**`Books`** — the table structure. One `TableColumn` per column, so every column reference in your SQL is a compile-checked symbol:

```csharp
public partial class Books : MySqlTableStructure
{
    public override string TableName { get; } = "books";
    public override string QuotedTableName { get; } = "books";

    public TableColumn Id { get; private set; }
    public TableColumn Title { get; private set; }
    public TableColumn PublisherName { get; private set; }
    public TableColumn PublishYear { get; private set; }
    public TableColumn Edition { get; private set; }
    public TableColumn Summary { get; private set; }

    public Books()
    {
        Id            = new MySqlColumn("id", false, "int(11)", typeof(int), 10, this);
        Title         = new MySqlColumn("title", false, "varchar(200)", typeof(string), 200, this);
        PublisherName = new MySqlColumn("publisher_name", false, "varchar(150)", typeof(string), 150, this);
        PublishYear   = new MySqlColumn("publish_year", false, "smallint(6)", typeof(short), 5, this);
        Edition       = new MySqlColumn("edition", false, "varchar(50)", typeof(string), 50, this);
        Summary       = new MySqlColumn("summary", false, "text", typeof(string), 65535, this);
    }
}
```

The constructor passes the **real** database column name (`"id"`, `"publisher_name"`) — so the SQL keeps the database's own casing — plus the raw SQL type and the CLR type it maps to. The C# property name is the column name in PascalCase.

**`BooksRow`** — the CLR-typed row, with ordinals resolved once per result set:

```csharp
public partial class BooksRow : TableRow
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PublisherName { get; set; } = null!;
    public short PublishYear { get; set; }
    public string? Edition { get; set; } = null!;
    public string? Summary { get; set; } = null!;

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("id"),
        reader.GetOrdinal("title"),
        reader.GetOrdinal("publisher_name"),
        reader.GetOrdinal("publish_year"),
        reader.GetOrdinal("edition"),
        reader.GetOrdinal("summary")
    };

    public override void SetFields(DbDataReader reader, int[] orders)
    {
        if (!reader.IsDBNull(orders[0])) Id = reader.GetInt32(orders[0]);
        if (!reader.IsDBNull(orders[1])) Title = reader.GetString(orders[1]);
        if (!reader.IsDBNull(orders[2])) PublisherName = reader.GetString(orders[2]);
        // ...
    }
}
```

Nullable columns get a nullable CLR type (`string? Edition`); non-null ones get the value type.

**`BooksRepository`** — the CRUD operations for that table. You never write these; the generator does.

**`WorldContext`** — the context, exposing one repository per table:

```csharp
public partial class WorldContext : SqlNativeContext
{
    private BooksRepository? _Books;
    public BooksRepository Books => _Books ??= new(this);

    private UsersRepository? _Users;
    public UsersRepository Users => _Users ??= new(this);
}
```

You can also write these classes by hand — they are plain C# with no attributes or magic, and the generator is only a shortcut.

> The generator overwrites existing files without prompting, and generation **fails** if the context name collides with a table class name.

---

## 2. Your first query

The context wraps a connection. Everything else hangs off it. The generated context has no constructor of its own — it inherits the parameterless one and takes the connection through the `Connection` property:

```csharp
using MySql.Data.MySqlClient;
using SqlNative;
using Books.Models;

using var connection = new MySqlConnection(connStr);
connection.Open();

using var ctx = new WorldContext
{
    Connection = connection,
    OwnsConnection = true
};
```

`OwnsConnection = true` means the context closes the connection when it is disposed. Leave it `false` when the connection is borrowed from somewhere else — an EF `DbContext`, a unit of work, a connection pool — and let whoever created it dispose it.

If you write the context by hand, you can give it a constructor instead:

```csharp
public class WorldContext : SqlNativeContext
{
    public WorldContext(DbConnection connection) => Connection = connection;
}
```

Now write SQL. `BuildQuery` takes the tables and returns a `Query` — a builder that has **not** run yet:

```csharp
var query = ctx.BuildQuery<Books>((q, b) => q
    .Append($"SELECT {b.Title}, {b.PublishYear}")
    .Append($"FROM {b}")
    .Append($"WHERE {b.PublishYear} = @year")
    .Append($"ORDER BY {b.Title}")
    .SetParameters(new { year = 1998 }));

Console.WriteLine(query.Sql);
// SELECT b.Title, b.PublishYear
// FROM books as b
// WHERE b.PublishYear = @year
// ORDER BY b.Title

var rows = query.Execute<BooksRow>();
```

Because the query is an object, you can inspect the SQL before it hits the database, or extend it:

```csharp
query.Append("AND b.PublisherName = @publisher");
var rows = query.Execute<BooksRow>();
```

`TableStructure.ToString()` renders as `books as b` — the alias is assigned automatically and kept unique across the query. You never set it by hand, and you never write the table name as a string.

---

## 3. Load a table

The generated repository gives you the common per-table reads:

```csharp
// every row
var all = ctx.Books.All();

// filtered
var from1998 = ctx.Books.Select(
    b => $"{b.PublishYear} = @year",
    new { year = 1998 });

// the predicate can use any column
var longTitles = ctx.Books.Select(b => $"{b.Title} LIKE @t", new { t = "The%" });
```

`ctx.Books.Select(...)` runs `SELECT b.* FROM books as b WHERE ...` and maps straight into `BooksRow`.

Rows can also be plain POCOs — SqlNative matches column names to property names case-insensitively, ignoring non-alphanumeric characters, so `publisher_name` binds to `PublisherName` and `first_name` would bind to `FirstName`:

```csharp
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PublisherName { get; set; } = null!;
    public short PublishYear { get; set; }
}

var dtos = ctx.BuildQuery<Books>((q, b) => q
        .Append($"SELECT {b.Star}")
        .Append($"FROM {b}"))
    .Execute<BookDto>();
```

Override the match with `[SqlNativeColumn]` when the SQL alias differs from the property name:

```csharp
[SqlNativeColumn("publisher_name")]
public string Publisher { get; set; } = null!;
```

---

## 4. Insert

```csharp
var book = new BooksRow
{
    Title = "The Pragmatic Programmer",
    PublisherName = "Addison-Wesley",
    PublishYear = 1999,
    Edition = "2nd",
    Summary = "A guide to the craft."
};

ctx.Books.Insert(book, OperationOptions.LoadAndSkip);
```

`LoadAndSkip` omits the primary key from the INSERT and reads the row back, so the object you passed in comes back populated. The other options are `SkipPrimaryKey` (the default), `LoadRecord` and `None`.

The same insert without the repository — still type-checked against the table:

```csharp
var affected = ctx.BuildQuery<Books>((q, b) => q
    .Append($"INSERT INTO {b}")
    .Append($"({b.Title}, {b.PublisherName}, {b.PublishYear})")
    .Append($"VALUES (@Title, @PublisherName, @PublishYear)")
    .SetParameters(new { book.Title, book.PublisherName, book.PublishYear }))
    .ExecuteNonQuery();
```

```sql
INSERT INTO books
(Title, PublisherName, PublishYear)
VALUES ( @Title, @PublisherName, @PublishYear )
```

Insert many rows at once — the repository batches them into a single multi-row `VALUES` statement:

```csharp
var rows = new List<BooksRow> { book1, book2, book3 };
ctx.Books.Insert(rows);
```

---

## 5. Update

```csharp
book.PublishYear = 2000;
ctx.Books.Update(book, OperationOptions.LoadRecord);
```

`LoadRecord` (or `LoadAndSkip`) re-reads the row afterwards, so database-side defaults and triggers land on your object; `None` — the default — just updates. The generated `UPDATE` sets every non-key column and matches on the primary key:

```sql
UPDATE books
SET Title = @Title, PublisherName = @PublisherName, PublishYear = @PublishYear
WHERE id = @Key
```

Update a list and it is batched:

```csharp
ctx.Books.Update(rows, OperationOptions.LoadRecord);
```

To insert or update in one call, `InsertOrUpdate` sends the rows whose primary key is unset (null or the CLR default for the key — composite keys supported) to `Insert`, and the rest to `Update`, batching as usual:

```csharp
ctx.Books.InsertOrUpdate(rows, OperationOptions.LoadRecord);
```

The raw form works the same way:

```csharp
ctx.BuildQuery<Books>((q, b) => q
    .Append($"UPDATE {b}")
    .Append($"SET {b.PublishYear} = @PublishYear")
    .Append($"WHERE {b.Id} = @Id")
    .SetParameters(new { PublishYear = 2000, Id = 1 }))
    .ExecuteNonQuery();
```

---

## 6. Delete

```csharp
ctx.Books.Delete(book);            // by key, through the repository
ctx.Books.Delete(rows);            // a list — batched
ctx.Books.Delete(b => $"{b.PublishYear} = @year");
```

Or straight off a query, for statements that don't need a row object:

```csharp
ctx.BuildQuery<Books>((q, b) => q
    .Append($"DELETE FROM {b}")
    .Append($"WHERE {b.PublishYear} = @year")
    .SetParameters(new { year = 1998 }))
    .ExecuteNonQuery();
```

```sql
DELETE FROM books
WHERE PublishYear = @year
```

---

## 7. Advanced querying

### Joins

Add a table to the generic list and get an alias for it too:

```csharp
var rows = ctx.BuildQuery<Books, Users>((q, b, u) => q
    .Append($"SELECT {b.Title}, {u.FirstName} AS Reader")
    .Append($"FROM {b}")
    .Append($"INNER JOIN {u} ON {u.Id} = {b.Id}"))
    .Execute<BookWithReader>();
```

```sql
SELECT b.Title, u.FirstName AS Reader
FROM books as b
INNER JOIN users as u ON u.Id = b.Id
```

Up to fifteen tables are supported by `BuildQuery`.

### Group by and having

```csharp
var stats = ctx.BuildQuery<Users>((q, u) => q
    .Append($"SELECT {u.LastName}, COUNT(*) AS UserCount")
    .Append($"FROM {u}")
    .Append($"GROUP BY {u.LastName}")
    .Append($"HAVING COUNT(*) > 1"))
    .Execute<UserStat>();
```

```sql
SELECT u.LastName, COUNT(*) AS UserCount
FROM users as u
GROUP BY u.LastName
HAVING COUNT(*) > 1
```

### Raw SQL, with the model still available

`BuildQuery` is the escape hatch. You write the SQL; the table classes still supply the aliases, the quoting and the parameters:

```csharp
var rows = ctx.BuildQuery<Books>((q, b) => q
    .Append($"WITH recent AS (SELECT {b.Title}, {b.PublishYear} FROM {b} WHERE {b.PublishYear} > 2000)")
    .Append($"SELECT {b.Title}, {b.PublishYear} FROM recent")
    .Append($"ORDER BY {b.PublishYear} DESC"))
    .Execute<BooksRow>();
```

```sql
WITH recent AS (SELECT b.Title, b.PublishYear FROM books as b WHERE b.PublishYear > 2000)
SELECT b.Title, b.PublishYear FROM recent
ORDER BY b.PublishYear DESC
```

Nothing stops you using window functions, `MERGE`, recursive CTEs or a hint — this is your SQL.

### `IN` lists

`AppendFor` repeats a fragment once per item, with an optional separator. Bind the values as parameters rather than interpolating them into the text:

```csharp
var years = new[] { 1998, 1999, 2000 };
var parameters = new Dictionary<string, object>();
var placeholders = new List<string>();

for (var i = 0; i < years.Length; i++)
{
    parameters[$"@year{i}"] = years[i];
    placeholders.Add($"@year{i}");
}

var rows = ctx.BuildQuery<Books>((q, b) => q
    .Append($"SELECT {b.Star} FROM {b}")
    .Append($"WHERE {b.PublishYear} IN ({string.Join(", ", placeholders)})")
    .SetParameters(parameters))
    .Execute<BooksRow>();
```

### Generate a row class for an ad-hoc query

Run the query and let SqlNative write the matching row class for the result set:

```csharp
ctx.BuildQuery<Books, Users>((q, b, u) => q
    .Append($"SELECT {b.Title}, {u.FirstName} AS Reader")
    .Append($"FROM {b}")
    .Append($"INNER JOIN {u} ON {u.Id} = {b.Id}"))
.Model<Program>("BookWithReader")
.Execute<BookWithReader>();
```

The file is written next to the calling source file as `BookWithReader.cs`. This is the quickest way to get a correct row type for an awkward query, and it is how the checked-in examples in `SqlNative.ConsoleTest` were produced. It only runs when `IsDevelopMode` is on.

---

## API reference

### The query builder

`BuildQuery` returns a `Query`. You finish it and execute it yourself:

```csharp
var q = ctx.BuildQuery<Books>((q, b) => q
    .Append($"SELECT {b.Star}")
    .Append($"FROM {b}"));

q.Append("ORDER BY PublishYear DESC");
Console.WriteLine(q.Sql);
var rows = q.Execute<BooksRow>();
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
| `Config(Action)` | Change the table structure before the SQL is built. |
| `Model<T>(className)` | Write a row class for this result set. |
| `Sql` | The generated SQL, without executing it. |

### Async

Every execution method has an `Async` counterpart that takes an optional `CancellationToken`:

```csharp
var rows = await ctx.BuildQuery<Books>((q, b) => q
        .Append($"SELECT {b.Star}")
        .Append($"FROM {b}"))
    .ExecuteAsync<BooksRow>(ct);
```

The async methods use the provider's real async I/O (`ExecuteReaderAsync`, `ReadAsync`,
`ExecuteScalarAsync`, `ExecuteNonQueryAsync`), so the calling thread is released while the
database works. The synchronous methods delegate to the async ones, so both paths run the
same code.

The `TableRepository` members have async counterparts as well:

```csharp
await ctx.Books.SelectAsync(b => $"{b.PublishYear} = @year", new { year = 1998 }, ct);
await ctx.Books.DeleteAsync(book, ct);
```

### How parameters are handled

`Query.CreateCommand` binds every parameter as a real `DbParameter` on the command:

| Parameter value | What happens |
| --- | --- |
| `null` | Bound with `DBNull.Value`. |
| Anything else | Bound with its value. |

So strings, dates, GUIDs, arrays and `DBNull` all reach the database as parameters, and numeric values are escaped by the provider like everything else. The `@` prefix is optional — `@year` and `year` both bind to `@year`.

### Mapping results

Two modes. Pick per query, not per project.

**Reflection (POCO)** — a parameterless constructor and public settable properties; column names match property names case-insensitively, ignoring non-alphanumeric characters. `DBNull` columns are skipped rather than throwing, and values are converted with `Convert.ChangeType` when the provider returns a different type.

**Explicit ordinals (`QueryResult`)** — derive from `QueryResult` to take control. Resolve the ordinals once per result set, then read them by index — no per-row name lookup, and duplicate column names stop being a problem:

```csharp
public partial class BooksRow : TableRow
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("id"),
        reader.GetOrdinal("title")
    };

    public override void SetFields(DbDataReader r, int[] orders)
    {
        if (!r.IsDBNull(orders[0])) Id = r.GetInt32(orders[0]);
        if (!r.IsDBNull(orders[1])) Title = r.GetString(orders[1]);
    }
}
```

`TableRow` is an alias for `QueryResult` intended for rows that map to a table. Inside a `QueryResult` subclass, `OrdinalN(reader, "Name", 1)` resolves the second occurrence of a repeated column name in a join.

### Configuration and lifecycle

```csharp
var ctx = new WorldContext
{
    Connection = connection,
    IsDevelopMode = true,
    LogSqlCommands = true,
    OwnsConnection = true,
};
```

| Setting | Default | Used for |
| --- | --- | --- |
| `IsDevelopMode` | `false` | Enables row-class generation and turns on SQL logging. |
| `LogSqlCommands` | `false` | Writes executed commands to the logger or the console. |
| `OwnsConnection` | `false` | When `true`, the connection is closed and disposed with the context. |
| `Logger` | `null` | The `Microsoft.Extensions.Logging` sink for executed SQL. |

`SqlNativeContext` implements `IDisposable` and `IAsyncDisposable`. It only closes the connection when `OwnsConnection` is `true`, so you can hand it a connection borrowed from an EF `DbContext` or a unit-of-work and leave the lifetime with whoever created it.

### Logging

Set `Logger` on the context to route the executed SQL through `Microsoft.Extensions.Logging` instead of the console:

```csharp
ctx.Logger = loggerFactory.CreateLogger("SqlNative");
ctx.BuildQuery(q => q.Append("SELECT * FROM books")).ExecuteNonQuery();
// log: Information: SQL command: SELECT * FROM books
```

The command is written at `Information` level with the SQL as the `Sql` property. What gets written is the command text with the parameter *placeholders* still in place — every value is bound, so nothing is inlined.

---

## What SqlNative is not

| | |
| --- | --- |
| Tables and columns as typed classes | Yes |
| Row → POCO mapping | Yes — reflection, or explicit ordinals for full control |
| Query builder (`BuildQuery`) | Yes — returns a `Query` you can inspect before it runs |
| Per-table CRUD (`ctx.Books.Insert(row)`) | Yes — via `TableRepository<Row, Table>` |
| Code generation from a live schema | Yes |
| Async query execution | Yes — `ExecuteAsync`, `ExecuteScalarAsync`, `ExecuteNonQueryAsync` |
| Change tracking | No — you call `Insert` or `Update` yourself |
| Lazy loading / proxies | No |
| Migrations | No — you write the DDL |
| Unit of work / identity map | No — every `Query` stands alone |
| LINQ provider | No — you write SQL |

The trade is deliberate. If you want an ORM that generates SQL for you and gets out of the way, use EF Core or Dapper. If you want an ORM that stays out of your SQL, use this.

---

## Supported databases

| Database | Table base class | Function catalog |
| --- | --- | --- |
| SQL Server | `MsSqlTableStructure` | `SqlNative.MsSqlFunc` |
| MySQL | `MySqlTableStructure` | `SqlNative.MySqlFunc` |
| PostgreSQL | `PostgresTableStructure` | `SqlNative.PostgresFunc` |
| Oracle | `OracleTableStructure` | — |

The core `SqlUtils` class currently exposes `SqlUtils.NULL`, `SqlUtils.Join` and `SqlUtils.Select`; the per-engine catalogs add the rest of the dialect's built-in functions.

> The `Query` builder emits only what you `Append`. There is no `LIMIT` / `OFFSET` helper — write the clause your engine wants (SQL Server spells it `OFFSET … ROWS FETCH NEXT … ROWS ONLY`).

---

## Packages

| Package | Purpose |
| --- | --- |
| [`SqlNative`](src/SqlNative/README.md) | The query builder, table/column model, result mapping and CRUD. This is the package above. |
| [`SqlNative.Core`](src/SqlNative.Core/README.md) | Identifier helpers, CLR type mappers and the `DatabaseEngine` enum. Referenced automatically. |
| [`SqlNative.ModelGenerator`](src/SqlNative.ModelGenerator/README.md) | The `sqlnative-gen` tool — generates complete data-access layers from a live schema. |
| [`SqlNative.MsSqlFunc`](src/SqlNative.MsSqlFunc) / [`MySqlFunc`](src/SqlNative.MySqlFunc) / [`PostgresFunc`](src/SqlNative.PostgresFunc) | Optional catalogs of each dialect's SQL functions, as `QueryPart` factories. |
| [`SqlNative.AspNetCore`](src/SqlNative.AspNetCore/README.md) | DI registration: `builder.Services.AddSqlNativeContext<TContext, TConnection>(...)`. |

---

## License

MIT — see [LICENSE](LICENSE).
