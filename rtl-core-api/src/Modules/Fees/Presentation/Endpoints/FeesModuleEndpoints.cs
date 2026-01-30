using Rtl.Core.Presentation.Endpoints;

namespace Rtl.Module.Fees.Presentation.Endpoints;

public sealed class FeesModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "fees";

    public override string ModuleName => "Fees Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
