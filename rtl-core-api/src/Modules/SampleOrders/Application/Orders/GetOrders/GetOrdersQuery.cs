using Rtl.Core.Application.Messaging;
using Rtl.Module.SampleOrders.Application.Orders.GetOrder;

namespace Rtl.Module.SampleOrders.Application.Orders.GetOrders;

public sealed record GetOrdersQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<OrderResponse>>;
