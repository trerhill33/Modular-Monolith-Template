using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Fees.Domain.FeeSchedules;

public interface IFeeScheduleRepository : IRepository<FeeSchedule, Guid>
{
    /// <summary>
    /// Gets all active fee schedules for a specific category that are effective at the given date.
    /// </summary>
    /// <param name="feeCategory">The fee category to filter by (e.g., "OrderProcessing").</param>
    /// <param name="effectiveDate">The date to check effectiveness against.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active and effective fee schedules.</returns>
    Task<IReadOnlyList<FeeSchedule>> GetActiveSchedulesAsync(
        string feeCategory,
        DateTime effectiveDate,
        CancellationToken cancellationToken = default);
}
