using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sales.Domain.Catalogs.Events;

public sealed record CatalogCreatedDomainEvent(Guid CatalogId) : DomainEvent;
