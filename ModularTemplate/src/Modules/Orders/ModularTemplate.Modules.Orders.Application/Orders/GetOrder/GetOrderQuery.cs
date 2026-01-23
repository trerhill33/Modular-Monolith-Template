using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Orders.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;
