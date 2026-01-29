using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.SampleSales.Application.Catalogs.GetCatalog;

namespace ModularTemplate.Modules.SampleSales.Application.Catalogs.GetCatalogs;

public sealed record GetCatalogsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CatalogResponse>>;
