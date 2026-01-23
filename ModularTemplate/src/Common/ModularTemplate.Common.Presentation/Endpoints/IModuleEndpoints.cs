using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ModularTemplate.Common.Presentation.Endpoints;

/// <summary>
/// Interface for module-level endpoint registration.
/// Each module implements this to expose all its resources.
/// </summary>
/// <remarks>
/// When extracted to a microservice, the module's endpoints can be
/// registered with a single call to MapEndpoints().
/// </remarks>
public interface IModuleEndpoints
{
    /// <summary>
    /// The module's URL prefix (e.g., "sample", "orders", "inventory").
    /// All resources in this module will be prefixed with this path.
    /// </summary>
    string ModulePrefix { get; }

    /// <summary>
    /// The module name for Swagger/OpenAPI documentation.
    /// This creates a separate schema in the Swagger dropdown.
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// Maps all module endpoints to the application.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app);
}

/// <summary>
/// Base class for module endpoint registration.
/// Provides common functionality for registering resources.
/// </summary>
public abstract class ModuleEndpoints : IModuleEndpoints
{
    public abstract string ModulePrefix { get; }

    public abstract string ModuleName { get; }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        // Note: No module prefix in URL - modules are logical boundaries, not URL constructs
        // URLs are resource-based: /sample-items, /categories, /orders
        // NOT: /sample/sample-items, /sample/categories

        foreach (var (resourcePath, tag, endpoints) in GetResources())
        {
            var resourceGroup = app.MapGroup(resourcePath);

            // Apply tags and group name for Swagger organization
            resourceGroup.WithTags(tag);
            resourceGroup.WithGroupName(ModulePrefix); // Links to module's Swagger schema

            endpoints.MapEndpoints(resourceGroup);
        }

        return app;
    }

    /// <summary>
    /// Returns all resources in this module.
    /// Override this to register your module's resources.
    /// </summary>
    /// <returns>Collection of (resourcePath, swaggerTag, endpoints) tuples.</returns>
    protected abstract IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources();
}
