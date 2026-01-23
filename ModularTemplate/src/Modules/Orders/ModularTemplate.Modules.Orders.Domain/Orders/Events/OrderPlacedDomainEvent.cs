using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Orders.Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(Guid OrderId) : DomainEvent;
