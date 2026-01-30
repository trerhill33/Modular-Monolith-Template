using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleSales.Application.Catalogs.GetCatalog;

public sealed record GetCatalogQuery(Guid CatalogId) : IQuery<CatalogResponse>;
