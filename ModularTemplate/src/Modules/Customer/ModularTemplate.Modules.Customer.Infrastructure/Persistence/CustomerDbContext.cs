using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Customer.Domain;

namespace ModularTemplate.Modules.Customer.Infrastructure.Persistence;

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
