using Rtl.Core.Application.Messaging;
using Rtl.Module.SampleSales.Application.Catalogs.GetCatalog;

namespace Rtl.Module.SampleSales.Application.Catalogs.GetCatalogs;

public sealed record GetCatalogsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CatalogResponse>>;
