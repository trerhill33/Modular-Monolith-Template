using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ModularTemplate.Api.Shared.HealthChecks;
using ModularTemplate.Common.Infrastructure.Application;
using Npgsql;

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
    /// Adds granular health checks for outbox lag, inbox lag, queue depth, and per-module health.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// Requires ApplicationOptions to be registered first via AddApplicationOptions().
    /// Validation is handled by ValidateOnStart() at app startup.
    /// </remarks>
    public static IServiceCollection AddGranularHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get module schemas from ApplicationOptions (single source of truth)
        var moduleSchemas = GetModuleSchemas(configuration);

        // Configure options from configuration with validation
        // Use PostConfigure to inject schemas from ApplicationOptions
        services.AddOptions<OutboxLagHealthCheckOptions>()
            .Bind(configuration.GetSection(OutboxLagHealthCheckOptions.SectionName))
            .PostConfigure(options => options.Schemas = moduleSchemas)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<InboxLagHealthCheckOptions>()
            .Bind(configuration.GetSection(InboxLagHealthCheckOptions.SectionName))
            .PostConfigure(options => options.Schemas = moduleSchemas)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<SqsQueueDepthHealthCheckOptions>()
            .Bind(configuration.GetSection(SqsQueueDepthHealthCheckOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register health checks
        services.AddHealthChecks()
            .AddCheck<OutboxLagHealthCheck>(
                "outbox-lag",
                failureStatus: HealthStatus.Degraded,
                tags: ["messaging", "outbox"])
            .AddCheck<InboxLagHealthCheck>(
                "inbox-lag",
                failureStatus: HealthStatus.Degraded,
                tags: ["messaging", "inbox"])
            .AddCheck<SqsQueueDepthHealthCheck>(
                "sqs-queue-depth",
                failureStatus: HealthStatus.Degraded,
                tags: ["messaging", "sqs"]);

        // Add per-module health checks
        services.AddModuleHealthChecks(configuration);

        return services;
    }

    /// <summary>
    /// Adds per-module health checks for each configured module.
    /// </summary>
    private static IServiceCollection AddModuleHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get module schemas from ApplicationOptions (single source of truth)
        var moduleSchemas = GetModuleSchemas(configuration);

        var healthChecksBuilder = services.AddHealthChecks();

        foreach (var schema in moduleSchemas)
        {
            var moduleName = char.ToUpperInvariant(schema[0]) + schema[1..];
            var checkName = $"module-{schema}";

            // Register a factory for each module health check
            healthChecksBuilder.Add(new HealthCheckRegistration(
                checkName,
                sp =>
                {
                    var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
                    var options = new ModuleHealthCheckOptions
                    {
                        ModuleName = moduleName,
                        Schema = schema
                    };

                    // Allow per-module configuration overrides
                    var moduleSection = configuration.GetSection($"HealthChecks:Module:{moduleName}");
                    if (moduleSection.Exists())
                    {
                        options.OutboxDegradedThresholdSeconds =
                            moduleSection.GetValue("OutboxDegradedThresholdSeconds", options.OutboxDegradedThresholdSeconds);
                        options.OutboxUnhealthyThresholdSeconds =
                            moduleSection.GetValue("OutboxUnhealthyThresholdSeconds", options.OutboxUnhealthyThresholdSeconds);
                        options.OutboxDegradedCountThreshold =
                            moduleSection.GetValue("OutboxDegradedCountThreshold", options.OutboxDegradedCountThreshold);
                        options.OutboxUnhealthyCountThreshold =
                            moduleSection.GetValue("OutboxUnhealthyCountThreshold", options.OutboxUnhealthyCountThreshold);
                    }

                    return new ModuleHealthCheck(dataSource, options);
                },
                failureStatus: HealthStatus.Degraded,
                tags: ["module", schema]));
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

    /// <summary>
    /// Maps a filtered health check endpoint that only runs health checks with specific tags.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <param name="pattern">The URL pattern for the endpoint.</param>
    /// <param name="tags">The tags to filter health checks by.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapTaggedHealthCheckEndpoint(
        this IEndpointRouteBuilder app,
        string pattern,
        params string[] tags)
    {
        app.MapHealthChecks(pattern, new HealthCheckOptions
        {
            Predicate = check => tags.Any(tag => check.Tags.Contains(tag)),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }).AllowAnonymous();

        return app;
    }

    /// <summary>
    /// Gets module schemas from ApplicationOptions configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The array of module schema names.</returns>
    /// <remarks>
    /// ApplicationOptions must be registered with ValidateOnStart() before this is called.
    /// Validation of required fields is handled by ValidateOnStart() during app startup.
    /// </remarks>
    private static string[] GetModuleSchemas(IConfiguration configuration)
    {
        var applicationOptions = configuration
            .GetSection(ApplicationOptions.SectionName)
            .Get<ApplicationOptions>();

        // GetModules() returns Modules array if set, otherwise derives from DatabaseName/Name
        // If config is invalid, ValidateOnStart() will fail at app.Build()
        return applicationOptions?.GetModules() ?? [];
    }
}
