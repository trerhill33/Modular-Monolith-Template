using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sample.Domain.Catalogs.Events;

public sealed record CatalogCreatedDomainEvent(Guid CatalogId) : DomainEvent;
