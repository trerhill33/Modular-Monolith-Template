using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Orders.Application.Orders.GetOrder;

namespace ModularTemplate.Modules.Orders.Application.Orders.GetOrders;

public sealed record GetOrdersQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<OrderResponse>>;
