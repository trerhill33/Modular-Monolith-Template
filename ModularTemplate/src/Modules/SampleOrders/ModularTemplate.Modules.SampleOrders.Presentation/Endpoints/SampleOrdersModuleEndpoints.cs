using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.SampleOrders.Presentation.Endpoints.Customers;
using ModularTemplate.Modules.SampleOrders.Presentation.Endpoints.Orders;

namespace ModularTemplate.Modules.SampleOrders.Presentation.Endpoints;

public sealed class SampleOrdersModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "sampleorders";

    public override string ModuleName => "SampleOrders Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield return ("orders", "Orders", new OrdersEndpoints());
        yield return ("customers", "Customers", new CustomersEndpoints());
    }
}
