using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Organization.Domain;

namespace ModularTemplate.Modules.Organization.Infrastructure.Persistence;

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
