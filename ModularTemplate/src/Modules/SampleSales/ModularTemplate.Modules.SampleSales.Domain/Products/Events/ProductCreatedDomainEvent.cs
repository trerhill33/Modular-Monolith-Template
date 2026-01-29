using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.SampleSales.Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId) : DomainEvent;
