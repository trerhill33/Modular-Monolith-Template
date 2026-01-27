using ModularTemplate.Common.Application.EventBus;

namespace ModularTemplate.Modules.Sample.IntegrationEvents;

// Test file - should trigger CI check for missing TestCompletedIntegrationEventHandler.cs
public sealed record TestCompletedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid TestId,
    string Result) : IntegrationEvent(Id, OccurredOnUtc);
