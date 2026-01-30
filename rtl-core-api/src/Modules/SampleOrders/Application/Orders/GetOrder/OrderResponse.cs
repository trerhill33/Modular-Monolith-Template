using Rtl.Module.SampleOrders.Domain.Orders;

namespace Rtl.Module.SampleOrders.Application.Orders.GetOrder;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    IReadOnlyCollection<OrderLineResponse> Lines,
    decimal TotalPrice,
    string Currency,
    OrderStatus Status,
    DateTime OrderedAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedByUserId,
    DateTime? ModifiedAtUtc,
    Guid? ModifiedByUserId);

public sealed record OrderLineResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    string Currency,
    decimal LineTotal);
