using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sample.Presentation.Endpoints.Catalogs;
using ModularTemplate.Modules.Sample.Presentation.Endpoints.Products;

namespace ModularTemplate.Modules.Sample.Presentation.Endpoints;

public sealed class SampleModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "sample";

    public override string ModuleName => "Sample Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield return ("products", "Products", new ProductsEndpoints());
        yield return ("catalogs", "Catalogs", new CatalogsEndpoints());
    }
}
