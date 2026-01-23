using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Domain.Catalogs;
using System.Linq.Expressions;

namespace ModularTemplate.Modules.Sales.Infrastructure.Persistence.Repositories;

internal sealed class CatalogRepository(SalesDbContext dbContext)
    : Repository<Catalog, Guid, SalesDbContext>(dbContext), ICatalogRepository
{
    protected override Expression<Func<Catalog, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Catalog>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Catalog> query = DbSet.OrderByDescending(c => c.CreatedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
