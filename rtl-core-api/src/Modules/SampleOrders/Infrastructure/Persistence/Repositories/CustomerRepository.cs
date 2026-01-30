using Microsoft.EntityFrameworkCore;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleOrders.Domain.Customers;
using System.Linq.Expressions;

namespace Rtl.Module.SampleOrders.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository(OrdersDbContext dbContext)
    : Repository<Customer, Guid, OrdersDbContext>(dbContext), ICustomerRepository
{
    protected override Expression<Func<Customer, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Customer>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = DbSet.AsNoTracking().OrderByDescending(c => c.CreatedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }
}
