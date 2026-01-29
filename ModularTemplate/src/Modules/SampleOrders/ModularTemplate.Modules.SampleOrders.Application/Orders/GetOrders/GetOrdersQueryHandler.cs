using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrder;
using ModularTemplate.Modules.SampleOrders.Domain.Orders;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrdersQuery, IReadOnlyCollection<OrderResponse>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Order> orders = await orderRepository.GetAllAsync(
            request.Limit,
            cancellationToken);

        var response = orders.Select(o => new OrderResponse(
            o.Id,
            o.CustomerId,
            o.Lines.Select(l => new OrderLineResponse(
                l.Id,
                l.ProductId,
                l.Quantity,
                l.UnitPrice.Amount,
                l.UnitPrice.Currency,
                l.LineTotal.Amount)).ToList(),
            o.TotalPrice.Amount,
            o.TotalPrice.Currency,
            o.Status,
            o.OrderedAtUtc,
            o.CreatedAtUtc,
            o.CreatedByUserId,
            o.ModifiedAtUtc,
            o.ModifiedByUserId)).ToList();

        return response;
    }
}
