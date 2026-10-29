# SqlNative.AspNetCore

ASP.NET Core integration for [SqlNative](https://github.com/peymanjahanbazi/SqlNative/blob/main/src/SqlNative/README.md) — the light-weight ORM that keeps your SQL visible. This package is the DI glue: it registers your `SqlNativeContext` in the host's service collection so controllers and minimal API endpoints can take it as a constructor parameter, and it manages the lifetime of the underlying `DbConnection`.

It does not change how you query. Everything else — the query builder, table and column classes, result mapping, CRUD — lives in the core package.

---

## Contents

- [Install](#install)
- [Quick start](#quick-start)
- [What the registration does](#what-the-registration-does)
- [Options](#options)
- [Minimal APIs](#minimal-apis)
- [Reusing a connection you don't own](#reusing-a-connection-you-dont-own)
- [Current status](#current-status)
- [Related packages](#related-packages)
- [License](#license)

---

## Install

```bash
dotnet add package SqlNative.AspNetCore
```

Targets `net6.0`, `net8.0`, `net9.0` and `net10.0`, and requires the `Microsoft.AspNetCore.App` shared framework. You also need an ADO.NET provider (`Microsoft.Data.SqlClient`, `MySqlConnector`, `Npgsql`, …) — neither this package nor the core one references any.

---

## Quick start

**1. Derive a context.** It needs a public parameterless constructor, because the DI container creates it.

```csharp
using SqlNative;

public class FpprodContext : SqlNativeContext
{
    private CountriesEntity countries;
    public CountriesEntity Countries => countries ??= new(this);
}
```

**2. Register it.**

```csharp
using SqlNative.AspNetCore;

builder.Services.AddSqlNativeContext<FpprodContext, MySqlConnection>(new()
{
    ConnectionString = configuration.GetConnectionString("Default")!,
    IsDevelopMode = builder.Environment.IsDevelopment()
});
```

**3. Inject it.**

```csharp
[ApiController]
[Route("[controller]")]
public class HelloController(FpprodContext ctx) : ControllerBase
{
    [HttpGet("[action]")]
    public IActionResult CountryCount()
    {
        return Ok(new { CountryCount = ctx.Countries.All().Count });
    }

    [HttpGet("[action]")]
    public IActionResult Countries()
    {
        return Ok(ctx.BuildQuery<Countries>((q, c) => q
            .Append($"SELECT {c.Id}, {c.Name}, {c.Iso3}")
            .Append($"FROM {c}"))
            .Execute<Countrybrief>());
    }
}
```

The generic parameters are your context type and your provider's connection type. `AddSqlNativeContext<TContext, TConnection>` requires `TConnection : DbConnection, new()` — a public parameterless constructor, which every ADO.NET connection type has.

---

## What the registration does

`AddSqlNativeContext` registers `TContext` as **scoped**, and its factory:

1. Instantiates `TConnection` and sets its `ConnectionString`.
2. **Opens the connection immediately.**
3. Constructs `TContext` and assigns `Connection` and `Config` (built from the options you passed).
4. Sets `OwnsConnection = true`.
5. Assigns the host's `ILogger` to `TContext.Logger`, so executed SQL commands go to the app's logging pipeline instead of the console. The category is `SqlNative`, so the host can turn the commands on or off by level:

```json
"Logging": { "LogLevel": { "SqlNative": "Information" } }
```

Because the context is scoped and owns its connection, the container disposes it at the end of the request, and disposal closes and disposes the connection. You don't open, close, or dispose anything yourself.

Two consequences worth knowing:

- **The connection is opened at resolve time, not lazily.** Injecting a context into a request that never queries still opens a connection.
- **A failure to open throws during dependency injection**, not at the point of the first query. A bad connection string or an unreachable server surfaces as a DI resolution exception on the request that first asked for the context.

If your context constructor throws, the factory closes and disposes the connection it already opened before rethrowing — a half-built context never leaks an open socket.

---

## Options

```csharp
builder.Services.AddSqlNativeContext<FpprodContext, MySqlConnection>(new()
{
    ConnectionString = "Server=localhost;Database=fpprod;...",
    IsDevelopMode = true,
    MaxParametersPerStatement = 60_000,
    MaxPacketSize = 1_000_000
});
```

| Option | Default | Effect |
| --- | --- | --- |
| `ConnectionString` | `""` | Assigned to the connection before it is opened. Required. |
| `IsDevelopMode` | `false` | Forwarded to `SqlNativeContextConfig`. |
| `LogSqlCommands` | `false` | Forwarded to `SqlNativeContextConfig`; writes every executed SQL command to the host's `ILogger`. `IsDevelopMode` turns it on too. |
| `MaxParametersPerStatement` | `60_000` | Forwarded to `SqlNativeContextConfig`; batch size for `DeleteByIds` and generated entities. |
| `MaxPacketSize` | `1_000_000` | Forwarded to `SqlNativeContextConfig`. |

Note that `IsDevelopMode` defaults to `false` here, while a `SqlNativeContext` constructed by hand defaults to `true`. If you want consistent behaviour across an app, set it explicitly.

---

## Minimal APIs

The context is an ordinary scoped service, so minimal API endpoints take it as a parameter. `Execute<T>()` is synchronous — read from the database on the request thread, and return the mapped result:

```csharp
app.MapGet("/countries", (FpprodContext ctx) =>
{
    return ctx.BuildQuery<Countries>((q, c) => q
        .Append($"SELECT {c.Id}, {c.Name}, {c.Iso3}")
        .Append($"FROM {c}"))
        .Execute<Countrybrief>();
});

app.MapGet("/countries/{id}", (int id, FpprodContext ctx) =>
    ctx.BuildQuery<Countries>((q, c) => q
        .Append($"SELECT {c.Id}, {c.Name}, {c.Iso3}")
        .Append($"FROM {c}")
        .Append($"WHERE {c.Id} = @id")
        .SetParameters(new { id }))
    .Execute<Countrybrief>()
    .FirstOrDefault());
```

On a web app, prefer the async overloads — they use the provider's real async I/O, so the
request thread is released while the database works:

```csharp
app.MapGet("/countries", async (FpprodContext ctx, CancellationToken ct) =>
{
    return await ctx.BuildQuery<Countries>((q, c) => q
        .Append($"SELECT {c.Id}, {c.Name}, {c.Iso3}")
        .Append($"FROM {c}"))
        .ExecuteAsync<Countrybrief>(ct);
});
```

---

## Reusing a connection you don't own

`AddSqlNativeContext` always creates and owns its connection. If you would rather hand SqlNative a connection you already have — one borrowed from an EF Core `DbContext`, or one shared with the rest of your app — the core context supports that directly. Leave `OwnsConnection` at its default of `false` and nobody will close it:

```csharp
app.MapGet("/countries", (AppDbContext ef, FpprodContext sql) =>
{
    var connection = ef.Database.GetDbConnection();
    connection.Open();
    sql.Connection = connection;      // OwnsConnection stays false — EF disposes it
    return sql.Countries.All();
});
```

Doing this by hand in every endpoint is unpleasant, and a registration helper for it does not exist in this package yet — the sample project carries one you can copy. See `AddNoStringDbContext` in `SqlNative.ConsoleTest` / `WebApiTest`.

---

## Current status

This package is deliberately small. Not implemented yet:

- **Connection pooling.** `ConnectionCreatePolicy` (`ConnectionPerContext` / `ConnectionPool`) is declared but `internal`, and the corresponding properties are commented out in `SqlNativeOptions`. Every scoped context opens its own connection; pooling is left to the provider.
- **A bare `AddSqlNative()`** for when you only want the core services — the method is written but commented out.
- **Health checks, interceptors and logging hooks.** Executed SQL commands are written to the host's `ILogger`; health checks and command interceptors are not wired up.

---

## Related packages

| Package | Purpose |
| --- | --- |
| `SqlNative` | The core ORM — query builder, table and column model, CRUD, result mapping. Referenced automatically. |
| `SqlNative.Core` | Identifier helpers, CLR type mappers, `DatabaseEngine`. Transitive. |
| `SqlNative.MsSqlFunc` / `MySqlFunc` / `PostgresFunc` | Optional catalogs of each dialect's SQL functions, as `QueryPart` factories. |
| `SqlNative.ModelGenerator` | Full code generator, for producing complete data-access layers from a live schema. |

---

## License

MIT — see [LICENSE](https://github.com/peymanjahanbazi/SqlNative/blob/main/LICENSE).