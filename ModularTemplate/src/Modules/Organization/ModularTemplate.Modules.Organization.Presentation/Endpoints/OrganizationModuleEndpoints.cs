using ModularTemplate.Common.Presentation.Endpoints;

namespace ModularTemplate.Modules.Organization.Presentation.Endpoints;

public sealed class OrganizationModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "organization";

    public override string ModuleName => "Organization Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
