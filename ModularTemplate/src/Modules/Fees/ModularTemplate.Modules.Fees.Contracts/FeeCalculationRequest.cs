namespace ModularTemplate.Modules.Fees.Contracts;

/// <summary>
/// Request for fee calculation containing all necessary parameters.
/// </summary>
/// <param name="CustomerId">Optional customer ID for customer-specific fee rules.</param>
/// <param name="FeeType">The type of fee to calculate (e.g., "OrderProcessing", "Shipping", "ServiceFee").</param>
/// <param name="BaseAmount">The base amount to calculate fees on (e.g., order subtotal).</param>
/// <param name="Currency">Optional currency code (defaults to system default if not specified).</param>
/// <param name="LineItems">Optional line items for itemized fee calculation.</param>
public sealed record FeeCalculationRequest(
    Guid? CustomerId,
    string FeeType,
    decimal BaseAmount,
    string? Currency = null,
    IReadOnlyList<FeeLineItem>? LineItems = null);

/// <summary>
/// Represents a line item for itemized fee calculation.
/// </summary>
/// <param name="ProductType">The type/category of the product.</param>
/// <param name="Amount">The unit price or amount.</param>
/// <param name="Quantity">The quantity of items.</param>
public sealed record FeeLineItem(
    string ProductType,
    decimal Amount,
    int Quantity);
