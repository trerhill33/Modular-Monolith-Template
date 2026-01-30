using Rtl.Core.Domain.Results;

namespace Rtl.Module.SampleSales.Domain.Products;

public static class ProductErrors
{
    public static Error NotFound(Guid productId) =>
        Error.NotFound("Products.NotFound", $"The product with ID '{productId}' was not found.");

    public static readonly Error NameEmpty =
        Error.Validation("Products.NameEmpty", "The product name cannot be empty.");

    public static readonly Error NameTooLong =
        Error.Validation("Products.NameTooLong", "The product name cannot exceed 200 characters.");

    public static readonly Error PriceInvalid =
        Error.Validation("Products.PriceInvalid", "The product price must be greater than zero.");
}
