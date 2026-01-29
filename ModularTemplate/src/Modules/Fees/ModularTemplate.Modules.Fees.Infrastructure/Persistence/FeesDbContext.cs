using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Fees.Domain;

namespace ModularTemplate.Modules.Fees.Infrastructure.Persistence;

public sealed class FeesDbContext(DbContextOptions<FeesDbContext> options)
    : ModuleDbContext<FeesDbContext>(options), IUnitOfWork<IFeesModule>
{
    protected override string Schema => Schemas.Fees;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations here
    }
}
