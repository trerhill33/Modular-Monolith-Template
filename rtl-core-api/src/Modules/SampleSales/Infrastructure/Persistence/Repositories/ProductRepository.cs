using Microsoft.EntityFrameworkCore;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleSales.Domain.Products;
using System.Linq.Expressions;

namespace Rtl.Module.SampleSales.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(SampleDbContext dbContext)
    : Repository<Product, Guid, SampleDbContext>(dbContext), IProductRepository
{
    protected override Expression<Func<Product, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Product>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = DbSet.AsNoTracking().OrderByDescending(p => p.CreatedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
