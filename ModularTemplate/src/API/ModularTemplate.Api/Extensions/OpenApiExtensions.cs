using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using ModularTemplate.Common.Presentation.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for OpenAPI/Swagger configuration with API versioning.
/// </summary>
internal static class OpenApiExtensions
{
    /// <summary>
    /// Adds OpenAPI/Swagger services with per-module schema separation.
    /// Each module gets its own Swagger document in the dropdown.
    /// </summary>
    public static IServiceCollection AddOpenApiVersioned(
        this IServiceCollection services,
        string applicationTitle,
        params IModuleEndpoints[] modules)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            // Create a separate Swagger doc for each module
            foreach (var module in modules)
            {
                options.SwaggerDoc(module.ModulePrefix, new OpenApiInfo
                {
                    Title = $"{applicationTitle} - {module.ModuleName}",
                    Version = "v1",
                    Description = $"API endpoints for the {module.ModuleName} module"
                });
            }

            // Filter endpoints to only show in their respective module's doc
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                var groupName = apiDesc.ActionDescriptor.EndpointMetadata
                    .OfType<EndpointGroupNameAttribute>()
                    .FirstOrDefault()?.EndpointGroupName;

                return string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);
            });

            // Customize operation IDs for cleaner client generation
            options.CustomOperationIds(apiDesc =>
                apiDesc.TryGetMethodInfo(out var methodInfo)
                    ? methodInfo.Name
                    : null);
        });

        return services;
    }

    /// <summary>
    /// Uses OpenAPI/Swagger middleware with per-module schema dropdown.
    /// Version info is obtained from API versioning provider when available.
    /// </summary>
    public static IApplicationBuilder UseOpenApiVersioned(
        this WebApplication app,
        params IModuleEndpoints[] modules)
    {
        if (app.Environment.IsDevelopment())
        {
            // Get version info for display (optional enhancement)
            var versionProvider = app.Services.GetService<IApiVersionDescriptionProvider>();
            var currentVersion = versionProvider?.ApiVersionDescriptions
                .FirstOrDefault(v => !v.IsDeprecated)?.ApiVersion.ToString() ?? "v1";

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Add a dropdown entry for each module
                foreach (var module in modules)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{module.ModulePrefix}/swagger.json",
                        $"{module.ModuleName} ({currentVersion})");
                }

                options.DisplayRequestDuration();
                options.EnableDeepLinking();
            });
        }

        return app;
    }
}
