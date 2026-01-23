using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Modules.Orders.Domain.Orders;

public static class OrderErrors
{
    public static Error NotFound(Guid orderId) =>
        Error.NotFound("Orders.NotFound", $"The order with ID '{orderId}' was not found.");

    public static readonly Error ProductNotFound =
        Error.Validation("Orders.ProductNotFound", "The product for this order was not found.");

    public static readonly Error QuantityInvalid =
        Error.Validation("Orders.QuantityInvalid", "The order quantity must be greater than zero.");

    public static readonly Error InvalidStatusTransition =
        Error.Validation("Orders.InvalidStatusTransition", "The status transition is not allowed.");
}
