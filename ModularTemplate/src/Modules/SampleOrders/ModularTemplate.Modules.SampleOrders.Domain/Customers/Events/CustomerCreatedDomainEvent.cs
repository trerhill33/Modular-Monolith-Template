using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleOrders.Domain.Customers.Events;

public sealed record CustomerCreatedDomainEvent(Guid CustomerId) : DomainEvent;
