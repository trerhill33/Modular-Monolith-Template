using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Orders.Domain.Orders.Events;

public sealed record OrderStatusChangedDomainEvent(
    Guid OrderId,
    OrderStatus OldStatus,
    OrderStatus NewStatus) : DomainEvent;
