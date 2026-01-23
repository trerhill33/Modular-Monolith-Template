namespace ModularTemplate.Common.Application.EventBus;

/// <summary>
/// Base record for integration events.
/// </summary>
public abstract record IntegrationEvent(Guid Id, DateTime OccurredOnUtc) : IIntegrationEvent;
