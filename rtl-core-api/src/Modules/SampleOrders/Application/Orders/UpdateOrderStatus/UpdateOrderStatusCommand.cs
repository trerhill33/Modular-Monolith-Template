using Rtl.Core.Application.Messaging;
using Rtl.Module.SampleOrders.Domain.Orders;

namespace Rtl.Module.SampleOrders.Application.Orders.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus) : ICommand;
