using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Inbox.Handlers;
using ModularTemplate.Modules.SampleOrders.Domain;
using ModularTemplate.Modules.SampleOrders.Infrastructure.Persistence;

namespace ModularTemplate.Modules.SampleOrders.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory<ISampleOrdersModule> dbConnectionFactory)
    : IdempotentIntegrationEventHandlerBase<TIntegrationEvent, ISampleOrdersModule>(decorated, dbConnectionFactory)
    where TIntegrationEvent : IIntegrationEvent
{
    protected override string Schema => Schemas.Orders;
}
