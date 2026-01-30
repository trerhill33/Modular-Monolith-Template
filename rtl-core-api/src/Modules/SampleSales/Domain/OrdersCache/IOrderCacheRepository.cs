using Rtl.Core.Domain;

namespace Rtl.Module.SampleSales.Domain.OrdersCache;

/// <summary>
/// Read-only repository for order cache data.
/// Write operations are internal and performed only by integration event handlers.
/// </summary>
public interface IOrderCacheRepository : IReadRepository<OrderCache, Guid>
{
    Task<IReadOnlyCollection<OrderCache>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
