using Rtl.Core.Presentation.Endpoints;
using Rtl.Module.SampleOrders.Presentation.Endpoints.Customers;
using Rtl.Module.SampleOrders.Presentation.Endpoints.Orders;

namespace Rtl.Module.SampleOrders.Presentation.Endpoints;

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
