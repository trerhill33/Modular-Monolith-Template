using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Sample.Domain.TestAggregate;

// Test file - should trigger CI check for missing TestCreatedDomainEventHandler.cs
public sealed class TestCreatedDomainEvent(Guid testId) : DomainEvent
{
    public Guid TestId { get; } = testId;
}
