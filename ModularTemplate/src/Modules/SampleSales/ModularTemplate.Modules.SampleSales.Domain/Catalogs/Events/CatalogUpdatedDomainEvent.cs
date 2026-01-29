using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogUpdatedDomainEvent(Guid CatalogId) : DomainEvent;
