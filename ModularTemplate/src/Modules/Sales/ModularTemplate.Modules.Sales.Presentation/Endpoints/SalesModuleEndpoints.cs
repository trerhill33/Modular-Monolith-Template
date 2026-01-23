using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Products;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints;

public sealed class SalesModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "sales";

    public override string ModuleName => "Sales Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield return ("products", "Products", new ProductsEndpoints());
    }
}
