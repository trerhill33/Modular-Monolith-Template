using Rtl.Core.Presentation.Endpoints;

namespace Rtl.Module.Sales.Presentation.Endpoints;

public sealed class SalesModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "sales";

    public override string ModuleName => "Sales Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
