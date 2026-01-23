using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.Orders.Domain.Orders;
using ModularTemplate.Modules.Orders.Domain.Orders.Events;
using ModularTemplate.Modules.Orders.IntegrationEvents;

namespace ModularTemplate.Modules.Orders.Application.Orders.PlaceOrder;

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

        await eventBus.PublishAsync(
            new OrderPlacedIntegrationEvent(
                Guid.NewGuid(),
                dateTimeProvider.UtcNow,
                order.Id,
                order.ProductId,
                order.Quantity,
                order.TotalPrice,
                order.Status.ToString(),
                order.OrderedAtUtc),
            cancellationToken);
    }
}
