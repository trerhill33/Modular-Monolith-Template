using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.V1;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.V2;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs;

internal sealed class CatalogsEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        // V1 endpoints
        new V1.GetAllCatalogsEndpoint(),
        new V1.GetCatalogByIdEndpoint(),
        new V1.CreateCatalogEndpoint(),
        new V1.UpdateCatalogEndpoint(),

        // V2 endpoints (demonstrate versioning with enhanced response)
        new V2.GetAllCatalogsV2Endpoint()
    ];
}
