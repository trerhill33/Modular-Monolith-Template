using ModularTemplate.Common.Domain.Entities;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Fees.Domain.FeeSchedules.Events;

namespace ModularTemplate.Modules.Fees.Domain.FeeSchedules;

/// <summary>
/// Represents a fee schedule that defines how fees are calculated.
/// </summary>
public sealed class FeeSchedule : AuditableEntity, IAggregateRoot
{
    private const int MaxNameLength = 200;

    private FeeSchedule()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>
    /// Display name for this fee schedule.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// The category of fee (e.g., "OrderProcessing", "Shipping", "ServiceFee").
    /// </summary>
    public string FeeCategory { get; private set; } = string.Empty;

    /// <summary>
    /// The calculation type (e.g., "Percentage", "Flat", "Tiered").
    /// </summary>
    public string FeeType { get; private set; } = string.Empty;

    /// <summary>
    /// The rate for calculation (percentage value or flat amount).
    /// </summary>
    public decimal Rate { get; private set; }

    /// <summary>
    /// Minimum fee amount (for percentage-based fees).
    /// </summary>
    public decimal? MinAmount { get; private set; }

    /// <summary>
    /// Maximum fee amount (for percentage-based fees).
    /// </summary>
    public decimal? MaxAmount { get; private set; }

    /// <summary>
    /// Whether this fee schedule is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// When this fee schedule becomes effective.
    /// </summary>
    public DateTime EffectiveFrom { get; private set; }

    /// <summary>
    /// When this fee schedule expires (null for no expiration).
    /// </summary>
    public DateTime? EffectiveTo { get; private set; }

    public static Result<FeeSchedule> Create(
        string name,
        string feeCategory,
        string feeType,
        decimal rate,
        DateTime effectiveFrom,
        DateTime? effectiveTo = null,
        decimal? minAmount = null,
        decimal? maxAmount = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.NameEmpty);
        }

        if (name.Length > MaxNameLength)
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.NameTooLong);
        }

        if (string.IsNullOrWhiteSpace(feeCategory))
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.FeeCategoryEmpty);
        }

        if (string.IsNullOrWhiteSpace(feeType))
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.FeeTypeEmpty);
        }

        if (rate < 0)
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.RateNegative);
        }

        if (effectiveTo.HasValue && effectiveTo.Value <= effectiveFrom)
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.InvalidEffectiveDateRange);
        }

        if (minAmount.HasValue && maxAmount.HasValue && minAmount.Value > maxAmount.Value)
        {
            return Result.Failure<FeeSchedule>(FeeScheduleErrors.InvalidAmountRange);
        }

        var feeSchedule = new FeeSchedule
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            FeeCategory = feeCategory.Trim(),
            FeeType = feeType.Trim(),
            Rate = rate,
            MinAmount = minAmount,
            MaxAmount = maxAmount,
            IsActive = true,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo
        };

        feeSchedule.Raise(new FeeScheduleCreatedDomainEvent(feeSchedule.Id));

        return feeSchedule;
    }

    public Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(FeeScheduleErrors.AlreadyInactive);
        }

        IsActive = false;
        Raise(new FeeScheduleDeactivatedDomainEvent(Id));

        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(FeeScheduleErrors.AlreadyActive);
        }

        IsActive = true;
        Raise(new FeeScheduleActivatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Checks if this fee schedule is effective at the given date.
    /// </summary>
    public bool IsEffectiveAt(DateTime date)
    {
        return IsActive &&
               date >= EffectiveFrom &&
               (!EffectiveTo.HasValue || date <= EffectiveTo.Value);
    }

    /// <summary>
    /// Calculates the fee amount based on this schedule and the base amount.
    /// </summary>
    public decimal CalculateFee(decimal baseAmount)
    {
        decimal fee = FeeType.ToLowerInvariant() switch
        {
            "percentage" => baseAmount * (Rate / 100),
            "flat" => Rate,
            _ => 0
        };

        // Apply min/max constraints
        if (MinAmount.HasValue && fee < MinAmount.Value)
        {
            fee = MinAmount.Value;
        }

        if (MaxAmount.HasValue && fee > MaxAmount.Value)
        {
            fee = MaxAmount.Value;
        }

        return Math.Round(fee, 2);
    }
}
