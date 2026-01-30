using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.Customer.Domain;

namespace Rtl.Module.Customer.Infrastructure.Persistence;

public sealed class CustomerDbContext(DbContextOptions<CustomerDbContext> options)
    : ModuleDbContext<CustomerDbContext>(options), IUnitOfWork<ICustomerModule>
{
    protected override string Schema => Schemas.Customer;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations here
    }
}
