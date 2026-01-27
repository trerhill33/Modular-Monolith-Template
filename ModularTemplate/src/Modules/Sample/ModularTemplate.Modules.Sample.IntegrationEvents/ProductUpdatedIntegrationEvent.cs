using ModularTemplate.Common.Application.EventBus;

namespace ModularTemplate.Modules.Sample.IntegrationEvents;

/// <summary>
/// Integration event published when a product is updated.
/// Other modules (e.g., Orders) can subscribe to sync their ProductCache.
/// </summary>
public sealed record ProductUpdatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive)
    : IntegrationEvent(Id, OccurredOnUtc);
