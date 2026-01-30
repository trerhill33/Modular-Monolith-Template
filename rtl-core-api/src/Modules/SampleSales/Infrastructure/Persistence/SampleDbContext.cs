using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleSales.Domain;
using Rtl.Module.SampleSales.Domain.Catalogs;
using Rtl.Module.SampleSales.Domain.OrdersCache;
using Rtl.Module.SampleSales.Domain.Products;
using Rtl.Module.SampleSales.Infrastructure.Persistence.Configurations;

namespace Rtl.Module.SampleSales.Infrastructure.Persistence;

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
