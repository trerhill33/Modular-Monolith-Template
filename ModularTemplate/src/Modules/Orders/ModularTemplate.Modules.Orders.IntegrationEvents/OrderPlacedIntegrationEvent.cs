using ModularTemplate.Common.Application.EventBus;

namespace ModularTemplate.Modules.Orders.IntegrationEvents;

/// <summary>
/// Integration event published when a new order is placed.
/// Other modules (e.g., Sales) can subscribe to sync their OrderCache.
/// </summary>
public sealed record OrderPlacedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice,
    string Status,
    DateTime OrderedAtUtc) : IntegrationEvent(Id, OccurredOnUtc);
