using ModularTemplate.Common.Presentation.Endpoints;

namespace ModularTemplate.Modules.Product.Presentation.Endpoints;

public sealed class ProductModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "product";

    public override string ModuleName => "Product Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
