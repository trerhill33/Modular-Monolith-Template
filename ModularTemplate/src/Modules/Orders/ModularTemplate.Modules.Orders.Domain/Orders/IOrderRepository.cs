using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Orders.Domain.Orders;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<IReadOnlyCollection<Order>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
