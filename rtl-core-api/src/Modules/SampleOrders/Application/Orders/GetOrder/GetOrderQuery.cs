using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleOrders.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;
