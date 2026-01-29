using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;
