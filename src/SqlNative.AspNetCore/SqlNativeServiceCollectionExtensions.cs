using Microsoft.Extensions.DependencyInjection;
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

    public static IServiceCollection AddSqlNativeContext<T, Connection>(this IServiceCollection services, SqlNativeOptions options)
       where T : SqlNativeContext, new()
       where Connection : DbConnection, new()
    {
        var config = new SqlNativeContextConfig()
        {
            IsDevelopMode = options.IsDevelopMode,
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

    //public static IServiceCollection AddSqlNative(this IServiceCollection services)
    //{
    //    ArgumentNullException.ThrowIfNull(services);

    //    // Reserved for future registrations (IDbConnectionFactory, DefaultContext, etc.).
    //    // Kept intentionally empty for now so consumers can start using the package
    //    // without a breaking change once concrete registrations are introduced.
    //    return services;
    //}
}