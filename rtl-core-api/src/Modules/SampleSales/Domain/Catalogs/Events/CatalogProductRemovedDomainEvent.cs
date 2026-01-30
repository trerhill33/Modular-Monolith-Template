using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogProductRemovedDomainEvent(
    Guid CatalogId,
    Guid CatalogProductId,
    Guid ProductId) : DomainEvent;
