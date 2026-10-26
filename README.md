# SqlNative

Type-safe, low-level SQL toolkit for .NET — write real SQL with C# identifiers for tables, columns and functions (no typos), simple CRUD, and full access to every RDBMS feature. Unlike Entity Framework, it never hides the database.

You describe a table once as a C# class, and from then on `City.CountryCode` is a symbol the compiler checks. The SQL you write is the SQL that runs — window functions, CTEs, `OUTPUT`, query hints, `MERGE`, vendor extensions all stay available because nothing sits between you and the wire.

```csharp
var rows = ctx.Select<City>(c => new SelectModel
{
    From   = c,
    Select = $"{c.Name}, {c.Population}",
    Where  = $"{c.CountryCode} = @cc",
    OrderBy = $"{c.Population} DESC",
    Limit  = 5,
    Parameters = new { cc = "USA" }
}).Execute<CityRow>();
```

```sql
SELECT c.Name, c.Population
FROM city as c
WHERE c.CountryCode = @cc
ORDER BY c.Population DESC
LIMIT 5
```

One codebase for **SQL Server, MySQL, PostgreSQL and Oracle** — SqlNative has no provider dependency, you supply the `DbConnection` and the table classes decide the dialect.

---

## Contents

- [Install](#install)
- [The example: the MySQL `world` database](#the-example-the-mysql-world-database)
- [1. Generate the model](#1-generate-the-model)
- [2. Your first query](#2-your-first-query)
- [3. Load a table](#3-load-a-table)
- [4. Insert](#4-insert)
- [5. Update](#5-update)
- [6. Delete](#6-delete)
- [7. Store — insert or update](#7-store--insert-or-update)
- [8. Advanced querying](#8-advanced-querying)
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

## The example: the MySQL `world` database

Every snippet below runs against the classic MySQL sample database, `world`:

```sql
CREATE TABLE `city` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` char(35) NOT NULL DEFAULT '',
  `CountryCode` char(3) NOT NULL DEFAULT '',
  `District` char(20) NOT NULL DEFAULT '',
  `Population` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`),
  KEY `CountryCode` (`CountryCode`),
  CONSTRAINT `city_ibfk_1` FOREIGN KEY (`CountryCode`) REFERENCES `country` (`Code`)
);
```

```sql
CREATE TABLE `country` (
  `Code` char(3) NOT NULL DEFAULT '',
  `Name` char(52) NOT NULL DEFAULT '',
  `Continent` enum('Asia','Europe','North America','Africa','Oceania','Antarctica','South America') NOT NULL DEFAULT 'Asia',
  `SurfaceArea` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Population` int NOT NULL DEFAULT '0',
  `LifeExpectancy` decimal(3,1) DEFAULT NULL,
  ...
  PRIMARY KEY (`Code`)
);
```

The complete generated output for this database is checked in at [`src/SqlNative.ConsoleTest/Data`](src/SqlNative.ConsoleTest/Data) — [`City.cs`](src/SqlNative.ConsoleTest/Data/City.cs), [`Country.cs`](src/SqlNative.ConsoleTest/Data/Country.cs), [`Countrylanguage.cs`](src/SqlNative.ConsoleTest/Data/Countrylanguage.cs) and [`WorldContext.cs`](src/SqlNative.ConsoleTest/Data/WorldContext.cs). Everything in this README is drawn from those files.

```bash
docker run -d --name worlddb -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=world mysql:8 \
  -v /path/to/world.sql:/docker-entrypoint-initdb.d/world.sql
```

Connection string used throughout:

```csharp
string connStr = "Server=localhost;Database=world;User=root;Password=root;";
```

---

## 1. Generate the model

Point the generator at a live database and it writes the whole data-access layer for you:

```bash
dotnet tool install -g SqlNative.ModelGenerator

sqlnative-gen \
  --cs "Server=localhost;Database=world;User=root;Password=root;" \
  --engine mysql \
  --folder "./Models" \
  --package "World.Models" \
  --context WorldContext
```

```
Database 'world': generated 3 table model(s) and context 'WorldContext'.
  City.cs
  Country.cs
  Countrylanguage.cs
  WorldContext.cs
```

Each table file contains three classes. For `city`:

**`City`** — the table structure. One `TableColumn` per column, so every column reference in your SQL is a compile-checked symbol:

```csharp
public partial class City : MySqlTableStructure, HasPrimaryKey
{
    public override string TableName { get; } = "city";
    protected override string QuotedTableName { get; } = "city";
    public TableColumn Id { get; private set; }
    public TableColumn Name { get; private set; }
    public TableColumn CountryCode { get; private set; }
    public TableColumn District { get; private set; }
    public TableColumn Population { get; private set; }

    public override TableColumn? PKey => Id;

    public City()
    {
        Id = new MySqlColumn("ID", false, "int(11)", typeof(int), this);
        Name = new MySqlColumn("Name", false, "char(35)", typeof(string), this);
        CountryCode = new MySqlColumn("CountryCode", false, "char(3)", typeof(string), this);
        District = new MySqlColumn("District", false, "char(20)", typeof(string), this);
        Population = new MySqlColumn("Population", false, "int(11)", typeof(int), this);
    }
}
```

The constructor passes the **real** database column name (`"ID"`, `"CountryCode"`) — so the SQL keeps the database's own casing — plus the raw SQL type and the CLR type it maps to. The C# property name is the column name in PascalCase.

**`CityRow`** — the CLR-typed row, with ordinals resolved once per result set:

```csharp
public partial class CityRow : TableRow<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public string District { get; set; } = null!;
    public int Population { get; set; }
    public override int Key => Id;

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("ID"),
        reader.GetOrdinal("Name"),
        reader.GetOrdinal("CountryCode"),
        reader.GetOrdinal("District"),
        reader.GetOrdinal("Population")
    };

    public override void SetFields(DbDataReader reader, int[] orders)
    {
        if (!reader.IsDBNull(orders[0])) Id = reader.GetInt32(orders[0]);
        if (!reader.IsDBNull(orders[1])) Name = reader.GetString(orders[1]);
        if (!reader.IsDBNull(orders[2])) CountryCode = reader.GetString(orders[2]);
        if (!reader.IsDBNull(orders[3])) District = reader.GetString(orders[3]);
        if (!reader.IsDBNull(orders[4])) Population = reader.GetInt32(orders[4]);
    }
}
```

**`CityEntity`** — the CRUD operations for that table. You never write these; the generator does.

**`WorldContext`** — the context, exposing one entity per table:

```csharp
public partial class WorldContext : SqlNativeContext
{
    private CityEntity? _City;
    public CityEntity City => _City ??= new(this);

    private CountryEntity? _Country;
    public CountryEntity Country => _Country ??= new(this);

    private CountrylanguageEntity? _Countrylanguage;
    public CountrylanguageEntity Countrylanguage => _Countrylanguage ??= new(this);
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
using World.Models;

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

Now write SQL. `Select` takes the tables and returns a `Query` — a builder that has **not** run yet:

```csharp
var query = ctx.Select<City>(c => new SelectModel
{
    From   = c,
    Select = $"{c.Name}, {c.Population}",
    Where  = $"{c.CountryCode} = @cc",
    OrderBy = $"{c.Population} DESC",
    Limit  = 5,
    Parameters = new { cc = "USA" }
});

Console.WriteLine(query.Sql);
// SELECT c.Name, c.Population
// FROM city as c
// WHERE c.CountryCode = @cc
// ORDER BY c.Population DESC
// LIMIT 5

var rows = query.Execute<CityRow>();
```

Because the query is an object, you can inspect the SQL before it hits the database, or extend it:

```csharp
query.Append("AND c.District = @district");
var rows = query.Execute<CityRow>();
```

`TableStructure.ToString()` renders as `city as c` — the alias is assigned automatically and kept unique across the query. You never set it by hand, and you never write the table name as a string.

---

## 3. Load a table

The generated entity gives you the common per-table reads:

```csharp
// every row
var all = ctx.City.All();

// filtered
var german = ctx.City.Select(
    c => $"{c.CountryCode} = @cc",
    new { cc = "DEU" });

// the predicate can use any column
var big = ctx.City.Select(c => $"{c.Population} > @min", new { min = 1_000_000 });
```

`ctx.City.Select(...)` runs `SELECT c.* FROM city as c WHERE ...` and maps straight into `CityRow`. The row type is `TableRow<int>`, so `row.Key` is the primary key.

Rows can also be plain POCOs — SqlNative matches column names to property names case-insensitively, ignoring non-alphanumeric characters, so `CountryCode` binds to `CountryCode` and `first_name` would bind to `FirstName`:

```csharp
public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public int Population { get; set; }
}

var dtos = ctx.Select<City>(c => new SelectModel { From = c, Select = $"{c.Star}" })
              .Execute<CityDto>();
```

Override the match with `[SqlNativeColumn]` when the SQL alias differs from the property name:

```csharp
[SqlNativeColumn("CountryName")]
public string Country { get; set; } = null!;
```

---

## 4. Insert

```csharp
var city = new CityRow
{
    Name = "Springfield",
    CountryCode = "USA",
    District = "Illinois",
    Population = 116000
};

ctx.City.Insert(city, InsertOptions.LoadAndSkip);
// city.Id now holds the generated key
```

`LoadAndSkip` omits the primary key from the INSERT and reads the row back with `LAST_INSERT_ID()`, so the object you passed in comes back populated. The other options are `SkipPrimaryKey` (the default) and `LoadRecord`.

The same insert without the entity — still type-checked against the table:

```csharp
var affected = ctx.Insert<City>(c => new InsertModel
{
    Columns = $"{c.Name}, {c.CountryCode}, {c.District}, {c.Population}",
    Values  = "@Name, @CountryCode, @District, @Population",
    Parameters = new { Name = "Springfield", CountryCode = "USA", District = "Illinois", Population = 116000 }
}).ExecuteNonQuery();
```

```sql
INSERT INTO city
(Name, CountryCode, District, Population)
VALUES ( @Name, @CountryCode, @District, @Population )
```

Insert many rows at once — the generated entity batches them into a single multi-row `VALUES` statement:

```csharp
var rows = new List<CityRow> { city1, city2, city3 };
ctx.City.Insert(rows);
```

---

## 5. Update

```csharp
city.Population = 120000;
ctx.City.Update(city, reload: true);
```

`reload: true` re-reads the row afterwards, so database-side defaults and triggers land on your object. The generated `UPDATE` sets every non-key column and matches on the primary key:

```sql
UPDATE city
SET Name = @Name, CountryCode = @CountryCode, District = @District, Population = @Population
WHERE ID = @Key
```

Update a list and the generator batches it with a `VALUES`-join, chunked by `MaxParametersPerStatement`:

```csharp
ctx.City.Update(rows, reload: true);
```

The raw form works the same way:

```csharp
ctx.Update<City>(c => new UpdateModel
{
    Set        = $"{c.Population} = @Population",
    Where      = $"{c.Id} = @Id",
    Parameters = new { Population = 120000, Id = 3793 }
}).ExecuteNonQuery();
```

---

## 6. Delete

```csharp
ctx.City.Delete(city);            // by key, through the entity
ctx.City.Delete(rows);            // a list — batched
```

Or straight off the context, for statements that don't need a row object:

```csharp
ctx.DeleteById<City, int>(3793);
ctx.DeleteByIds<City, int>(new[] { 3793, 3794, 3795 });   // batched
ctx.DeleteWhere<City>(c => $"{c.CountryCode} = @cc", new { cc = "USA" });
ctx.DeleteAll<City>();
```

```sql
DELETE FROM city
WHERE CountryCode = @cc
```

---

## 7. Store — insert or update

`Store` looks at the key: a default key means insert, a populated key means update.

```csharp
var city = new CityRow { Name = "Springfield", CountryCode = "USA", District = "Illinois", Population = 116000 };
ctx.City.Store(city);      // inserted, city.Id filled in

city.Population = 120000;
ctx.City.Store(city);      // updated

ctx.City.Store(rows);      // splits the list and does both
```

This is the method to reach for when the same code path writes both new and existing rows.

---

## 8. Advanced querying

### Joins

Add a table to the generic list and get an alias for it too:

```csharp
var rows = ctx.Select<City, Country>((c, co) => new SelectModel
{
    From   = c,
    Select = $"{c.Name}, {co.Name} AS CountryName, {co.Continent}",
    Joins  = [new InnerJoin { Table = co, On = $"{c.CountryCode} = {co.Code}" }],
    Where  = $"{co.Continent} = @continent",
    OrderBy = $"{c.Name}",
    Parameters = new { continent = "Europe" }
}).Execute<CityWithCountry>();
```

```sql
SELECT c.Name, o.Name AS CountryName, o.Continent
FROM city as c
INNER JOIN country as o ON c.CountryCode = o.Code
WHERE o.Continent = @continent
ORDER BY c.Name
```

Join types are `LeftJoin`, `RightJoin`, `InnerJoin`, `FullJoin` and `CrossJoin`. Up to fifteen tables are supported by `Select` and `BuildQuery`. `CityWithCountry` is the row class for the result set — see [Generate a row class for an ad-hoc query](#generate-a-row-class-for-an-ad-hoc-query) below, or a plain POCO.

### Group by and having

```csharp
var stats = ctx.Select<Country>(c => new SelectModel
{
    From    = c,
    Select  = $"{c.Continent}, COUNT(*) AS CountryCount",
    GroupBy = $"{c.Continent}",
    Having  = "COUNT(*) > 5",
    OrderBy = $"{c.Continent}"
}).Execute<ContinentStat>();
```

```sql
SELECT c.Continent, COUNT(*) AS CountryCount
FROM country as c
GROUP BY c.Continent
HAVING COUNT(*) > 5
ORDER BY c.Continent
```

### Raw SQL, with the model still available

`BuildQuery` is the escape hatch. You write the SQL; the table classes still supply the aliases, the quoting and the parameters:

```csharp
var rows = ctx.BuildQuery<City>((q, c) => q
    .Append($"WITH big AS (SELECT {c.Name}, {c.Population} FROM {c} WHERE {c.Population} > 1000000)")
    .Append($"SELECT {c.Name}, {c.Population} FROM big")
    .Append($"ORDER BY {c.Population} DESC"))
    .Execute<CityRow>();
```

```sql
WITH big AS (SELECT c.Name, c.Population FROM city as c WHERE c.Population > 1000000)
SELECT c.Name, c.Population FROM big
ORDER BY c.Population DESC
```

Nothing stops you using window functions, `MERGE`, recursive CTEs or a hint — this is your SQL.

### `IN` lists

`AppendFor` repeats a fragment once per item, with an optional separator. Bind the values as parameters rather than interpolating them into the text:

```csharp
var codes = new[] { "USA", "DEU", "FRA" };
var parameters = new Dictionary<string, object>();
var placeholders = new List<string>();

for (var i = 0; i < codes.Length; i++)
{
    parameters[$"@cc{i}"] = codes[i];
    placeholders.Add($"@cc{i}");
}

var rows = ctx.BuildQuery<City>((q, c) => q
    .Append($"SELECT {c.Star} FROM {c}")
    .Append($"WHERE {c.CountryCode} IN ({string.Join(", ", placeholders)})")
    .SetParameters(parameters))
    .Execute<CityRow>();
```

### Generate a row class for an ad-hoc query

Run the query and let SqlNative write the matching row class for the result set:

```csharp
ctx.Select<City, Country>((c, co) => new SelectModel
{
    From   = c,
    Select = $"{c.Name}, {co.Name} AS CountryName}",
    Joins  = [new LeftJoin { Table = co, On = $"{c.CountryCode} = {co.Code}" }]
})
.Model<Program>("CityWithCountry")
.Execute<CityWithCountry>();
```

The file is written next to the calling source file as `CityWithCountry.cs`. This is the quickest way to get a correct row type for an awkward query, and it is how the checked-in examples in `SqlNative.ConsoleTest` were produced. It only runs when `Config.IsDevelopMode` is on.

---

## API reference

### The query builder

`Select`, `Insert` and `Update` return a `Query`. You finish it and execute it yourself:

```csharp
var q = ctx.Select<City>(c => new SelectModel { From = c, Select = $"{c.Star}" });
q.Append("ORDER BY Population DESC");
Console.WriteLine(q.Sql);
var rows = q.Execute<CityRow>();
```

| Method | Purpose |
| --- | --- |
| `Append(string)` | Append a line of SQL. Blank lines are ignored. |
| `AppendIf(bool, string)` | Append only when the condition holds. |
| `AppendFor<T>(items, func)` | Append once per item. |
| `AppendFor<T>(join, items, func)` | Append once per item with a separator — useful for `IN` lists. |
| `SetParameters(object)` | Bind parameters, from a dictionary or an anonymous object. |
| `AddParameters(object)` | Add parameters to the ones already set. |
| `Execute<T>()` | Run and map to `List<T>`. |
| `ExecuteScalar()` | Run and return the first column of the first row. |
| `ExecuteNonQuery()` | Run and return the affected row count. |
| `Config(Action)` | Change the table structure before the SQL is built. |
| `Model<T>(className)` | Write a row class for this result set. |
| `Sql` | The generated SQL, without executing it. |

### How parameters are handled

`Query.CreateCommand` treats each parameter differently, and the distinction matters when you read a query plan or a log:

| Parameter value | What happens |
| --- | --- |
| `null` | Replaced in the SQL with the literal `NULL`. |
| `byte`, `short`, `int`, `long`, `decimal`, `double`, `float`, `bool` | Inlined as a literal, formatted with the invariant culture. |
| Anything else | Bound as a real `DbParameter` on the command. |

Only strings, dates, GUIDs, arrays and `DBNull` actually reach the database as parameters. The `@` prefix is optional — `@cc` and `cc` both bind to `@cc`.

### Mapping results

Two modes. Pick per query, not per project.

**Reflection (POCO)** — a parameterless constructor and public settable properties; column names match property names case-insensitively, ignoring non-alphanumeric characters. `DBNull` columns are skipped rather than throwing, and values are converted with `Convert.ChangeType` when the provider returns a different type.

**Explicit ordinals (`QueryResult`)** — derive from `QueryResult` to take control. Resolve the ordinals once per result set, then read them by index — no per-row name lookup, and duplicate column names stop being a problem:

```csharp
public partial class CityRow : TableRow<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public override int Key => Id;

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("ID"),
        reader.GetOrdinal("Name")
    };

    public override void SetFields(DbDataReader r, int[] orders)
    {
        if (!r.IsDBNull(orders[0])) Id = r.GetInt32(orders[0]);
        if (!r.IsDBNull(orders[1])) Name = r.GetString(orders[1]);
    }
}
```

`TableRow<TPk>` is a `QueryResult` that also exposes `Key`, which is what `DbEntity` uses to decide insert vs. update. Inside a `QueryResult` subclass, `OrdinalN(reader, "Name", 1)` resolves the second occurrence of a repeated column name in a join.

### Configuration and lifecycle

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
| `IsDevelopMode` | `true` on a bare context | Enables model generation and SQL logging. |
| `LogSqlCommands` | `true` on a bare context | Writes executed commands to the logger or the console. |
| `MaxPacketSize` | `1_000_000` | Development flag. |

`SqlNativeContext` implements `IDisposable` and `IAsyncDisposable`. It only closes the connection when `OwnsConnection` is `true`, so you can hand it a connection borrowed from an EF `DbContext` or a unit-of-work and leave the lifetime with whoever created it.

### Logging

Set `Logger` on the context to route the executed SQL through `Microsoft.Extensions.Logging` instead of the console:

```csharp
ctx.Logger = loggerFactory.CreateLogger("SqlNative");
ctx.BuildQuery(q => q.Append("SELECT * FROM city")).ExecuteNonQuery();
// log: Information: SQL command: SELECT * FROM city
```

The command is written at `Information` level with the SQL as the `Sql` property. What gets written is the command text with parameters already substituted — numeric and boolean parameters are inlined as literals, everything else stays a `@parameter` placeholder. Treat the log as sensitive if your inline values are.

---

## What SqlNative is not

| | |
| --- | --- |
| Tables and columns as typed classes | Yes |
| Row → POCO mapping | Yes — reflection, or explicit ordinals for full control |
| `Select` / `Insert` / `Update` / `Delete` | Yes — each returns a `Query` you can inspect before it runs |
| Per-table CRUD (`ctx.City.Insert(row)`) | Yes — via `DbEntity<T>` |
| Code generation from a live schema | Yes |
| Change tracking | No — `Store(row)` decides insert vs. update by looking at the key |
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

The core `SqlFunctions` class currently exposes `SqlFunctions.NULL`; the per-engine catalogs add the rest of the dialect's built-in functions.

> `Limit` and `Offset` are emitted verbatim as `LIMIT` / `OFFSET`, which is MySQL and PostgreSQL syntax but not SQL Server's (`OFFSET … ROWS FETCH NEXT … ROWS ONLY`). Use `Append` if you need the other spelling.

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
