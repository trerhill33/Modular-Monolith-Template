using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for health check configuration.
/// </summary>
internal static class HealthCheckExtensions
{
    /// <summary>
    /// Adds health check services for database and cache.
    /// </summary>
    public static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        string databaseConnectionString,
        string cacheConnectionString)
    {
        services.AddHealthChecks()
            .AddNpgSql(databaseConnectionString, name: "database")
            .AddRedis(cacheConnectionString, name: "cache");

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
