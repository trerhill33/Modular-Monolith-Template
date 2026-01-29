using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.SampleOrders.Domain.Orders;
using ModularTemplate.Modules.SampleOrders.Domain.Orders.Events;
using ModularTemplate.Modules.SampleOrders.IntegrationEvents;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.PlaceOrder;

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
