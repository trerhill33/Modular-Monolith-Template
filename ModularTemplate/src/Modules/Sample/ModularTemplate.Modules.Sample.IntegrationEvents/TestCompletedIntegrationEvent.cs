using ModularTemplate.Common.Application.EventBus;

namespace ModularTemplate.Modules.Sample.IntegrationEvents;

// Test file - should trigger CI check for missing TestCompletedIntegrationEventHandler.cs
public sealed class TestCompletedIntegrationEvent : IntegrationEvent
{
    public Guid TestId { get; init; }
    public string Result { get; init; } = string.Empty;
}
