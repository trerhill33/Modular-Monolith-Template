using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Modules.Fees.Domain.FeeSchedules;

public static class FeeScheduleErrors
{
    public static readonly Error NameEmpty = Error.Validation(
        "FeeSchedule.NameEmpty",
        "Fee schedule name cannot be empty.");

    public static readonly Error NameTooLong = Error.Validation(
        "FeeSchedule.NameTooLong",
        "Fee schedule name cannot exceed 200 characters.");

    public static readonly Error FeeCategoryEmpty = Error.Validation(
        "FeeSchedule.FeeCategoryEmpty",
        "Fee category cannot be empty.");

    public static readonly Error FeeTypeEmpty = Error.Validation(
        "FeeSchedule.FeeTypeEmpty",
        "Fee type cannot be empty.");

    public static readonly Error RateNegative = Error.Validation(
        "FeeSchedule.RateNegative",
        "Fee rate cannot be negative.");

    public static readonly Error InvalidEffectiveDateRange = Error.Validation(
        "FeeSchedule.InvalidEffectiveDateRange",
        "Effective end date must be after start date.");

    public static readonly Error InvalidAmountRange = Error.Validation(
        "FeeSchedule.InvalidAmountRange",
        "Minimum amount cannot be greater than maximum amount.");

    public static readonly Error AlreadyInactive = Error.Conflict(
        "FeeSchedule.AlreadyInactive",
        "Fee schedule is already inactive.");

    public static readonly Error AlreadyActive = Error.Conflict(
        "FeeSchedule.AlreadyActive",
        "Fee schedule is already active.");

    public static Error NotFound(Guid id) => Error.NotFound(
        "FeeSchedule.NotFound",
        $"Fee schedule with ID '{id}' was not found.");
}
