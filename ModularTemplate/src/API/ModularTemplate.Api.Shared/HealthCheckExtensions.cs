using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ModularTemplate.Api.Shared;

/// <summary>
/// Extension methods for health check configuration.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds health check services for database and cache connectivity.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseConnectionString">The PostgreSQL database connection string.</param>
    /// <param name="cacheConnectionString">The Redis cache connection string (optional).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        string databaseConnectionString,
        string? cacheConnectionString = null)
    {
        var healthChecks = services.AddHealthChecks()
            .AddNpgSql(databaseConnectionString, name: "database");

        if (!string.IsNullOrEmpty(cacheConnectionString))
        {
            healthChecks.AddRedis(cacheConnectionString, name: "cache");
        }

        return services;
    }

    /// <summary>
    /// Maps the health check endpoint with UI response writer.
    /// Explicitly allows anonymous access for health probes.
    /// </summary>
    public static IEndpointRouteBuilder MapHealthCheckEndpoint(
        this IEndpointRouteBuilder app,
        string pattern = "/health")
    {
        app.MapHealthChecks(pattern, new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }).AllowAnonymous();

        return app;
    }
}
