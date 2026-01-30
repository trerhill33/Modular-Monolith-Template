using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleOrders.Domain.Orders.Events;

public sealed record OrderLineAddedDomainEvent(
    Guid OrderId,
    Guid OrderLineId,
    Guid ProductId,
    int Quantity) : DomainEvent;
