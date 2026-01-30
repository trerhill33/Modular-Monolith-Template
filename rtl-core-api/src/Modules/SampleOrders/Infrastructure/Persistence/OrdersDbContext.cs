using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleOrders.Domain;
using Rtl.Module.SampleOrders.Domain.Customers;
using Rtl.Module.SampleOrders.Domain.Orders;
using Rtl.Module.SampleOrders.Domain.ProductsCache;
using Rtl.Module.SampleOrders.Infrastructure.Persistence.Configurations;

namespace Rtl.Module.SampleOrders.Infrastructure.Persistence;

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
