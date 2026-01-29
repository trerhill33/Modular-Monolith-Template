using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleSales.Application.Catalogs.GetCatalog;

public sealed record GetCatalogQuery(Guid CatalogId) : IQuery<CatalogResponse>;
