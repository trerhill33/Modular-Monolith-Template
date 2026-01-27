using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.Orders.IntegrationEvents;
using ModularTemplate.Modules.Sample.Domain.OrdersCache;

namespace ModularTemplate.Modules.Sample.Presentation.IntegrationEvents;

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
            "Processing OrderPlaced integration event: OrderId={OrderId}, ProductId={ProductId}",
            integrationEvent.OrderId,
            integrationEvent.ProductId);

        var orderCache = new OrderCache
        {
            Id = integrationEvent.OrderId,
            ProductId = integrationEvent.ProductId,
            Quantity = integrationEvent.Quantity,
            TotalPrice = integrationEvent.TotalPrice,
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
