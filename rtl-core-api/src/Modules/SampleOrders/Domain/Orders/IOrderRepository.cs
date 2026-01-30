using Rtl.Core.Domain;

namespace Rtl.Module.SampleOrders.Domain.Orders;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<IReadOnlyCollection<Order>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
