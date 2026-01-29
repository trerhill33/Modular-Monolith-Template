using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrder;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrders;

public sealed record GetOrdersQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<OrderResponse>>;
