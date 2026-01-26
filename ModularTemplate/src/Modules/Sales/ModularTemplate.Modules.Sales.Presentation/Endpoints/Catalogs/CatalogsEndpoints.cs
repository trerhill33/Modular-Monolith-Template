using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.CreateCatalog;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.GetAllCatalogs;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.GetCatalogById;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.UpdateCatalog;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.V2;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs;

internal sealed class CatalogsEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        // V1 endpoints
        new GetAllCatalogsEndpoint(),
        new GetCatalogByIdEndpoint(),
        new CreateCatalogEndpoint(),
        new UpdateCatalogEndpoint(),

        // V2 endpoints (demonstrate versioning with enhanced response)
        new GetAllCatalogsV2Endpoint()
    ];
}
