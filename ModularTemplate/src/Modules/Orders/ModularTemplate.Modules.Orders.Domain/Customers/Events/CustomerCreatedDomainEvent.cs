using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Orders.Domain.Customers.Events;

public sealed record CustomerCreatedDomainEvent(Guid CustomerId) : DomainEvent;
