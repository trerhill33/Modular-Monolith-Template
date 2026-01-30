using Microsoft.Extensions.Logging;
using Rtl.Core.Application.Caching;
using Rtl.Core.Application.EventBus;
using Rtl.Core.Domain;
using Rtl.Module.SampleOrders.Domain.ProductsCache;
using Rtl.Module.SampleSales.IntegrationEvents;

namespace Rtl.Module.SampleOrders.Presentation.IntegrationEvents;

/// <summary>
/// Handles ProductUpdatedIntegrationEvent from the Sales module.
/// Updates the product data in the local ProductCache.
/// </summary>
internal sealed class ProductUpdatedIntegrationEventHandler(
    ICacheWriteScope cacheWriteScope,
    IProductCacheWriter productCacheWriter,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProductUpdatedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<ProductUpdatedIntegrationEvent>
{
    public async Task HandleAsync(
        ProductUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        using var _ = cacheWriteScope.AllowWrites();

        logger.LogInformation(
            "Processing ProductUpdated integration event: ProductId={ProductId}, Name={Name}, IsActive={IsActive}",
            integrationEvent.ProductId,
            integrationEvent.Name,
            integrationEvent.IsActive);

        var productCache = new ProductCache
        {
            Id = integrationEvent.ProductId,
            Name = integrationEvent.Name,
            Description = integrationEvent.Description,
            Price = integrationEvent.Price,
            IsActive = integrationEvent.IsActive,
            LastSyncedAtUtc = dateTimeProvider.UtcNow
        };

        await productCacheWriter.UpsertAsync(productCache, cancellationToken);
    }

    public Task HandleAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        return HandleAsync((ProductUpdatedIntegrationEvent)integrationEvent, cancellationToken);
    }
}
