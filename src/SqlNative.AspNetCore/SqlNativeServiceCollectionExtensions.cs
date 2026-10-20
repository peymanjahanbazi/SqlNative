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
        services.AddScoped<T>((IServiceProvider s) =>
        {
            Connection? connection = new()
            {
                ConnectionString = options.ConnectionString
            };
            connection.Open();
            var ctx = new T() { Connection = connection };
            return ctx;
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