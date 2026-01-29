using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.SampleOrders.Domain.Orders;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus) : ICommand;
