using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleOrders.Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(Guid OrderId) : DomainEvent;
