using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleOrders.Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(Guid OrderId) : DomainEvent;
