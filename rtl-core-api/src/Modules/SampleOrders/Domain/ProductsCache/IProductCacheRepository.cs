using Rtl.Core.Domain;

namespace Rtl.Module.SampleOrders.Domain.ProductsCache;

/// <summary>
/// Read-only repository for product cache data.
/// Write operations are internal and performed only by integration event handlers.
/// </summary>
public interface IProductCacheRepository : IReadRepository<ProductCache, Guid>
{
    Task<IReadOnlyCollection<ProductCache>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
}
