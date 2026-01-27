using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sample.Domain.Catalogs.Events;

public sealed record CatalogUpdatedDomainEvent(Guid CatalogId) : DomainEvent;
