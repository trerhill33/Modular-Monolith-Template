using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.SampleSales.Domain;
using ModularTemplate.Modules.SampleSales.Domain.Catalogs;
using ModularTemplate.Modules.SampleSales.Domain.OrdersCache;
using ModularTemplate.Modules.SampleSales.Domain.Products;
using ModularTemplate.Modules.SampleSales.Infrastructure.Persistence.Configurations;

namespace ModularTemplate.Modules.SampleSales.Infrastructure.Persistence;

public sealed class SampleDbContext(DbContextOptions<SampleDbContext> options)
    : ModuleDbContext<SampleDbContext>(options), IUnitOfWork<ISampleSalesModule>
{
    protected override string Schema => Schemas.Sample;

    internal DbSet<Product> Products => Set<Product>();
    internal DbSet<Catalog> Catalogs => Set<Catalog>();
    internal DbSet<CatalogProduct> CatalogProducts => Set<CatalogProduct>();
    internal DbSet<OrderCache> OrdersCache => Set<OrderCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogProductConfiguration());
        modelBuilder.ApplyConfiguration(new OrderCacheConfiguration());
    }
}
