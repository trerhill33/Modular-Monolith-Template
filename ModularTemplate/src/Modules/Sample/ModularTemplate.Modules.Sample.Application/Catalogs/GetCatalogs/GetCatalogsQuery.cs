using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Sample.Application.Catalogs.GetCatalog;

namespace ModularTemplate.Modules.Sample.Application.Catalogs.GetCatalogs;

public sealed record GetCatalogsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CatalogResponse>>;
