using Rtl.Core.Application.EventBus;
using Rtl.Core.Application.Messaging;
using Rtl.Core.Domain;
using Rtl.Module.SampleOrders.Domain.Orders;
using Rtl.Module.SampleOrders.Domain.Orders.Events;
using Rtl.Module.SampleOrders.IntegrationEvents;

namespace Rtl.Module.SampleOrders.Application.Orders.PlaceOrder;

internal sealed class OrderPlacedDomainEventHandler(
    IOrderRepository orderRepository,
    IEventBus eventBus,
    IDateTimeProvider dateTimeProvider) : DomainEventHandler<OrderPlacedDomainEvent>
{
    public override async Task Handle(
        OrderPlacedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Order? order = await orderRepository.GetByIdAsync(
            domainEvent.OrderId,
            cancellationToken);

        if (order is null)
        {
            return;
        }

        var lines = order.Lines.Select(l => new OrderLineDto(
            l.ProductId,
            l.Quantity,
            l.UnitPrice.Amount,
            l.UnitPrice.Currency)).ToList();

        await eventBus.PublishAsync(
            new OrderPlacedIntegrationEvent(
                Guid.NewGuid(),
                dateTimeProvider.UtcNow,
                order.Id,
                order.CustomerId,
                lines,
                order.TotalPrice.Amount,
                order.TotalPrice.Currency,
                order.Status.ToString(),
                order.OrderedAtUtc),
            cancellationToken);
    }
}
