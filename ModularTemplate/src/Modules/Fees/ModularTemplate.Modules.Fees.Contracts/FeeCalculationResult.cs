namespace ModularTemplate.Modules.Fees.Contracts;

/// <summary>
/// Result of a fee calculation containing total fee and breakdown.
/// </summary>
/// <param name="Success">Whether the fee calculation was successful.</param>
/// <param name="TotalFee">The total calculated fee amount.</param>
/// <param name="Breakdown">Breakdown of individual fees that make up the total.</param>
/// <param name="ErrorMessage">Error message if calculation failed.</param>
public sealed record FeeCalculationResult(
    bool Success,
    decimal TotalFee,
    IReadOnlyList<FeeBreakdown> Breakdown,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Creates a successful result with the specified total and breakdown.
    /// </summary>
    public static FeeCalculationResult Successful(decimal totalFee, IReadOnlyList<FeeBreakdown> breakdown)
        => new(true, totalFee, breakdown);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    public static FeeCalculationResult Failed(string errorMessage)
        => new(false, 0, [], errorMessage);
}

/// <summary>
/// Represents a single fee in the fee breakdown.
/// </summary>
/// <param name="FeeName">The name of this fee (e.g., "Processing Fee", "Service Charge").</param>
/// <param name="FeeType">The calculation type (e.g., "Percentage", "Flat", "Tiered").</param>
/// <param name="Amount">The calculated fee amount.</param>
/// <param name="Description">Optional description of this fee.</param>
public sealed record FeeBreakdown(
    string FeeName,
    string FeeType,
    decimal Amount,
    string? Description = null);
