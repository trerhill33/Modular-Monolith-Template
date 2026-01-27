using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Sample.Domain.OrdersCache;

/// <summary>
/// Read-only repository for order cache data.
/// Write operations are internal and performed only by integration event handlers.
/// </summary>
public interface IOrderCacheRepository : IReadRepository<OrderCache, Guid>
{
    Task<IReadOnlyCollection<OrderCache>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
