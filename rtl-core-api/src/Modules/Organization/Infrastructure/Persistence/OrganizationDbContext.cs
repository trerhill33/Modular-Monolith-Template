using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.Organization.Domain;

namespace Rtl.Module.Organization.Infrastructure.Persistence;

public sealed class OrganizationDbContext(DbContextOptions<OrganizationDbContext> options)
    : ModuleDbContext<OrganizationDbContext>(options), IUnitOfWork<IOrganizationModule>
{
    protected override string Schema => Schemas.Organization;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations here
    }
}
