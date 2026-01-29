using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleOrders.Domain.Orders.Events;

public sealed record OrderStatusChangedDomainEvent(
    Guid OrderId,
    OrderStatus OldStatus,
    OrderStatus NewStatus) : DomainEvent;
