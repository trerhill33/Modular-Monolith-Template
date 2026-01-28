using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Orders.Domain.Orders;
using System.Linq.Expressions;

namespace ModularTemplate.Modules.Orders.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(OrdersDbContext dbContext)
    : Repository<Order, Guid, OrdersDbContext>(dbContext), IOrderRepository
{
    protected override Expression<Func<Order, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Order>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = DbSet.AsNoTracking().OrderByDescending(o => o.OrderedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(o => o.ProductId == productId)
            .OrderByDescending(o => o.OrderedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
