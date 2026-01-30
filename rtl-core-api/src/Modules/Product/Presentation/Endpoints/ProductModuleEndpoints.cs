using Rtl.Core.Presentation.Endpoints;

namespace Rtl.Module.Product.Presentation.Endpoints;

public sealed class ProductModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "product";

    public override string ModuleName => "Product Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
