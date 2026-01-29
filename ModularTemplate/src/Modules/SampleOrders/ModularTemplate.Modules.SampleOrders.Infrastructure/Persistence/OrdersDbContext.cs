using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.SampleOrders.Domain;
using ModularTemplate.Modules.SampleOrders.Domain.Customers;
using ModularTemplate.Modules.SampleOrders.Domain.Orders;
using ModularTemplate.Modules.SampleOrders.Domain.ProductsCache;
using ModularTemplate.Modules.SampleOrders.Infrastructure.Persistence.Configurations;

namespace ModularTemplate.Modules.SampleOrders.Infrastructure.Persistence;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : ModuleDbContext<OrdersDbContext>(options), IUnitOfWork<ISampleOrdersModule>
{
    protected override string Schema => Schemas.Orders;

    internal DbSet<Order> Orders => Set<Order>();
    internal DbSet<OrderLine> OrderLines => Set<OrderLine>();
    internal DbSet<Customer> Customers => Set<Customer>();
    internal DbSet<ProductCache> ProductsCache => Set<ProductCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCacheConfiguration());
    }
}
