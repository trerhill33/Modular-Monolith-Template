using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Orders.Domain.Orders;

namespace ModularTemplate.Modules.Orders.Application.Orders.GetOrder;

internal sealed class GetOrderQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result.Failure<OrderResponse>(OrderErrors.NotFound(request.OrderId));
        }

        return new OrderResponse(
            order.Id,
            order.ProductId,
            order.Quantity,
            order.TotalPrice,
            order.Status,
            order.OrderedAtUtc,
            order.CreatedAtUtc,
            order.CreatedByUserId,
            order.ModifiedAtUtc,
            order.ModifiedByUserId);
    }
}
