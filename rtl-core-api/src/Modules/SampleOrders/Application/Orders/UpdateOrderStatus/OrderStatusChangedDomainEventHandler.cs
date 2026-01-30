using Rtl.Core.Application.EventBus;
using Rtl.Core.Application.Messaging;
using Rtl.Core.Domain;
using Rtl.Module.SampleOrders.Domain.Orders.Events;
using Rtl.Module.SampleOrders.IntegrationEvents;

namespace Rtl.Module.SampleOrders.Application.Orders.UpdateOrderStatus;

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
