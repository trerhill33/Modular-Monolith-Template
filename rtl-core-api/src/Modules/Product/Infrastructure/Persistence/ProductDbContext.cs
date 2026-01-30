using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.Product.Domain;

namespace Rtl.Module.Product.Infrastructure.Persistence;

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
