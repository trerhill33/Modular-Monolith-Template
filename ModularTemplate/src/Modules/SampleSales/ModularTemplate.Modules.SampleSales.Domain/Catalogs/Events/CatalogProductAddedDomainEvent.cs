using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogProductAddedDomainEvent(
    Guid CatalogId,
    Guid CatalogProductId,
    Guid ProductId) : DomainEvent;
