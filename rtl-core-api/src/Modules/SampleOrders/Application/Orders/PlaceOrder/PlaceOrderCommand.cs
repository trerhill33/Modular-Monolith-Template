using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleOrders.Application.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : ICommand<Guid>;
