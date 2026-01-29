using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.SampleOrders.IntegrationEvents;
using ModularTemplate.Modules.SampleSales.Domain.OrdersCache;

namespace ModularTemplate.Modules.SampleSales.Presentation.IntegrationEvents;

/// <summary>
/// Handles OrderPlacedIntegrationEvent from the Orders module.
/// Upserts order data into the local OrderCache for read operations.
/// </summary>
internal sealed class OrderPlacedIntegrationEventHandler(
    ICacheWriteScope cacheWriteScope,
    IOrderCacheWriter orderCacheWriter,
    IDateTimeProvider dateTimeProvider,
    ILogger<OrderPlacedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<OrderPlacedIntegrationEvent>
{
    public async Task HandleAsync(
        OrderPlacedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        using var _ = cacheWriteScope.AllowWrites();

        logger.LogInformation(
            "Processing OrderPlaced integration event: OrderId={OrderId}, CustomerId={CustomerId}",
            integrationEvent.OrderId,
            integrationEvent.CustomerId);

        var orderCache = new OrderCache
        {
            Id = integrationEvent.OrderId,
            CustomerId = integrationEvent.CustomerId,
            TotalPrice = integrationEvent.TotalPrice,
            Currency = integrationEvent.Currency,
            Status = integrationEvent.Status,
            OrderedAtUtc = integrationEvent.OrderedAtUtc,
            LastSyncedAtUtc = dateTimeProvider.UtcNow
        };

        await orderCacheWriter.UpsertAsync(orderCache, cancellationToken);
    }

    public Task HandleAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        return HandleAsync((OrderPlacedIntegrationEvent)integrationEvent, cancellationToken);
    }
}

/// <summary>
/// Interface for writing to the OrderCache.
/// Used only by integration event handlers.
/// </summary>
public interface IOrderCacheWriter
{
    Task UpsertAsync(OrderCache orderCache, CancellationToken cancellationToken = default);
}
