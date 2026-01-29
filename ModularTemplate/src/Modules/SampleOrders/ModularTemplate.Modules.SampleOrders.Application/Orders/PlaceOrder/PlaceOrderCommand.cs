using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : ICommand<Guid>;
