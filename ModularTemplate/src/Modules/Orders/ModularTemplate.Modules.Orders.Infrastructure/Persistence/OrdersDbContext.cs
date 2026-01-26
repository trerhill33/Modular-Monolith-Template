using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Orders.Domain;
using ModularTemplate.Modules.Orders.Domain.Customers;
using ModularTemplate.Modules.Orders.Domain.Orders;
using ModularTemplate.Modules.Orders.Domain.ProductsCache;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence.Configurations;

namespace ModularTemplate.Modules.Orders.Infrastructure.Persistence;

public sealed class OrdersDbContext : ModuleDbContext<OrdersDbContext>, IUnitOfWork<IOrdersModule>
{
    public OrdersDbContext(
        DbContextOptions<OrdersDbContext> options,
        ILogger<OrdersDbContext>? logger = null)
        : base(options)
    {
        Logger = logger;
    }

    protected override string Schema => Schemas.Orders;

    internal DbSet<Order> Orders => Set<Order>();
    internal DbSet<Customer> Customers => Set<Customer>();
    internal DbSet<ProductCache> ProductsCache => Set<ProductCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCacheConfiguration());
    }
}
