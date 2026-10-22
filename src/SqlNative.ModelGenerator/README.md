# SqlNative.ModelGenerator

Code generator for [SqlNative](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative/README.md) — the light-weight ORM that keeps your SQL visible. Point it at a live database and it writes a complete, strongly-typed data-access layer: a table class, a row class, a CRUD entity, and a context, for every table in the schema.

Supports **SQL Server**, **MySQL**, **PostgreSQL** and **Oracle**. The generated code targets the core `SqlNative` package and compiles against any of its four dialect base classes.

---

## Contents

- [Install](#install)
- [Quick start](#quick-start)
- [CLI reference](#cli-reference)
- [What gets written](#what-gets-written)
- [Naming rules](#naming-rules)
- [The three classes generated per table](#the-three-classes-generated-per-table)
  - [1. The table structure](#1-the-table-structure)
  - [2. The row model](#2-the-row-model)
  - [3. The CRUD entity](#3-the-crud-entity)
- [The context class](#the-context-class)
- [Dialect differences](#dialect-differences)
- [Known issues](#known-issues)
- [Related packages](#related-packages)
- [License](#license)

---

## Install

Packed as a .NET tool named `sqlnative-gen`:

```bash
dotnet tool install -g SqlNative.ModelGenerator
```

Or run it without installing:

```bash
dotnet run --project path/to/SqlNative.ModelGenerator -- --help
```

The tool itself targets `net10.0` and carries all four ADO.NET providers, so it can connect to any of the supported engines without extra packages on your side.

---

## Quick start

Four options, all required: `--cs`, `--engine`, `--folder`, `--package`. `--context` is optional.

### MySQL

```bash
sqlnative-gen \
  --cs "Server=localhost;Port=3306;Database=shop;User ID=root;Password=p@ssw0rd;" \
  --engine mysql \
  --folder "./Models" \
  --package "Shop.Models" \
  --context ShopContext
```

Writes `Shop.Models.Countries`, `Shop.Models.CountriesRow`, `Shop.Models.CountriesEntity` and `Shop.Models.ShopContext`. The table class extends `MySqlTableStructure`; the row maps `Id` as `long` (from `bigint(20)`).

### SQL Server

```bash
sqlnative-gen \
  --cs "Server=localhost;Database=shop;User Id=sa;Password=p@ssw0rd;TrustServerCertificate=True" \
  --engine mssql \
  --folder "./Models" \
  --package "Shop.Models" \
  --context ShopContext
```

Writes `Shop.Models.DboOrders` (schema `dbo` + table `Orders`), `Shop.Models.DboOrdersRow`, `Shop.Models.DboOrdersEntity` and `Shop.Models.ShopContext`. The table class extends `MsSqlTableStructure`, carries a `Schema` property, and the row reload uses `OUTPUT INSERTED.*`.

### PostgreSQL

```bash
sqlnative-gen \
  --cs "Host=localhost;Database=shop;Username=postgres;Password=p@ssw0rd" \
  --engine postgresql \
  --folder "./Models" \
  --package "Shop.Models" \
  --context ShopContext
```

Writes `Shop.Models.PublicOrders` (schema `public` + table `Orders`), and the rest of the same four shapes. The table class extends `PostgresTableStructure`, quoted identifiers are `"Orders"`, and the row reload uses `RETURNING *`.

### Oracle

```bash
sqlnative-gen \
  --cs "User Id=scott;Password=tiger;Data Source=oracle-host:1521/ORCLPDB1" \
  --engine oracle \
  --folder "./Models" \
  --package "Shop.Models" \
  --context Sch1Context
```

Writes `Shop.Models.Orders` — Oracle tables are **not** schema-prefixed in the class name — plus `OrdersRow`, `OrdersEntity` and `Sch1Context`. The table class extends `OracleTableStructure`. Oracle is the one dialect that uses `:p1`-style parameter names instead of `@p1`, and it reads only the tables owned by the connected user.

---

## CLI reference

```
sqlnative-gen --cs <connectionString> --engine <mysql|mssql|postgresql|oracle>
              --folder <path> --package <namespace> [--context <contextName>]
```

| Option | Required | Aliases | Meaning |
| --- | --- | --- | --- |
| `--cs` | yes | `--connectionstring`, `--conn` | Connection string to the database. |
| `--engine` | yes | `--database`, `--db`, `--pg`, `--postgres`, `--ora`, `--oradb` | `mysql`, `mssql`, `postgresql` or `oracle`. |
| `--folder` | yes | `--out`, `--output` | Output directory. Created if missing. |
| `--package` | yes | `--namespace`, `--ns` | Root namespace for every generated class. |
| `--context` | no | `--ctx` | Name of the context class. Defaults to `<DatabaseName>Context`. |
| `--help` | no | `--h`, `--?` | Print usage. |

Both `--key value` and `--key=value` syntaxes work. Every option must start with `--`. Engine names are case-insensitive, and `pg`/`postgres`, `sqlserver`/`sql-server` and `ora`/`oradb` normalise to `postgresql`, `mssql` and `oracle`.

Exit codes: `0` success, `1` bad arguments or generation failure, `2` unsupported engine.

### What each engine reads

| Engine | Reads from | Scope |
| --- | --- | --- |
| `mssql` | `sys.tables` + `sys.columns` | User tables where `is_ms_shipped = 0`, all schemas. |
| `mysql` | `SHOW TABLES` | The connected database only. |
| `postgresql` | `information_schema.tables` | All `BASE TABLE`s, excluding `pg_catalog` and `information_schema`. |
| `oracle` | `all_tables` | Tables owned by the connected user; skips nested and IOT-overflow tables. |

All four read column nullability, primary-key membership and auto-increment, and use that to decide the CLR types and whether the generated row property is nullable.

---

## What gets written

For a database with N tables you get N + 1 files:

```
Models/
  Countries.cs        -> Countries, CountriesRow, CountriesEntity
  Orders.cs           -> Orders, OrdersRow, OrdersEntity
  ShopContext.cs      -> ShopContext
```

All three classes for a table live in one file, named after the table class. The generator overwrites existing files without prompting.

---

## Naming rules

| | Rule | Example |
| --- | --- | --- |
| SQL Server table class | `<Schema>_<Table>`, sanitised to PascalCase | `dbo.Emp` → `DboEmp` |
| PostgreSQL table class | `<Schema>_<Table>`, sanitised to PascalCase | `public.Emp` → `PublicEmp` |
| MySQL table class | `<Table>`, sanitised to PascalCase | `countries` → `Countries` |
| Oracle table class | `<Table>`, sanitised to PascalCase | `NEWTABLE` → `Newtable` |
| Row class | `<TableClass>Row` | `CountriesRow` |
| Entity class | `<TableClass>Entity` | `CountriesEntity` |
| Context class | `--context`, else `<DatabaseName>Context` | `fpprod` → `FpprodContext` |

Names are made unique with a numeric suffix if two tables sanitise to the same class name. Generation **fails** if the context name collides with a table class name — pass a different `--context`.

---

## The three classes generated per table

Every table produces three classes in a single file. Take a MySQL table `countries` with `Id bigint`, `Name varchar(100)`, `ISO varchar(2)`, `EU tinyint(1)` and a nullable `CreateUserId bigint`.

### 1. The table structure

The `Countries` class is the SQL-side description of the table. It extends the dialect's `TableStructure` base and holds one `TableColumn` per column, so `FROM` and column references become C# symbols.

```csharp
public partial class Countries : MySqlTableStructure, HasPrimaryKey
{
    public override string TableName { get; } = "countries";
    protected override string QuotedTableName { get; } = "countries";

    public TableColumn Id { get; private set; }
    public TableColumn Name { get; private set; }
    public TableColumn Iso { get; private set; }
    public TableColumn Createuserid { get; private set; }

    public override TableColumn? PKey => Id;

    public Countries()
    {
        Id = new MySqlColumn("Id", false, "bigint(20)", typeof(long), this);
        Name = new MySqlColumn("Name", false, "varchar(100)", typeof(string), this);
        Iso = new MySqlColumn("ISO", false, "varchar(2)", typeof(string), this);
        Createuserid = new MySqlColumn("CreateUserId", false, "bigint(20)", typeof(long), this);
    }
}
```

**Members:**

| Member | Purpose |
| --- | --- |
| `TableName` | The bare table name. Used to derive auto-aliases (`countries` → `c`). |
| `QuotedTableName` | The schema-qualified, quoted name used in `FROM`. |
| `Schema` | Present on the SQL Server, PostgreSQL and Oracle base classes; omitted on MySQL, which has no schemas. |
| `Star` | Inherited. A column representing `*`, for `SELECT {t.Star}`. |
| `PKey` | The primary key column, or `null` if the table has none. |
| `HasPrimaryKey` | Implemented only when the table actually has a primary key — it is what unlocks `DeleteById` / `DeleteByIds`. |

**The constructor matters.** It calls the base constructor once per column, passing the *real* database column name (so `ISO` and `CreateUserId` keep their casing), the *raw* SQL type as read from the catalog (`bigint(20)`, `varchar(2)`), the mapped CLR type, and `this`. The C# property name is normalised to PascalCase (`CreateUserid` for `CreateUserId`) because the identifier helper strips characters it can't use.

The third constructor argument — the `needsBrackets` / `needsBackticks` / `needsDoubleQuotes` flag — is emitted per dialect, not per column style. SQL Server emits `false` here and `true` for identifiers that actually need it; MySQL emits `false`; PostgreSQL and Oracle emit `true`. See [Dialect differences](#dialect-differences).

**Column names differ from property names.** `CreateUserId` → property `Createuserid`. Use the property in C#, and the SQL still emits `CreateUserId`.

### 2. The row model

`CountriesRow` extends `TableRow<long>` — one CLR-typed property per column, plus the ordinal-based reader that populates it.

```csharp
public partial class CountriesRow : TableRow<long>
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Iso { get; set; } = null!;
    public long? Createuserid { get; set; }

    public override long Key => Id;

    public override int[] GetOrders(DbDataReader reader) => new[]
    {
        reader.GetOrdinal("Id"),
        reader.GetOrdinal("Name"),
        reader.GetOrdinal("ISO"),
        reader.GetOrdinal("CreateUserId")
    };

    public override void SetFields(DbDataReader reader, int[] orders)
    {
        if (!reader.IsDBNull(orders[0])) Id = reader.GetInt64(orders[0]);
        if (!reader.IsDBNull(orders[1])) Name = reader.GetString(orders[1]);
        if (!reader.IsDBNull(orders[2])) Iso = reader.GetString(orders[2]);
        if (!reader.IsDBNull(orders[3])) Createuserid = reader.GetInt64(orders[3]);
    }
}
```

**Members:**

| Member | Purpose |
| --- | --- |
| `TableRow<TPk>` base | Binds the row to its primary key CLR type — here `long`, `decimal` for an Oracle `NUMBER`. |
| `Key` | The primary key value. `DbEntity.Store` uses this to decide insert vs. update, and `DeleteById` uses it as the `WHERE` value. |
| Properties | One per column, named after the table class's columns. **Nullable columns get a nullable CLR type** (`long? Createuserid`); non-null ones get the value type (`long Id`) with `= null!` on reference types. |
| `GetOrders` | Called **once per result set**, not per row, and resolves each column name to its ordinal — so the per-row cost is index lookups, not name comparisons. |
| `SetFields` | Called per row. Every `IsDBNull` check maps one-to-one onto the CLR type chosen above, which is why the generator emits `GetInt64` for a `bigint` and `GetDecimal` for an Oracle `NUMBER`. |

There is no `SetFields` guard for a null non-nullable property — a non-nullable column that is nevertheless `NULL` in the row leaves the property at its default rather than throwing.

### 3. The CRUD entity

`CountriesEntity` extends `DbEntity<CountriesRow, Countries, long>` and is the only class that issues writes. It takes the context in its constructor and holds it for the lifetime of the request.

```csharp
public partial class CountriesEntity : DbEntity<CountriesRow, Countries, long>
{
    private readonly FpprodContext ctx;

    public CountriesEntity(FpprodContext ctx) => this.ctx = ctx;
    // ...
}
```

**Generated overrides:**

| Method | What it does |
| --- | --- |
| `Select(Func<Countries, string> predicate, object parameters)` | `SELECT {e.Star} FROM {e}`, plus a `WHERE` when the predicate returns a non-empty string. Calls `.Execute<CountriesRow>()`. |
| `Insert(row, InsertOptions)` | Builds an `InsertModel`, runs it, and — when asked — re-reads the row and copies every field back onto the object you passed in. |
| `Insert(List<row>, InsertOptions)` | One multi-row `INSERT` with `@p1, @p2, …` parameters. With `LoadRecord` it falls back to looping single inserts, because the reload is per-row. |
| `Update(row, bool reload)` | `SET` every non-key column, `WHERE {e.Id} = @Key`. With `reload`, re-reads by key and copies the result back. |
| `Update(List<row>, bool reload)` | Chunks the list and delegates to `UpdateBatch`. With `reload`, re-reads all keys in one `IN (…)` query and matches results back by key. |
| `Delete(row)` | `ctx.DeleteById<Countries, long>(row.Key)`. |
| `Delete(List<row>)` | `ctx.DeleteByIds<Countries, long>(rows.Select(r => r.Key))`. |
| `UpdateBatch(rows, offset, count)` | Private helper holding the dialect's set-based update SQL for one chunk. |

**Inherited from `DbEntity<Row, Structure, Pk>`** — these are not generated, you get them from the core package:

| Member | What it does |
| --- | --- |
| `All()` | `Select(t => "")` — the whole table. |
| `Store(row)` | Insert if `Key` is null or default, otherwise update. Returns the affected count. |
| `Store(List<row>)` | Splits the list on `Key` and does both, summing the results. |

**`InsertOptions` controls what `Insert` does:**

| Value | Flag combination | Behaviour |
| --- | --- | --- |
| `SkipPrimaryKey` (default) | `2` | Omits the key from the `INSERT` column list and lets the database assign it. |
| `LoadRecord` | `1` | Includes the key **and** re-reads the row afterwards. |
| `LoadAndSkip` | `3` | Omits the key **and** re-reads the row afterwards. |

**The reload is dialect-specific**, and this is the part to know when reading generated code:

| Engine | Reload clause |
| --- | --- |
| `mssql` | `OUTPUT INSERTED.*` on the `INSERT`/`UPDATE` itself. |
| `postgresql` | `RETURNING *` on the `INSERT`/`UPDATE` itself. |
| `oracle` | `RETURNING *` on the `INSERT`/`UPDATE` itself. |
| `mysql` | A second `SELECT … WHERE {t.Id} = LAST_INSERT_ID()`. |

MySQL is the outlier because it has no `RETURNING` equivalent — so a MySQL `Insert(..., LoadRecord)` is **two** round trips, not one.

**Batching.** `Update(List<row>)` reads `ctx.Config.MaxParametersPerStatement` (default `60_000`) and divides by the number of parameters per row to get a chunk size, so a 50,000-row update never exceeds the provider's parameter limit:

```csharp
var parametersPerStatement = ctx.Config?.MaxParametersPerStatement ?? 60_000;
if (parametersPerStatement <= 0) parametersPerStatement = 60_000;
var batchSize = Math.Max(1, parametersPerStatement / 11);   // 11 = columns + 1
```

The generated code is verbose by design. There is no reflection and no runtime expression parser in the hot path — every `GetOrdinal`, every type conversion and every parameter name is written out, so the JIT has nothing to work out.

---

## The context class

One context is generated for the whole database, in its own file named after it.

```csharp
public partial class FpprodContext : SqlNativeContext
{
    private CountriesEntity? _Countries;

    public CountriesEntity Countries => _Countries ??= new(this);

    private OrdersEntity? _Orders;

    public OrdersEntity Orders => _Orders ??= new(this);
}
```

**Members:**

| Member | Purpose |
| --- | --- |
| `SqlNativeContext` base | Carries the `DbConnection`, the `Config`, and the `Select`/`Insert`/`Update`/`Delete` builders. |
| `_X` field | Backing field, lazily initialised. |
| `X` property | `??= new(this)` — one entity instance per context, created on first access. Handlers that never touch a table never allocate its entity. |
| `this` | Passes the context into the entity, which is how every write reaches the database. |

The property name is the table class name, so it lines up with the generated class you can `using` at the top of the file. If a table class happens to share the context's name, the property gets a `Table` suffix.

Entities are constructed once per context, and the context is scoped per request — so **the context is not thread-safe**, and one context belongs to one request. For ASP.NET Core, register it with [`SqlNative.AspNetCore`](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative.AspNetCore/README.md):

```csharp
builder.Services.AddSqlNativeContext<FpprodContext, MySqlConnection>(new()
{
    ConnectionString = configuration.GetConnectionString("Default")!
});
```

The context must have a **public parameterless constructor** — the DI container creates it and assigns `Connection` afterwards. Generated contexts comply.

Use it:

```csharp
var countries = ctx.Countries.All();                                   // every row
var german = ctx.Countries.Select(c => $"{c.Iso} = @iso", new { iso = "DE" });

ctx.Countries.Insert(new CountriesRow { Name = "Germany", Iso = "DE" });
ctx.Countries.Store(row);                                              // insert or update
ctx.Countries.Update(row, reload: true);                               // writes, then re-reads
ctx.Countries.Delete(row);
```

Everything the context exposes as a convenience is also available directly, which is where you go for anything the generated CRUD doesn't cover:

```csharp
var rows = ctx.Select<Countries, Users>((c, u) => new SelectModel
{
    From = c,
    Select = $"{c.Name}, {u.Login}",
    Joins = [new LeftJoin { Table = u, On = $"{c.Changeuserid} = {u.Id}" }],
    Where = $"{c.Iso} = @iso",
    Parameters = new { iso = "DE" },
    Limit = 100
}).Execute<CountryWithUser>();
```

---

## Dialect differences

| | SQL Server | MySQL | PostgreSQL | Oracle |
| --- | --- | --- | --- | --- |
| `--engine` | `mssql` | `mysql` | `postgresql` | `oracle` |
| Provider | `Microsoft.Data.SqlClient` | `MySqlConnector` | `Npgsql` | `Oracle.ManagedDataAccess.Core` |
| Table base | `MsSqlTableStructure` | `MySqlTableStructure` | `PostgresTableStructure` | `OracleTableStructure` |
| Column class | `MsSqlColumn` | `MySqlColumn` | `PostgresColumn` | `OracleColumn` |
| `Schema` property | Yes | No | Yes | Yes |
| Schema in class name | Yes (`DboEmp`) | n/a | Yes (`PublicEmp`) | No (`Newtable`) |
| Quoting | `[Name]` | `` `Name` `` | `"Name"` | `"Name"` |
| Parameter prefix | `@p1` | `@p1` | `@p1` | `:p1` |
| Single-row reload | `OUTPUT INSERTED.*` | re-`SELECT` on `LAST_INSERT_ID()` | `RETURNING *` | `RETURNING *` |
| `bigint` maps to | `long` | `long` | `long` | `decimal` (`NUMBER`) |

`MySqlTableStructure` has no `Schema` property because MySQL has no schemas — a table belongs to the connected database. Oracle's `NUMBER` maps to `decimal` rather than `long`, because Oracle's `NUMBER` is exact and unconstrained; a `long` mapping would silently truncate large values.

---

## Known issues

Three defects in the generator as it stands. All are in generated output, not in the core package, and all are worth fixing before you rely on generated code.

**1. The generated context class does not compile.** All four context writers emit

```csharp
public partial class ShopContext : DefaultContext
```

but no `DefaultContext` type exists anywhere in the solution — the base class is `SqlNativeContext`. Every freshly generated context fails with `CS0246: The type or namespace name 'DefaultContext' could not be found`. The checked-in samples in `SqlNative.ConsoleTest` derive from `SqlNativeContext`, so they predate this or were corrected by hand. **One-word fix in four files.**

**2. Batch update emits MySQL syntax for three of four engines.** The `Update(List<row>)` path generates

```sql
UPDATE <table> JOIN (<source>) t2 ON <table>.Id = t2.Id SET <table>.Col = t2.Col
```

`UPDATE … JOIN` is MySQL-only. SQL Server wants `UPDATE t SET … FROM t JOIN (…), PostgreSQL wants `UPDATE t SET … FROM (…) t2 WHERE …`, and Oracle wants `MERGE INTO t USING (…) t2 ON … WHEN MATCHED THEN UPDATE SET …`. Only the MySQL writer's `INNER JOIN (VALUES ROW(…), …)` is valid, and that form needs MySQL 8.0.19 or later.

**3. MySQL batch insert needs MySQL 8.0.19+.** Same reason — the `VALUES ROW(…)` derived-table form. On older servers, use the `Update(List<row>)` path or issue the inserts yourself.

---

## Related packages

| Package | Purpose |
| --- | --- |
| `SqlNative` | The core ORM the generated code targets — query builder, table and column model, CRUD primitives, result mapping. Referenced automatically. |
| `SqlNative.Core` | Identifier helpers and CLR type mappers, one per engine. The generator calls into these, so the two packages stay in step on naming and type mapping. |
| `SqlNative.AspNetCore` | Registers the generated context in DI: `AddSqlNativeContext<TContext, TConnection>(...)`. |
| `SqlNative.MsSqlFunc` / `MySqlFunc` / `PostgresFunc` | Optional catalogs of each dialect's SQL functions, as `QueryPart` factories. |

## License

MIT — see [LICENSE](https://github.com/peymanjahanbazi/SqlNative/blob/main/LICENSE).