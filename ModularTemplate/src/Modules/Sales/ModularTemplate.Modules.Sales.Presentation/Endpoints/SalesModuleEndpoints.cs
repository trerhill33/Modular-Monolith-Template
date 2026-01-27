using ModularTemplate.Common.Presentation.Endpoints;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints;

public sealed class SalesModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "sales";

    public override string ModuleName => "Sales Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
