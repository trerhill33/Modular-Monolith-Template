using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleOrders.Domain.Orders;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrder;

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

        var lines = order.Lines.Select(l => new OrderLineResponse(
            l.Id,
            l.ProductId,
            l.Quantity,
            l.UnitPrice.Amount,
            l.UnitPrice.Currency,
            l.LineTotal.Amount)).ToList();

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            lines,
            order.TotalPrice.Amount,
            order.TotalPrice.Currency,
            order.Status,
            order.OrderedAtUtc,
            order.CreatedAtUtc,
            order.CreatedByUserId,
            order.ModifiedAtUtc,
            order.ModifiedByUserId);
    }
}
