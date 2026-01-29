using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleOrders.Domain.Orders.Events;

public sealed record OrderLineRemovedDomainEvent(
    Guid OrderId,
    Guid OrderLineId,
    Guid ProductId) : DomainEvent;
