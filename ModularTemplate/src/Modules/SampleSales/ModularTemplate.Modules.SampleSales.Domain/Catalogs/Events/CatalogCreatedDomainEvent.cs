using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogCreatedDomainEvent(Guid CatalogId) : DomainEvent;
