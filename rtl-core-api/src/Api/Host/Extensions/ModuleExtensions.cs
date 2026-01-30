using Asp.Versioning.Builder;
using Rtl.Core.Presentation.Endpoints;
using Rtl.Module.Customer.Presentation.Endpoints;
using Rtl.Module.Fees.Presentation.Endpoints;
using Rtl.Module.Organization.Presentation.Endpoints;
using Rtl.Module.Product.Presentation.Endpoints;
using Rtl.Module.Sales.Presentation.Endpoints;
using Rtl.Module.SampleOrders.Presentation.Endpoints;
using Rtl.Module.SampleSales.Presentation.Endpoints;

namespace Rtl.Core.Api.Extensions;

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
