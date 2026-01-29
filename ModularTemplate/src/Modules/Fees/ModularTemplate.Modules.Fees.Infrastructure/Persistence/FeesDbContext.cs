using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Fees.Domain;
using ModularTemplate.Modules.Fees.Domain.FeeSchedules;
using ModularTemplate.Modules.Fees.Infrastructure.Persistence.Configurations;

namespace ModularTemplate.Modules.Fees.Infrastructure.Persistence;

public sealed class FeesDbContext(DbContextOptions<FeesDbContext> options)
    : ModuleDbContext<FeesDbContext>(options), IUnitOfWork<IFeesModule>
{
    public DbSet<FeeSchedule> FeeSchedules => Set<FeeSchedule>();

    protected override string Schema => Schemas.Fees;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new FeeScheduleConfiguration());
    }
}
