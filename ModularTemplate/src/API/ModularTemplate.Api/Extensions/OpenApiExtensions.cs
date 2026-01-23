using Microsoft.OpenApi.Models;
using ModularTemplate.Common.Presentation.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for OpenAPI/Swagger configuration.
/// </summary>
internal static class OpenApiExtensions
{
    /// <summary>
    /// Adds OpenAPI/Swagger services with per-module schema separation.
    /// Each module gets its own Swagger document in the dropdown.
    /// </summary>
    public static IServiceCollection AddOpenApi(
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
                // Get the group name from the endpoint metadata
                var groupName = apiDesc.ActionDescriptor.EndpointMetadata
                    .OfType<EndpointGroupNameAttribute>()
                    .FirstOrDefault()?.EndpointGroupName;

                // Include endpoint if its group matches the doc name
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
    /// </summary>
    public static IApplicationBuilder UseOpenApi(
        this WebApplication app,
        params IModuleEndpoints[] modules)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Add a dropdown entry for each module
                foreach (var module in modules)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{module.ModulePrefix}/swagger.json",
                        module.ModuleName);
                }

                options.DisplayRequestDuration();
                options.EnableDeepLinking();
            });
        }

        return app;
    }
}
