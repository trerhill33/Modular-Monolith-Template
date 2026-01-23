using ModularTemplate.Modules.Orders.Domain.Orders;

namespace ModularTemplate.Modules.Orders.Application.Orders.GetOrder;

public sealed record OrderResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice,
    OrderStatus Status,
    DateTime OrderedAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedByUserId,
    DateTime? ModifiedAtUtc,
    Guid? ModifiedByUserId);
