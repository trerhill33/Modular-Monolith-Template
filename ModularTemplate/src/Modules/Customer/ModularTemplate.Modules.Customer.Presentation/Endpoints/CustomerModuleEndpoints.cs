using ModularTemplate.Common.Presentation.Endpoints;

namespace ModularTemplate.Modules.Customer.Presentation.Endpoints;

public sealed class CustomerModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "customer";

    public override string ModuleName => "Customer Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
