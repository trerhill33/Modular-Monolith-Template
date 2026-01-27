using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Sample.Domain.TestAggregate;

// Test file - should trigger CI check for missing TestCreatedDomainEventHandler.cs
public sealed record TestCreatedDomainEvent(Guid TestId) : DomainEvent;
