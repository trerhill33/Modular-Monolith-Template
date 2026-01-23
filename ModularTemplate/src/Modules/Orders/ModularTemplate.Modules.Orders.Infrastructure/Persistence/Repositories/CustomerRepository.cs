using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Orders.Domain.Customers;
using System.Linq.Expressions;

namespace ModularTemplate.Modules.Orders.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository(OrdersDbContext dbContext)
    : Repository<Customer, Guid, OrdersDbContext>(dbContext), ICustomerRepository
{
    protected override Expression<Func<Customer, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Customer>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = DbSet.OrderByDescending(c => c.CreatedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }
}
