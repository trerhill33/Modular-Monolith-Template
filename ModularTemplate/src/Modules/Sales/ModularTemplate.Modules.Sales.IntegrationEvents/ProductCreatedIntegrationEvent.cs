using ModularTemplate.Common.Application.EventBus;

namespace ModularTemplate.Modules.Sales.IntegrationEvents;

/// <summary>
/// Integration event published when a new product is created.
/// Other modules (e.g., Orders) can subscribe to sync their ProductCache.
/// </summary>
public sealed record ProductCreatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price)
    : IntegrationEvent(Id, OccurredOnUtc);
