using ModularTemplate.Common.Application.EventBus;

namespace ModularTemplate.Modules.SampleOrders.IntegrationEvents;

/// <summary>
/// Integration event published when a new order is placed.
/// Other modules (e.g., Sales) can subscribe to sync their OrderCache.
/// </summary>
public sealed record OrderPlacedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid OrderId,
    Guid CustomerId,
    IReadOnlyCollection<OrderLineDto> Lines,
    decimal TotalPrice,
    string Currency,
    string Status,
    DateTime OrderedAtUtc) : IntegrationEvent(Id, OccurredOnUtc);

public sealed record OrderLineDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    string Currency);
