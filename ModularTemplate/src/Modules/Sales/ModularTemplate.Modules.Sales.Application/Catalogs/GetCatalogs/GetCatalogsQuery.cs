using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Sales.Application.Catalogs.GetCatalog;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.GetCatalogs;

public sealed record GetCatalogsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CatalogResponse>>;
