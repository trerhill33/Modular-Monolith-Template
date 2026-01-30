using Rtl.Core.Presentation.Endpoints;

namespace Rtl.Module.Customer.Presentation.Endpoints;

public sealed class CustomerModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "customer";

    public override string ModuleName => "Customer Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
