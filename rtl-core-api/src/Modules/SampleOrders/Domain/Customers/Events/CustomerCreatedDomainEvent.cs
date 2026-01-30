using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleOrders.Domain.Customers.Events;

public sealed record CustomerCreatedDomainEvent(Guid CustomerId) : DomainEvent;
