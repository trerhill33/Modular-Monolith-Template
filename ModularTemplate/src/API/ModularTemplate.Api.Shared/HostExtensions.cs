using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Infrastructure.Application;
using ModularTemplate.Common.Presentation.Endpoints;

namespace ModularTemplate.Api.Shared;

/// <summary>
/// Extension methods for configuring per-module API hosts with common defaults.
/// Use these methods in module-specific host projects for consistent configuration.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Adds common services required for a module API host.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="moduleName">The name of the module (for OpenAPI documentation).</param>
    /// <param name="databaseConnectionString">The database connection string.</param>
    /// <param name="cacheConnectionString">The cache connection string (optional).</param>
    /// <param name="moduleEndpoints">The module's endpoint registration.</param>
    /// <returns>The web application builder for chaining.</returns>
    public static WebApplicationBuilder AddModuleHostDefaults(
        this WebApplicationBuilder builder,
        string moduleName,
        string databaseConnectionString,
        string? cacheConnectionString = null,
        IModuleEndpoints? moduleEndpoints = null)
    {
        // Application identity - must be registered first as other services depend on it
        builder.Services.AddOptions<ApplicationOptions>()
            .Bind(builder.Configuration.GetSection(ApplicationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Health checks
        var healthChecks = builder.Services.AddHealthChecks()
            .AddNpgSql(databaseConnectionString, name: "database");

        if (!string.IsNullOrEmpty(cacheConnectionString))
        {
            healthChecks.AddRedis(cacheConnectionString, name: "cache");
        }

        // API versioning
        builder.Services.AddApiVersioningServices();

        // OpenAPI/Swagger
        if (moduleEndpoints != null)
        {
            builder.Services.AddOpenApiForModule(moduleName, moduleEndpoints);
        }
        else
        {
            builder.Services.AddOpenApiSimple(moduleName);
        }

        // Exception handling
        builder.Services.AddGlobalExceptionHandling();

        // CORS - configured from appsettings, defaults to restrictive policy
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
                {
                    // Development fallback only - still requires explicit configuration in non-dev
                    policy.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
                else if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
                // If no origins configured in non-dev, CORS will block all cross-origin requests
            });
        });

        return builder;
    }

    /// <summary>
    /// Configures the middleware pipeline for a module API host.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <param name="moduleEndpoints">The module's endpoint registration (optional).</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication UseModuleHostDefaults(
        this WebApplication app,
        IModuleEndpoints? moduleEndpoints = null)
    {
        // OpenAPI/Swagger (development only)
        if (moduleEndpoints != null)
        {
            app.UseOpenApiForModule(moduleEndpoints);
        }
        else
        {
            app.UseOpenApiSimple();
        }

        // Health checks
        app.MapHealthCheckEndpoint();

        // Exception handling
        app.UseGlobalExceptionHandling();

        // CORS
        app.UseCors();

        // Auth
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
