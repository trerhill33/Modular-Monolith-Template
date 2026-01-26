using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Domain;
using ModularTemplate.Modules.Sales.Domain.Catalogs;
using ModularTemplate.Modules.Sales.Domain.OrdersCache;
using ModularTemplate.Modules.Sales.Domain.Products;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence.Configurations;

namespace ModularTemplate.Modules.Sales.Infrastructure.Persistence;

public sealed class SalesDbContext : ModuleDbContext<SalesDbContext>, IUnitOfWork<ISalesModule>
{
    public SalesDbContext(
        DbContextOptions<SalesDbContext> options,
        ILogger<SalesDbContext>? logger = null)
        : base(options)
    {
        Logger = logger;
    }

    protected override string Schema => Schemas.Sales;

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
