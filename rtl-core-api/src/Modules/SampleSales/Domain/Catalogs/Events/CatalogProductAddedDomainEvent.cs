using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogProductAddedDomainEvent(
    Guid CatalogId,
    Guid CatalogProductId,
    Guid ProductId) : DomainEvent;
