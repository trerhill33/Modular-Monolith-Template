using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Product.Domain;

namespace ModularTemplate.Modules.Product.Infrastructure.Persistence;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options)
    : ModuleDbContext<ProductDbContext>(options), IUnitOfWork<IProductModule>
{
    protected override string Schema => Schemas.Product;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations here
    }
}
