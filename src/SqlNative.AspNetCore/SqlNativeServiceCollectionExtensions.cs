using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace SqlNative.AspNetCore;

/// <summary>
/// ASP.NET Core integration for SqlNative.
/// </summary>
public static class SqlNativeServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core SqlNative services required by the host's dependency-injection
    /// container. Call from <c>Program.cs</c>:
    /// <code>
    /// builder.Services.AddSqlNative();
    /// </code>
    /// </summary>
    /// <param name="services">The host's service collection.</param>
    /// <param name="options">Parameter to instanciate SqlNativeContext.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    /// <typeparam name="T">The SqlNative context type registered in the dependency-injection container.</typeparam>
    /// <typeparam name="Connection">The ADO.NET connection type created for each scope.</typeparam>
    public static IServiceCollection AddSqlNativeContext<T, Connection>(this IServiceCollection services, SqlNativeOptions options)
       where T : SqlNativeContext, new()
       where Connection : DbConnection, new()
    {
        var config = new SqlNativeContextConfig()
        {
            IsDevelopMode = options.IsDevelopMode,
            LogSqlCommands = options.LogSqlCommands,
            MaxParametersPerStatement = options.MaxParametersPerStatement,
            MaxPacketSize = options.MaxPacketSize
        };
        services.AddScoped<T>((IServiceProvider s) =>
        {
            Connection? connection = new()
            {
                ConnectionString = options.ConnectionString
            };
            connection.Open();

            try
            {
                var ctx = new T()
                {
                    Connection = connection,
                    Config = config
                };
                ctx.OwnsConnection = true;
                // The host's logger, so executed commands go to the app's logging
                // pipeline instead of the console. Null when no logger factory is registered.
                // One stable category, so a host can filter it with "LogLevel": { "SqlNative": "Information" }.
                ctx.Logger = s.GetService<ILoggerFactory>()?.CreateLogger("SqlNative");
                return ctx;
            }
            catch
            {
                // new T() (or generated subclass ctor) threw after Open().
                // Close + dispose the open connection and rethrow.
                try { connection.Close(); } catch { }
                connection.Dispose();
                throw;
            }
        });
        return services;
    }
}