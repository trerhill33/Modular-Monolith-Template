using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Infrastructure.Caching;
using ModularTemplate.Modules.Sample.Domain.OrdersCache;
using ModularTemplate.Modules.Sample.Presentation.IntegrationEvents;

namespace ModularTemplate.Modules.Sample.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for OrderCache that implements both read and write interfaces.
/// Inherits read operations from CacheReadRepository, adds custom queries and write operations.
/// </summary>
internal sealed class OrderCacheRepository(SampleDbContext dbContext)
    : CacheReadRepository<OrderCache, SampleDbContext>(dbContext),
      IOrderCacheRepository,
      IOrderCacheWriter
{
    public override async Task<IReadOnlyCollection<OrderCache>> GetAllAsync(
        int? limit = 100,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OrderCache> query = DbSet.OrderByDescending(o => o.OrderedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrderCache>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(o => o.ProductId == productId)
            .OrderByDescending(o => o.OrderedAtUtc)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Upserts an OrderCache entry. Used only by integration event handlers.
    /// </summary>
    public async Task UpsertAsync(OrderCache orderCache, CancellationToken cancellationToken = default)
    {
        var existing = await DbSet.FindAsync([orderCache.Id], cancellationToken);

        if (existing is null)
        {
            DbSet.Add(orderCache);
        }
        else
        {
            existing.ProductId = orderCache.ProductId;
            existing.Quantity = orderCache.Quantity;
            existing.TotalPrice = orderCache.TotalPrice;
            existing.Status = orderCache.Status;
            existing.OrderedAtUtc = orderCache.OrderedAtUtc;
            existing.LastSyncedAtUtc = orderCache.LastSyncedAtUtc;
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
