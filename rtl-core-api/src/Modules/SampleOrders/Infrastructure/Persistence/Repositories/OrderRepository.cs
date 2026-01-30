using Microsoft.EntityFrameworkCore;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleOrders.Domain.Orders;
using System.Linq.Expressions;

namespace Rtl.Module.SampleOrders.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(OrdersDbContext dbContext)
    : Repository<Order, Guid, OrdersDbContext>(dbContext), IOrderRepository
{
    protected override Expression<Func<Order, Guid>> IdSelector => entity => entity.Id;

    public override async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyCollection<Order>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = DbSet
            .Include(o => o.Lines)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Lines)
            .AsNoTracking()
            .Where(o => o.Lines.Any(l => l.ProductId == productId))
            .OrderByDescending(o => o.OrderedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Lines)
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
