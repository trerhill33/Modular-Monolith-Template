using ModularTemplate.Common.Presentation.Endpoints;

namespace ModularTemplate.Modules.Inventory.Presentation.Endpoints;

public sealed class InventoryModuleEndpoints : ModuleEndpoints
{
    public override string ModulePrefix => "inventory";

    public override string ModuleName => "Inventory Module";

    protected override IEnumerable<(string ResourcePath, string Tag, IResourceEndpoints Endpoints)> GetResources()
    {
        yield break; // Add resources here
    }
}
