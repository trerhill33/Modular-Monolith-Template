using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.CreateCatalog;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.GetAllCatalogs;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.GetCatalogById;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.UpdateCatalog;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs;

internal sealed class CatalogsEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        new GetAllCatalogsEndpoint(),
        new GetCatalogByIdEndpoint(),
        new CreateCatalogEndpoint(),
        new UpdateCatalogEndpoint()
    ];
}
