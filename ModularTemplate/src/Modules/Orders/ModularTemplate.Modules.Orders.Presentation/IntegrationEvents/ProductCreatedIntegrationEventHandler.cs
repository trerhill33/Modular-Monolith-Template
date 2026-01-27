using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.Orders.Domain.ProductsCache;
using ModularTemplate.Modules.Sample.IntegrationEvents;

namespace ModularTemplate.Modules.Orders.Presentation.IntegrationEvents;

/// <summary>
/// Handles ProductCreatedIntegrationEvent from the Sales module.
/// Upserts product data into the local ProductCache for read operations.
/// </summary>
internal sealed class ProductCreatedIntegrationEventHandler(
    ICacheWriteScope cacheWriteScope,
    IProductCacheWriter productCacheWriter,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProductCreatedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<ProductCreatedIntegrationEvent>
{
    public async Task HandleAsync(
        ProductCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        using var _ = cacheWriteScope.AllowWrites();

        logger.LogInformation(
            "Processing ProductCreated integration event: ProductId={ProductId}, Name={Name}",
            integrationEvent.ProductId,
            integrationEvent.Name);

        var productCache = new ProductCache
        {
            Id = integrationEvent.ProductId,
            Name = integrationEvent.Name,
            Description = integrationEvent.Description,
            Price = integrationEvent.Price,
            IsActive = true,
            LastSyncedAtUtc = dateTimeProvider.UtcNow
        };

        await productCacheWriter.UpsertAsync(productCache, cancellationToken);
    }

    public Task HandleAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default) => HandleAsync((ProductCreatedIntegrationEvent)integrationEvent, cancellationToken);
}

/// <summary>
/// Interface for writing to the ProductCache.
/// Used only by integration event handlers.
/// </summary>
public interface IProductCacheWriter
{
    Task UpsertAsync(ProductCache productCache, CancellationToken cancellationToken = default);
}
