using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.GetCatalog;

public sealed record GetCatalogQuery(Guid CatalogId) : IQuery<CatalogResponse>;
