# SqlNative.Core

Dialect knowledge for [SqlNative](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative/README.md) — the light-weight ORM that keeps your SQL visible. This package holds the two pieces of per-database logic that everything else depends on: how an identifier becomes a legal C# name in that database's dialect, and how a SQL type becomes a CLR type.

It has **no dependencies and no ADO.NET providers**. It is pure string and regex work.

**You probably don't need to install this.** `SqlNative` and `SqlNative.ModelGenerator` both reference it, so it arrives transitively. Reach for it directly only if you're writing your own schema reader or your own code generator and want the same naming and type-mapping rules the rest of the toolkit uses.

---

## Contents

- [Install](#install)
- [What is in here](#what-is-in-here)
- [Identifier helpers](#identifier-helpers)
- [CLR type mappers](#clr-type-mappers)
- [DatabaseEngine](#databaseengine)
- [Who uses this](#who-uses-this)
- [Notes and quirks](#notes-and-quirks)
- [Related packages](#related-packages)
- [License](#license)

---

## Install

```bash
dotnet add package SqlNative.Core
```

Targets `net461`, `netstandard2.0`, `net6.0`, `net8.0`, `net9.0` and `net10.0`.

---

## What is in here

| Type | Per engine | Purpose |
| --- | --- | --- |
| `MsSqlIdentifierHelper`, `MySqlIdentifierHelper`, `PostgresIdentifierHelper`, `OracleIdentifierHelper` | 4 static classes | Turn database names into C# identifiers, and decide when a SQL identifier needs quoting. |
| `MsSqlClrTypeMapper`, `ClrTypeMapper`, `PostgresClrTypeMapper`, `OracleClrTypeMapper` | 4 static classes | Map a raw SQL type string to a CLR type, apply nullability, and emit the `DbDataReader` call that reads it. |
| `DatabaseEngine` | 1 enum | Selects which of the above to use. |

Every mapper and helper is a `static class` with no state, so all of it is safe to call from anywhere, concurrently, at any point.

---

## Identifier helpers

One per engine. The API is identical across all four except for the name of the "does this need quoting?" method:

| Class | Quoting method | Quote character |
| --- | --- | --- |
| `MsSqlIdentifierHelper` | `NeedsBrackets` | `[` `]` |
| `MySqlIdentifierHelper` | `NeedsBackticks` | `` ` `` |
| `PostgresIdentifierHelper` | `NeedsDoubleQuotes` | `"` |
| `OracleIdentifierHelper` | `NeedsDoubleQuotes` | `"` |

### `GetObjectNameInCs(string originalname)`

The one that matters most. It splits the identifier on every non-alphanumeric run and capitalises each part. A part that already mixes upper and lower case keeps its own word boundaries; a part that is entirely upper or entirely lower case is title cased:

```csharp
MsSqlIdentifierHelper.GetObjectNameInCs("CreateUserId");  // "CreateUserId"
MsSqlIdentifierHelper.GetObjectNameInCs("CountryCode");   // "CountryCode"
MsSqlIdentifierHelper.GetObjectNameInCs("ISO3");          // "Iso3"
MsSqlIdentifierHelper.GetObjectNameInCs("dbo_Emp");       // "DboEmp"
MsSqlIdentifierHelper.GetObjectNameInCs("public.Emp");    // "PublicEmp"
MySqlIdentifierHelper.GetObjectNameInCs("countries");     // "Countries"
OracleIdentifierHelper.GetObjectNameInCs("NEWTABLE");     // "Newtable"
```

Two edge cases:

- An identifier that starts with a digit gets an `_` prefix — `_2fa`, not `2fa`. (`MsSqlIdentifierHelper` also prefixes when the result starts with `_`.)
- An empty or fully-punctuation identifier becomes `"Item"`.

Note the casing: a word is left alone when it mixes upper and lower case, because it already marks its own boundaries — `CountryCode` stays `CountryCode` and `SurfaceArea` stays `SurfaceArea`. A word with no internal capitalisation is title cased, so `ISO` becomes `Iso` and `first_name` becomes `FirstName`. The C# property name therefore usually matches the SQL column name, but not always: the generated code always emits the original column name separately.

### `WrapObjectName(string objectName)`

Wraps the identifier in the dialect's quote characters **only when it needs them**, and returns it bare otherwise:

```csharp
MsSqlIdentifierHelper.WrapObjectName("Order");     // "[Order]"   — reserved word
MsSqlIdentifierHelper.WrapObjectName("Createdate");// "Createdate"
MySqlIdentifierHelper.WrapObjectName("Key");       // "`Key`"      — reserved word
PostgresIdentifierHelper.WrapObjectName("select");// "\"select\""
```

`MsSqlIdentifierHelper` escapes an embedded `]` by doubling it (`a]b` → `[a]]b]`). The other three don't need to — backticks and double quotes aren't valid inside those identifiers.

### `NeedsBrackets` / `NeedsBackticks` / `NeedsDoubleQuotes`

The predicate `WrapObjectName` uses. An identifier needs quoting when any of these is true:

1. It is null or empty.
2. Its first character is not a letter or `_` (SQL Server also allows `@`; MySQL allows `$` anywhere).
3. It contains a character outside letters, digits, `_` (plus `$` and `#` on SQL Server, `$` on MySQL).
4. It is a **reserved word** in that dialect — the helper carries the full list, ~240 for T-SQL, ~300 for MySQL, and the equivalent sets for PostgreSQL and Oracle.

That last rule is the interesting one: `NeedsBrackets("Order")` is `true`, so SQL Server columns named `Order` get bracketed automatically. You never have to remember the reserved-word list.

### `EnsureUnique(string candidate, ISet<string> usedNames)`

Returns `candidate` if it adds cleanly, otherwise `candidate2`, `candidate3`, and so on. Pass it a `HashSet<string>` to accumulate names:

```csharp
var used = new HashSet<string>(StringComparer.Ordinal);
used.Add("Emp");

MsSqlIdentifierHelper.EnsureUnique("Emp", used);   // "Emp2"
MsSqlIdentifierHelper.EnsureUnique("Dept", used); // "Dept"
```

The comparison set is the caller's, so you control case sensitivity — the generator passes `StringComparer.Ordinal` for class names.

### `EscapeCsStringLiteral` / `EscapeCsStringLiteralNoQuote`

Escape a string for emission into generated C# source: backslashes, double quotes, newlines, tabs, and any other control character (as `\uXXXX`). The first wraps the result in quotes and renders `null` for a null input; the second leaves the wrapping to you and returns `null` for null.

Both write C#-style escapes, not JSON-style — note the difference from what you might expect.

---

## CLR type mappers

One per engine, with the same three methods.

### `MapToClrType(string columnRawType)`

Takes the raw type as read from the catalog — including any length — and returns a CLR type name as a string:

```csharp
MsSqlClrTypeMapper.MapToClrType("bigint");        // "long"
PostgresClrTypeMapper.MapToClrType("int8");      // "long"
PostgresClrTypeMapper.MapToClrType("text[]");     // "string[]"
OracleClrTypeMapper.MapToClrType("NUMBER");      // "decimal"
OracleClrTypeMapper.MapToClrType("NUMBER(5)");   // "int"
ClrTypeMapper.MapToClrType("tinyint(1)");        // "bool"   — this is MySQL's mapper
ClrTypeMapper.MapToClrType("varchar(100)");      // "string"
```

**Anything unrecognised falls back to `string`.** That's a deliberate choice — a wrong `string` still reads the row correctly and fails loudly at the call site, whereas a wrong numeric type silently truncates.

The mapping highlights that differ per engine:

| CLR | SQL Server | MySQL | PostgreSQL | Oracle |
| --- | --- | --- | --- | --- |
| `long` | `bigint` | `bigint` | `bigint`, `int8`, `bigserial` | `NUMBER(p≤18)` |
| `int` | `int`, `integer` | `int`, `mediumint`, `integer` | `int`, `int4`, `integer`, `serial` | `NUMBER(p≤9)` |
| `short` | `smallint` | `smallint` | `smallint`, `int2`, `smallserial` | `NUMBER(p≤4)` |
| `byte` | `tinyint` | `tinyint` unsigned | — | — |
| `bool` | `bit` | `tinyint(1)`, `bit(1)` | `boolean`, `bool` | `NUMBER(1)` |
| `decimal` | `decimal`, `numeric`, `money`, `smallmoney` | `decimal`, `numeric`, `dec` | `numeric`, `decimal`, `money` | `NUMBER` (no precision), `BINARY_FLOAT`, `BINARY_DOUBLE` |
| `double` | `float` | `double`, `precision` | `float8`, `double precision` | — |
| `float` | `real` | `float`, `real` | `real`, `float4` | — |
| `DateTime` | `date`, `datetime`, `datetime2`, `smalldatetime`, `datetimeoffset` | `date`, `datetime`, `timestamp` | `date`, `timestamp`, `timestamptz` | `date`, `timestamp` |
| `TimeSpan` | `time` | `time` | `time`, `timetz` | `interval` |
| `Guid` | `uniqueidentifier` | — | `uuid` | — |
| `byte[]` | `binary`, `varbinary`, `timestamp`, `rowversion` | `binary`, `varbinary`, `*blob` | `bytea` | `raw`, `long_raw`, `blob`, `bfile` |
| `string` | `nvarchar`, `varchar`, `nchar`, `char`, `ntext`, `text`, `xml` | `varchar`, `char`, `*text`, `enum`, `set`, `json`, `geometry` family | `text`, `varchar`, `json`, `jsonb`, `citext`, `inet`, `cidr`, `macaddr` | `varchar2`, `nvarchar2`, `char`, `clob`, `nclob`, `long`, `xmltype` |
| `T[]` | — | — | any type with a `[]` suffix | — |

Three of these deserve a note:

- **MySQL unsigned integers map to unsigned CLR types.** `bigint unsigned` → `ulong`, `int unsigned` → `uint`. On every other engine the signed type is used.
- **Oracle's `NUMBER` is resolved by precision**, not by name — `NUMBER(1)` → `bool`, `NUMBER(5)` → `int`, `NUMBER(20)` → `decimal`. `NUMBER` with no precision falls back to `decimal`, because Oracle's `NUMBER` is exact and unconstrained and a `long` would silently truncate. This is why the generated Oracle `Id` is a `decimal` and not a `long`.
- **PostgreSQL is the only engine that produces arrays.** A `[]` suffix on the raw type produces `string[]`, `int[]` and so on, and is the reason its mapper has the extra reader cases.

### `ApplyNullability(string clrType, bool isNullable)`

Appends `?` when the column is nullable and the type is a value type or a reference type:

```csharp
MsSqlClrTypeMapper.ApplyNullability("long", true);    // "long?"
MsSqlClrTypeMapper.ApplyNullability("long", false);   // "long"
MsSqlClrTypeMapper.ApplyNullability("string", true);  // "string?"
```

`IsReferenceType` covers `string`, `byte[]` and `object`. The PostgreSQL and Oracle mappers additionally recognise array types, so `string[]` becomes `string[]?`.

### `GetReaderExpression(string clrType, string ordinal)`

Returns the `DbDataReader` call that reads that type — the expression the generated `SetFields` body is built from:

```csharp
MsSqlClrTypeMapper.GetReaderExpression("long", "orders[0]");    // "reader.GetInt64(orders[0])"
MsSqlClrTypeMapper.GetReaderExpression("Guid", "orders[3]");    // "reader.GetGuid(orders[3])"
MsSqlClrTypeMapper.GetReaderExpression("byte[]", "orders[4]");  // "(byte[])reader.GetValue(orders[4])"
```

A nullable `?` is trimmed first, so pass the type straight from `ApplyNullability`. Anything not in the switch falls back to `({type})reader.GetValue({ordinal})`.

Typed `GetInt64`/`GetBoolean`/`GetString` calls rather than `GetValue` plus a cast is the point of the whole exercise — the provider returns the exact type, so there's no boxing and no conversion.

---

## DatabaseEngine

```csharp
public enum DatabaseEngine
{
    MySql,
    Postgres,
    MsSql,
    Oracle,
}
```

There is no registry or lookup behind it — it's a plain discriminator. The core package uses it to pick an identifier helper at runtime; the generator uses its own `EngineFactory` and normalises string names (`pg`, `postgres`, `sqlserver`, `sql-server`, `ora`, `oradb`) before reaching any of this.

---

## Who uses this

Two consumers inside the toolkit:

**`Query.Model<T>()`** in the core package. When you run a query and ask it to write a `QueryResult` class from the result set, it picks an identifier helper from the `DatabaseEngine` to decide property names, and uses it to escape column names into `[SqlNativeColumn]` attributes and `GetOrdinal` calls. It also handles repeated column names in a join by emitting `QueryResult.OrdinalN(reader, "Name", 1)` for the second and subsequent occurrences.

**`SqlNative.ModelGenerator`** calls all four mappers to type every column of a live schema, and all four helpers to name every generated class. Keeping the rules in one shared package is what stops the generated output from drifting away from the runtime naming.

---

## Notes and quirks

**The MySQL type mapper is called `ClrTypeMapper`, not `MySqlClrTypeMapper`.** It lives in `ClrTypeMapper.cs` and is used by the MySQL writer in the generator. Every other engine's mapper is prefixed with its name. If you write `MySqlClrTypeMapper.MapToClrType(...)` it won't compile; the class you want is `SqlNative.Core.ClrTypeMapper`.

**Two generators read reader expressions through the MySQL mapper.** `Postgres/ModelWriter.cs` and `Oracle/ModelWriter.cs` call `ClrTypeMapper.IsReferenceType` and `ClrTypeMapper.GetReaderExpression` rather than their own engine's equivalents. This is harmless today — `IsReferenceType` is identical in all four, and PostgreSQL's extra array cases produce the same string the shared `default` branch would anyway — but it means changing the MySQL mapper's reader table would silently change Oracle and PostgreSQL output too. Both are one-line changes to point at the right class.

**`Query.Model<T>()` cannot be told which engine you're on.** The underlying generator takes a `DatabaseEngine` and defaults it to `MySql`, but `Query.Model<T>()` on the public `Query` class exposes no way to pass one, so the runtime generator always uses MySQL's rules. The practical difference is small — the `GetObjectNameInCs` implementations are nearly identical, and MySQL's `NeedsBackticks` is a subset of SQL Server's `NeedsBrackets` — but a leading-underscore identifier would be prefixed differently.

**`MsSqlClrTypeMapper.TextTypes` lists `uniqueidentifier` even though it's handled as `Guid`.** The switch runs before the set lookup, so `Guid` wins. The set entry is dead but harmless.

---

## Related packages

| Package | Purpose |
| --- | --- |
| `SqlNative` | The core ORM. References this package. |
| `SqlNative.ModelGenerator` | Schema-to-source code generator. References this package. |
| `SqlNative.AspNetCore` | DI registration for the core context. |
| `SqlNative.MsSqlFunc` / `MySqlFunc` / `PostgresFunc` | Optional catalogs of each dialect's SQL functions, as `QueryPart` factories. |

## License

MIT — see [LICENSE](https://github.com/peymanjahanbazi/SqlNative/blob/main/LICENSE).