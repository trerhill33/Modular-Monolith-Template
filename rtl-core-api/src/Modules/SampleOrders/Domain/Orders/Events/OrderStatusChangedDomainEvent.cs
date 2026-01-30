using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleOrders.Domain.Orders.Events;

public sealed record OrderStatusChangedDomainEvent(
    Guid OrderId,
    OrderStatus OldStatus,
    OrderStatus NewStatus) : DomainEvent;
