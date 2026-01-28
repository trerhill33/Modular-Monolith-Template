using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Domain.Catalogs;
using ModularTemplate.Modules.Sample.Domain.OrdersCache;
using ModularTemplate.Modules.Sample.Domain.Products;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence.Configurations;

namespace ModularTemplate.Modules.Sample.Infrastructure.Persistence;

public sealed class SampleDbContext(DbContextOptions<SampleDbContext> options)
    : ModuleDbContext<SampleDbContext>(options), IUnitOfWork<ISampleModule>
{
    protected override string Schema => Schemas.Sample;

    internal DbSet<Product> Products => Set<Product>();
    internal DbSet<Catalog> Catalogs => Set<Catalog>();
    internal DbSet<OrderCache> OrdersCache => Set<OrderCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogConfiguration());
        modelBuilder.ApplyConfiguration(new OrderCacheConfiguration());
    }
}
