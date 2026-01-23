using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders.GetAllOrders;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders.GetOrderById;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders.PlaceOrder;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders.UpdateOrderStatus;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders;

internal sealed class OrdersEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        new GetAllOrdersEndpoint(),
        new GetOrderByIdEndpoint(),
        new PlaceOrderEndpoint(),
        new UpdateOrderStatusEndpoint()
    ];
}
