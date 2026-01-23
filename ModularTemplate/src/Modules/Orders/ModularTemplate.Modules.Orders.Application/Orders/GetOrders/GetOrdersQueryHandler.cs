using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Orders.Application.Orders.GetOrder;
using ModularTemplate.Modules.Orders.Domain.Orders;

namespace ModularTemplate.Modules.Orders.Application.Orders.GetOrders;

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
            o.ProductId,
            o.Quantity,
            o.TotalPrice,
            o.Status,
            o.OrderedAtUtc,
            o.CreatedAtUtc,
            o.CreatedByUserId,
            o.ModifiedAtUtc,
            o.ModifiedByUserId)).ToList();

        return response;
    }
}
