using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Handlers;
using ModularTemplate.Modules.Orders.Domain;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;

namespace ModularTemplate.Modules.Orders.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory<IOrdersModule> dbConnectionFactory)
    : IdempotentIntegrationEventHandlerBase<TIntegrationEvent, IOrdersModule>(decorated, dbConnectionFactory)
    where TIntegrationEvent : IIntegrationEvent
{
    protected override string Schema => Schemas.Orders;
}
