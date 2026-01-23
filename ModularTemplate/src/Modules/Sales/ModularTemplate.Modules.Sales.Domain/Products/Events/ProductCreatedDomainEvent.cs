using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sales.Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId) : DomainEvent;
