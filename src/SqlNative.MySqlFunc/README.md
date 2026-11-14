# SqlNative.MySqlFunc

Optional MySQL and MariaDB function catalog for [SqlNative](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative/README.md) — the light-weight ORM that keeps your SQL visible. It adds `MySqlFunctions`, a catalog of 376 built-in MySQL functions and operators as ordinary C# methods.

Each method returns the SQL text for one function, so you write the query as usual and let the catalog handle the function name, argument order and compatibility details. There is no hidden translation layer: what the method returns is what goes on the wire.

This package does not execute SQL, map results or change how SqlNative works. It is a typed, documented shorthand for the MySQL manual, and nothing else.

---

## Contents

- [Install](#install)
- [How it works](#how-it-works)
- [Quick start](#quick-start)
- [What the catalog covers](#what-the-catalog-covers)
- [Examples by category](#examples-by-category)
- [Literals and parameters](#literals-and-parameters)
- [Obsolete members](#obsolete-members)
- [Current status](#current-status)
- [Related packages](#related-packages)
- [License](#license)

---

## Install

```bash
dotnet add package SqlNative.MySqlFunc
```

Targets `net461`, `netstandard2.0`, `net6.0`, `net8.0`, `net9.0` and `net10.0`. It references `SqlNative`, so the core package comes with it. You still need an ADO.NET provider such as `MySqlConnector` or `MySql.Data` — the catalog references no provider.

The package id is `SqlNative.MySqlFunc`, but the namespace is `SqlNative`: one `using SqlNative;` covers the core types and the catalog. There is no `SqlNative.MySqlFunc` namespace to import.

---

## How it works

`MySqlFunctions` is a `public partial class` split into one file per category:

| File | Members |
| --- | --- |
| `MySqlFunctions.cs` | The `AS` helper and the `My` alias |
| `MySqlFunctions.Aggregate.cs` | `COUNT`, `SUM`, `AVG`, `GROUP_CONCAT`, `JSON_ARRAYAGG`, … |
| `MySqlFunctions.Bit.cs` | `BIT_COUNT` |
| `MySqlFunctions.Cast.cs` | `CAST`, `CONVERT`, `BINARY` |
| `MySqlFunctions.ControlFlow.cs` | `IF`, `IFNULL`, `NULLIF`, `ISNULL`, `COALESCE` |
| `MySqlFunctions.DateTime.cs` | `NOW`, `DATE_FORMAT`, `DATE_ADD`, `TIMESTAMPDIFF`, … |
| `MySqlFunctions.Encryption.cs` | `MD5`, `SHA2`, `AES_ENCRYPT`, `RANDOM_BYTES`, … |
| `MySqlFunctions.GTID.cs` | `GTID_SUBSET`, `WAIT_FOR_EXECUTED_GTID_SET`, … |
| `MySqlFunctions.Information.cs` | `DATABASE`, `USER`, `VERSION`, `LAST_INSERT_ID`, … |
| `MySqlFunctions.Ip.cs` | `INET_ATON`, `INET6_NTOA`, `IS_IPV4`, … |
| `MySqlFunctions.Json.cs` | `JSON_EXTRACT`, `JSON_SET`, `JSON_TABLE`, … |
| `MySqlFunctions.Locking.cs` | `GET_LOCK`, `RELEASE_LOCK`, `IS_FREE_LOCK`, … |
| `MySqlFunctions.Misc.cs` | `UUID`, `UUID_TO_BIN`, `ANY_VALUE`, `SLEEP`, … |
| `MySqlFunctions.Numeric.cs` | `ABS`, `ROUND`, `POW`, `RAND`, … |
| `MySqlFunctions.Operators.cs` | `OpAdd`, `OpEqEq`, `OpIn`, `OpAnd`, … |
| `MySqlFunctions.PerformanceSchema.cs` | `FORMAT_BYTES`, `PS_CURRENT_THREAD_ID`, … |
| `MySqlFunctions.Replication.cs` | `MASTER_POS_WAIT`, `SOURCE_POS_WAIT` |
| `MySqlFunctions.Spatial.cs` | `ST_GEOMFROMTEXT`, `ST_DISTANCE`, `ST_CONTAINS`, … |
| `MySqlFunctions.String.cs` | `SUBSTRING`, `CONCAT`, `REGEXP_REPLACE`, … |
| `MySqlFunctions.Window.cs` | `ROW_NUMBER`, `RANK`, `LAG`, `NTH_VALUE`, … |

Every method is `static`. Most return `string`; the eight IP helpers return `QueryPart`, and `ST_X` accepts one. The XML documentation on each member carries availability notes (which MySQL and MariaDB versions introduced the function) and a `SELECT` example, so IntelliSense tells you whether the function exists on your server before you run it.

### The `My` alias

`My` derives from `MySqlFunctions`, so its inherited static members are reachable through the shorter name. This follows the `Su` alias for `SqlUtils` in the core package:

```csharp
var sql1 = MySqlFunctions.UPPER(b.Title);
var sql2 = My.UPPER(b.Title);          // same call, less typing
```

`using static SqlNative.MySqlFunctions;` also works, but imports all 376 names at once and collides with everyday identifiers such as `LEFT`, `RIGHT`, `INSERT`, `REPLACE` and `CAST`. The `My.` prefix is usually the better trade.

### Return types

| Return type | Members | How to use them |
| --- | --- | --- |
| `string` | 368 | Interpolate into `Append($"…")` or assign to a local. |
| `QueryPart` | 8 | The eight helpers in `MySqlFunctions.Ip.cs`. Interpolate or compose with the `QueryPart` operators. |

`TableColumn` converts implicitly to `string` and to `QueryPart`, so columns can be passed straight into either kind of method. `QueryPart` does not convert implicitly to `string`; call `ToString()` or interpolate it if a `string` parameter needs an expression that is already a `QueryPart`.

---

## Quick start

**1. Describe the table.** The catalog does not replace the table model — it works alongside it.

```csharp
using SqlNative;

public partial class Books : MySqlTableStructure
{
    public override string TableName { get; } = "books";
    public override string QuotedTableName { get; } = "books";

    public TableColumn Id { get; }
    public TableColumn Title { get; }
    public TableColumn Author { get; }
    public TableColumn PublishedAt { get; }
    public TableColumn Price { get; }
    public TableColumn Metadata { get; }

    public Books()
    {
        Id          = new MySqlColumn("id", false, "int(11)", typeof(int), 10, this);
        Title       = new MySqlColumn("title", false, "varchar(200)", typeof(string), 200, this);
        Author      = new MySqlColumn("author", false, "varchar(150)", typeof(string), 150, this);
        PublishedAt = new MySqlColumn("published_at", false, "date", typeof(DateTime), 0, this);
        Price       = new MySqlColumn("price", false, "decimal(10,2)", typeof(decimal), 10, this);
        Metadata    = new MySqlColumn("metadata", false, "json", typeof(string), 0, this);
    }
}
```

**2. Wire it to a context.**

```csharp
public class BooksContext : SqlNativeContext
{
    public BooksContext(DbConnection connection) => Connection = connection;
}
```

```csharp
using var connection = new MySqlConnection(connectionString);
using var ctx = new BooksContext(connection) { OwnsConnection = true };
```

**3. Use the catalog inside a query.**

```csharp
var rows = ctx.BuildQuery<Books>((q, b) => q
    .Append($"SELECT {b.Id}, {My.UPPER(b.Title)} AS Title, {b.PublishedAt}, {b.Price}")
    .Append($"FROM {b}")
    .Append($"WHERE {My.YEAR(b.PublishedAt)} = @year")
    .SetParameters(new { year = 1998 }))
    .Execute<BooksRow>();
```

```sql
SELECT b.id, UPPER(b.title) AS Title, b.published_at, b.price
FROM books as b
WHERE YEAR(b.published_at) = @year
```

The catalog contributed `UPPER(b.title)` and `YEAR(b.published_at)`; everything else is plain SqlNative. `BooksRow` can be a POCO — the reflection mapper matches the aliases, and the extra table columns are simply not selected:

```csharp
public class BooksRow
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime PublishedAt { get; set; }
    public decimal Price { get; set; }
}
```

**4. Or just build an expression.**

```csharp
Console.WriteLine(My.CONCAT("'Hello'", "', '", "'World'"));
// CONCAT('Hello', ', ', 'World')

Console.WriteLine(My.DATE_ADD("'2024-01-15'", "INTERVAL 30 DAY"));
// DATE_ADD('2024-01-15', INTERVAL 30 DAY)
```

---

## What the catalog covers

| Category | Members | File | Representative members |
| --- | ---: | --- | --- |
| Aggregates | 18 | `MySqlFunctions.Aggregate.cs` | `COUNT`, `AVG`, `STDDEV_POP`, `GROUP_CONCAT`, `JSON_OBJECTAGG` |
| Bit | 1 | `MySqlFunctions.Bit.cs` | `BIT_COUNT` |
| Cast / convert | 3 | `MySqlFunctions.Cast.cs` | `CAST`, `CONVERT`, `BINARY` |
| Control flow | 5 | `MySqlFunctions.ControlFlow.cs` | `IF`, `IFNULL`, `NULLIF`, `COALESCE` |
| Date and time | 65 | `MySqlFunctions.DateTime.cs` | `NOW`, `DATE_FORMAT`, `DATE_ADD`, `TIMESTAMPDIFF`, `EXTRACT` |
| Encryption and hashing | 13 | `MySqlFunctions.Encryption.cs` | `MD5`, `SHA2`, `AES_ENCRYPT`, `RANDOM_BYTES`, `STATEMENT_DIGEST` |
| GTID | 4 | `MySqlFunctions.GTID.cs` | `GTID_SUBSET`, `GTID_SUBTRACT`, `WAIT_FOR_EXECUTED_GTID_SET` |
| Server information | 16 | `MySqlFunctions.Information.cs` | `DATABASE`, `USER`, `VERSION`, `LAST_INSERT_ID`, `ROW_COUNT` |
| IP addresses | 8 | `MySqlFunctions.Ip.cs` | `INET_ATON`, `INET6_ATON`, `IS_IPV4`, `IS_IPV6` |
| JSON | 34 | `MySqlFunctions.Json.cs` | `JSON_EXTRACT`, `JSON_SET`, `JSON_TABLE`, `MEMBER_OF` |
| Locking | 4 | `MySqlFunctions.Locking.cs` | `GET_LOCK`, `RELEASE_LOCK`, `IS_FREE_LOCK` |
| Miscellaneous | 12 | `MySqlFunctions.Misc.cs` | `UUID`, `UUID_TO_BIN`, `ANY_VALUE`, `SLEEP` |
| Numeric | 31 | `MySqlFunctions.Numeric.cs` | `ABS`, `ROUND`, `POW`, `RAND`, `CRC32` |
| Operators | 53 | `MySqlFunctions.Operators.cs` | `OpAdd`, `OpEqEq`, `OpLike`, `OpIn`, `OpAnd` |
| Performance Schema | 4 | `MySqlFunctions.PerformanceSchema.cs` | `FORMAT_BYTES`, `PS_CURRENT_THREAD_ID` |
| Replication | 2 | `MySqlFunctions.Replication.cs` | `MASTER_POS_WAIT`, `SOURCE_POS_WAIT` |
| Spatial | 36 | `MySqlFunctions.Spatial.cs` | `ST_GEOMFROMTEXT`, `ST_DISTANCE`, `ST_CONTAINS`, `ST_ASGEOJSON` |
| String | 55 | `MySqlFunctions.String.cs` | `SUBSTRING`, `CONCAT`, `REGEXP_REPLACE`, `LPAD` |
| Window | 11 | `MySqlFunctions.Window.cs` | `ROW_NUMBER`, `RANK`, `LAG`, `NTH_VALUE` |
| Helper | 1 | `MySqlFunctions.cs` | `AS` |
| **Total** | **376** | | |

The catalog is curated rather than exhaustive. If a function is missing, write it inline — the query builder accepts any SQL text.

---

## Examples by category

### Strings and numbers

```csharp
My.SUBSTRING(b.Title, "1", "20")               // SUBSTRING(b.title FROM 1 FOR 20)
My.CONCAT_WS("', '", b.Title, b.Author)        // CONCAT_WS(', ', b.title, b.author)
My.LPAD(b.Id, "6", "'0'")                      // LPAD(b.id, 6, '0')
My.ROUND(b.Price, "2")                         // ROUND(b.price, 2)
My.REGEXP_LIKE(b.Title, "@pattern")            // REGEXP_LIKE(b.title, @pattern)
```

### Dates and times

```csharp
My.NOW()                                       // NOW()
My.DATE_FORMAT(b.PublishedAt, "'%Y-%m'")       // DATE_FORMAT(b.published_at, '%Y-%m')
My.DATE_ADD(b.PublishedAt, "INTERVAL 30 DAY")  // DATE_ADD(b.published_at, INTERVAL 30 DAY)
My.TIMESTAMPDIFF("DAY", b.PublishedAt, My.NOW())
// TIMESTAMPDIFF(DAY, b.published_at, NOW())
```

The interval and unit arguments are SQL syntax, not values: pass `"INTERVAL 30 DAY"`, `"DAY"` or `"YEAR"` as text.

### JSON

```csharp
My.JSON_EXTRACT(b.Metadata, "'$.isbn'")
// JSON_EXTRACT(b.metadata, '$.isbn')

My.JSON_UNQUOTE(My.JSON_EXTRACT(b.Metadata, "'$.isbn'"))
// JSON_UNQUOTE(JSON_EXTRACT(b.metadata, '$.isbn'))

My.JSON_SET(b.Metadata, "'$.verified'", "TRUE", "'$.source'", "'import'")
// JSON_SET(b.metadata, '$.verified', TRUE, '$.source', 'import')

My.JSON_CONTAINS_PATH(b.Metadata, "'one'", "'$.isbn'", "'$.cover'")
// JSON_CONTAINS_PATH(b.metadata, 'one', '$.isbn', '$.cover')
```

Path arguments are string literals and must include their quotes. `JSON_CONTAINS_PATH` takes `'one'` or `'all'` for its second argument.

### Aggregates and window functions

```csharp
My.COUNT(b.Id)                                 // COUNT(b.id)
My.AVG(b.Price)                                // AVG(b.price)
My.GROUP_CONCAT(b.Title)                       // GROUP_CONCAT(b.title)
My.JSON_ARRAYAGG(b.Title)                      // JSON_ARRAYAGG(b.title)

My.ROW_NUMBER()                                // ROW_NUMBER() OVER ()
My.LAG(b.Price, "1", "0")                      // LAG(b.price, 1, 0) OVER ()
```

The window helpers emit a fixed, empty `OVER ()`. They save you the function syntax, but they cannot carry `PARTITION BY`, `ORDER BY` or a frame clause. When you need a real window, write the whole expression in the query:

```csharp
.Append($"SELECT ROW_NUMBER() OVER (PARTITION BY {b.Author} ORDER BY {b.Price} DESC) AS rn")
```

### Operators

SQL operators are exposed as `Op…` methods so they cannot collide with C# operators. Most results are parenthesised, so they compose safely:

```csharp
var rows = ctx.BuildQuery<Books>((q, b) =>
{
    var filter = My.OpAnd(
        My.OpGte(b.PublishedAt, "@fromDate"),
        My.OpLike(b.Title, "@pattern"));

    return q
        .Append($"SELECT {b.Id}, {b.Title}")
        .Append($"FROM {b}")
        .Append($"WHERE {filter}")
        .SetParameters(new { fromDate = new DateTime(1990, 1, 1), pattern = "%SQL%" });
})
.Execute<BooksRow>();
```

```sql
SELECT b.id, b.title
FROM books as b
WHERE ((b.published_at >= @fromDate) AND (b.title LIKE @pattern))
```

`OpAssign` emits `:=`, `OpDivInt` emits `DIV`, `OpNullSafeEq` emits `<=>`, and the bitwise group emits `&`, `|`, `^`, `~`, `<<` and `>>`.

### Server state, locks and replication

```csharp
My.DATABASE()                                  // DATABASE()
My.VERSION()                                   // VERSION()
My.LAST_INSERT_ID()                            // LAST_INSERT_ID()
My.GET_LOCK("'import:books'", "30")            // GET_LOCK('import:books', 30)
My.SOURCE_POS_WAIT("'source-bin.000001'", "4", "5")
// SOURCE_POS_WAIT('source-bin.000001', 4, 5)
```

Named locks are scoped to the connection that acquired them, so call `GET_LOCK` and `RELEASE_LOCK` on the same `SqlNativeContext` connection.

---

## Literals and parameters

Every argument is pasted into the SQL text. The catalog does **not** quote strings, escape values or create parameters. That is the one rule to keep in mind:

| You pass | You get | Correct? |
| --- | --- | --- |
| `"'Hello'"` | `'Hello'` | Yes — a quoted SQL string literal |
| `"Hello"` | `Hello` | No — parsed as a column or keyword |
| `b.Title` | `b.title` | Yes — a column reference |
| `"@name"` | `@name` | Yes — a parameter placeholder |
| user input | user input, verbatim | **No** — SQL injection |

For values, bind a parameter and let the provider escape it, even inside a function call:

```csharp
var term = Console.ReadLine() ?? "";

var rows = ctx.BuildQuery<Books>((q, b) => q
    .Append($"SELECT {b.Id}, {b.Title}")
    .Append($"FROM {b}")
    .Append($"WHERE {My.OpLike(b.Title, "@pattern")}")
    .SetParameters(new { pattern = $"%{term}%" }))
    .Execute<BooksRow>();
```

The value is bound by `SetParameters`, so it never becomes part of the SQL string.

If you must write a literal, include the quotes yourself and escape embedded quotes by doubling them. There is no `Quote` helper in this package.

---

## Obsolete members

Seven members are marked `[Obsolete]`, matching MySQL's own deprecations. Calling them produces a compiler warning:

| Member | Deprecated in | Replacement |
| --- | --- | --- |
| `BINARY` | MySQL 8.0.27 | `My.CAST(x, "BINARY")` |
| `OpBinary` | MySQL 8.0.27 | `My.CAST(x, "BINARY")` |
| `IS_IPV4_COMPAT` | MySQL 8.0.13 | — |
| `IS_IPV4_MAPPED` | MySQL 8.0.13 | — |
| `JSON_MERGE` | MySQL 8.0.16 | `My.JSON_MERGE_PRESERVE(...)` |
| `VALUES` | MySQL 8.0.20 | The `INSERT ... AS new` row alias |
| `MASTER_POS_WAIT` | MySQL 8.0.26 | `My.SOURCE_POS_WAIT(...)` |

`STD` and `STDDEV` also carry a deprecation note in their XML documentation — `STDDEV_POP` is the replacement — but they are not marked `[Obsolete]`.

---

## Current status

Worth knowing before you build on this:

- **The catalog is 376 members, curated rather than exhaustive.** It covers the functions used most often in application SQL. Anything else can be written inline; there is no runtime gate that rejects unknown functions.
- **It emits text, not parameters.** Strings, paths and intervals are your responsibility. See [Literals and parameters](#literals-and-parameters).
- **Window helpers emit a fixed `OVER ()`.** `ROW_NUMBER`, `RANK`, `LAG`, `LEAD`, `FIRST_VALUE`, `LAST_VALUE` and friends cannot accept a window specification. Write the full `OVER (...)` clause by hand when you need partitioning, ordering or a frame.
- **Most arguments are strings.** Numbers, types, units and interval clauses are passed as text (`"2"`, `"DATE"`, `"INTERVAL 7 DAY"`), not as typed values. A few methods (`IF`, `IFNULL`, `OpEqEq`, the IP helpers) accept `object` or `QueryPart` where that is useful.
- **Some members are obsolete and some are version-specific.** The XML docs record the MySQL and MariaDB versions; the catalog itself does not check the server version.
- **No execution, mapping or CRUD.** This package only builds function expressions. Query execution, result mapping, repositories and connection handling live in `SqlNative`.

---

## Related packages

| Package | Purpose |
| --- | --- |
| [`SqlNative`](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative/README.md) | The core ORM — query builder, table and column model, CRUD, result mapping. Referenced automatically. |
| `SqlNative.MsSqlFunc` / `PostgresFunc` | The same idea for SQL Server and PostgreSQL. |
| [`SqlNative.AspNetCore`](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative.AspNetCore/README.md) | DI registration and connection lifetime for ASP.NET Core. |
| [`SqlNative.ModelGenerator`](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative.ModelGenerator/README.md) | Generates complete data-access layers from a live schema. |

---

## License

MIT — see [LICENSE](https://github.com/peymanjahanbazi/SqlNative/blob/main/LICENSE).
