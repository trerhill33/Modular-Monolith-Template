using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Domain.Products;
using System.Linq.Expressions;

namespace ModularTemplate.Modules.Sales.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(SalesDbContext dbContext)
    : Repository<Product, Guid, SalesDbContext>(dbContext), IProductRepository
{
    protected override Expression<Func<Product, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Product>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = DbSet.OrderByDescending(p => p.CreatedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
