using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.Fees.Contracts;
using ModularTemplate.Modules.Fees.Domain.FeeSchedules;

namespace ModularTemplate.Modules.Fees.Infrastructure.Services;

/// <summary>
/// Implementation of IFeeCalculator that calculates fees based on fee schedules stored in the database.
/// </summary>
internal sealed class FeeCalculator(
    IFeeScheduleRepository feeScheduleRepository,
    IDateTimeProvider dateTimeProvider) : IFeeCalculator
{
    public async Task<FeeCalculationResult> CalculateAsync(
        FeeCalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FeeType))
        {
            return FeeCalculationResult.Failed("Fee type is required.");
        }

        if (request.BaseAmount < 0)
        {
            return FeeCalculationResult.Failed("Base amount cannot be negative.");
        }

        try
        {
            var schedules = await feeScheduleRepository.GetActiveSchedulesAsync(
                request.FeeType,
                dateTimeProvider.UtcNow,
                cancellationToken);

            if (schedules.Count == 0)
            {
                // No fees configured for this type - return zero fees
                return FeeCalculationResult.Successful(0, []);
            }

            var breakdown = new List<FeeBreakdown>();
            decimal totalFee = 0;

            foreach (var schedule in schedules)
            {
                var feeAmount = schedule.CalculateFee(request.BaseAmount);

                breakdown.Add(new FeeBreakdown(
                    schedule.Name,
                    schedule.FeeType,
                    feeAmount,
                    $"Rate: {schedule.Rate:N2}" + (schedule.FeeType.Equals("Percentage", StringComparison.OrdinalIgnoreCase) ? "%" : "")));

                totalFee += feeAmount;
            }

            return FeeCalculationResult.Successful(Math.Round(totalFee, 2), breakdown);
        }
        catch (Exception ex)
        {
            return FeeCalculationResult.Failed($"Fee calculation failed: {ex.Message}");
        }
    }
}
