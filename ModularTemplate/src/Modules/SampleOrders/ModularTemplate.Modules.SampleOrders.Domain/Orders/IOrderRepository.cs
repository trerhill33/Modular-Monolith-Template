using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.SampleOrders.Domain.Orders;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<IReadOnlyCollection<Order>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
