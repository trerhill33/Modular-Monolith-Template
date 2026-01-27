using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sample.Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId) : DomainEvent;
