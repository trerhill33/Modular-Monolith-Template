using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleOrders.Domain.Orders.Events;

public sealed record OrderLineRemovedDomainEvent(
    Guid OrderId,
    Guid OrderLineId,
    Guid ProductId) : DomainEvent;
