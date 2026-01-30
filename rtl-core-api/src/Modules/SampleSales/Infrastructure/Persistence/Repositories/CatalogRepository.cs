using Microsoft.EntityFrameworkCore;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleSales.Domain.Catalogs;
using System.Linq.Expressions;

namespace Rtl.Module.SampleSales.Infrastructure.Persistence.Repositories;

internal sealed class CatalogRepository(SampleDbContext dbContext)
    : Repository<Catalog, Guid, SampleDbContext>(dbContext), ICatalogRepository
{
    protected override Expression<Func<Catalog, Guid>> IdSelector => entity => entity.Id;

    public override async Task<IReadOnlyCollection<Catalog>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default)
    {
        IQueryable<Catalog> query = DbSet.AsNoTracking().OrderByDescending(c => c.CreatedAtUtc);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
