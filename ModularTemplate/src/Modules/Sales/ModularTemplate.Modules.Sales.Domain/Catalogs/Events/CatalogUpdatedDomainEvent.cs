using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sales.Domain.Catalogs.Events;

public sealed record CatalogUpdatedDomainEvent(Guid CatalogId) : DomainEvent;
