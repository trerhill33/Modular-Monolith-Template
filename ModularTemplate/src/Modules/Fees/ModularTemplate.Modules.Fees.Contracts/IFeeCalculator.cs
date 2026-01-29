namespace ModularTemplate.Modules.Fees.Contracts;

/// <summary>
/// Public contract for fee calculation that other modules can consume.
/// Implemented by the Fees module Infrastructure layer.
/// </summary>
public interface IFeeCalculator
{
    /// <summary>
    /// Calculates fees based on the provided request parameters.
    /// </summary>
    /// <param name="request">The fee calculation request containing base amount and fee type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the total fee and breakdown of individual fees.</returns>
    Task<FeeCalculationResult> CalculateAsync(
        FeeCalculationRequest request,
        CancellationToken cancellationToken = default);
}
