using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Orders.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints;

namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for module endpoint registration.
/// </summary>
internal static class ModuleExtensions
{
    /// <summary>
    /// Gets all module endpoint registrations.
    /// Each module has its own Swagger schema and endpoint group.
    /// </summary>
    public static IModuleEndpoints[] GetModuleEndpoints()
    {
        return
        [
            new SalesModuleEndpoints(),
            new OrdersModuleEndpoints(),
        ];
    }

    /// <summary>
    /// Maps all module endpoints to the application.
    /// </summary>
    public static IApplicationBuilder MapModuleEndpoints(
        this WebApplication app,
        params IModuleEndpoints[] modules)
    {
        foreach (var module in modules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}
