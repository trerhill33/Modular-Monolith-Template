using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.SampleOrders.Domain.Orders.Events;
using ModularTemplate.Modules.SampleOrders.IntegrationEvents;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.UpdateOrderStatus;

internal sealed class OrderStatusChangedDomainEventHandler(
    IEventBus eventBus,
    IDateTimeProvider dateTimeProvider) : DomainEventHandler<OrderStatusChangedDomainEvent>
{
    public override async Task Handle(
        OrderStatusChangedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new OrderStatusChangedIntegrationEvent(
                Guid.NewGuid(),
                dateTimeProvider.UtcNow,
                domainEvent.OrderId,
                domainEvent.NewStatus.ToString()),
            cancellationToken);
    }
}
