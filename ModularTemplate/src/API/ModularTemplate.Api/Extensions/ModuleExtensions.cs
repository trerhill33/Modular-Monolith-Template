using Asp.Versioning.Builder;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Customer.Presentation.Endpoints;
using ModularTemplate.Modules.Fees.Presentation.Endpoints;
using ModularTemplate.Modules.Organization.Presentation.Endpoints;
using ModularTemplate.Modules.Product.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints;
using ModularTemplate.Modules.SampleOrders.Presentation.Endpoints;
using ModularTemplate.Modules.SampleSales.Presentation.Endpoints;

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
            new SampleSalesModuleEndpoints(),
            new SampleOrdersModuleEndpoints(),
            new OrganizationModuleEndpoints(),
            new CustomerModuleEndpoints(),
            new SalesModuleEndpoints(),
            new FeesModuleEndpoints(),
            new ProductModuleEndpoints(),
        ];
    }

    /// <summary>
    /// Maps all module endpoints with API versioning.
    /// </summary>
    public static IApplicationBuilder MapVersionedModuleEndpoints(
        this WebApplication app,
        ApiVersionSet versionSet,
        params IModuleEndpoints[] modules)
    {
        foreach (var module in modules)
        {
            module.MapEndpoints(app, versionSet);
        }

        return app;
    }
}
