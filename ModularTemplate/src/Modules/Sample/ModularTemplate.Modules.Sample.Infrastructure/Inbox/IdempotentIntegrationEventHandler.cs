using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Handlers;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence;

namespace ModularTemplate.Modules.Sample.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory<ISampleModule> dbConnectionFactory)
    : IdempotentIntegrationEventHandlerBase<TIntegrationEvent, ISampleModule>(decorated, dbConnectionFactory)
    where TIntegrationEvent : IIntegrationEvent
{
    protected override string Schema => Schemas.Sample;
}
