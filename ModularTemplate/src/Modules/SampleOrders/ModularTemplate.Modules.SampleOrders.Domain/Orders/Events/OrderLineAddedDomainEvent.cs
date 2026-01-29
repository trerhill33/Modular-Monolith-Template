using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleOrders.Domain.Orders.Events;

public sealed record OrderLineAddedDomainEvent(
    Guid OrderId,
    Guid OrderLineId,
    Guid ProductId,
    int Quantity) : DomainEvent;
