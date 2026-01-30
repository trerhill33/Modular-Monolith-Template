using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.Fees.Domain;

namespace Rtl.Module.Fees.Infrastructure.Persistence;

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
