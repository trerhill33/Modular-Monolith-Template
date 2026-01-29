using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Fees.Domain.FeeSchedules;
using System.Linq.Expressions;

namespace ModularTemplate.Modules.Fees.Infrastructure.Persistence.Repositories;

internal sealed class FeeScheduleRepository(FeesDbContext dbContext)
    : Repository<FeeSchedule, Guid, FeesDbContext>(dbContext), IFeeScheduleRepository
{
    protected override Expression<Func<FeeSchedule, Guid>> IdSelector => entity => entity.Id;

    public async Task<IReadOnlyList<FeeSchedule>> GetActiveSchedulesAsync(
        string feeCategory,
        DateTime effectiveDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(fs => fs.IsActive &&
                         fs.FeeCategory == feeCategory &&
                         fs.EffectiveFrom <= effectiveDate &&
                         (!fs.EffectiveTo.HasValue || fs.EffectiveTo.Value >= effectiveDate))
            .OrderBy(fs => fs.Name)
            .ToListAsync(cancellationToken);
    }
}
