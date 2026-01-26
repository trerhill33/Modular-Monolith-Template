using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints;

public sealed class OrdersModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "orders";

    public override string ModuleName => "Orders Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield return ("orders", "Orders", new OrdersEndpoints());
        yield return ("customers", "Customers", new CustomersEndpoints());
    }
}
