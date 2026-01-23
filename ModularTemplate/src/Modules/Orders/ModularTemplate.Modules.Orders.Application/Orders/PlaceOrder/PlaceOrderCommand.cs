using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Orders.Application.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    Guid ProductId,
    int Quantity) : ICommand<Guid>;
