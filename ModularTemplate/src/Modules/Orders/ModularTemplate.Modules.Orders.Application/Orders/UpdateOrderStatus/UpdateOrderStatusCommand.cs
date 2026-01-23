using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Orders.Domain.Orders;

namespace ModularTemplate.Modules.Orders.Application.Orders.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus) : ICommand;
