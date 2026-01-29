using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Inbox.Handlers;
using ModularTemplate.Modules.SampleSales.Domain;
using ModularTemplate.Modules.SampleSales.Infrastructure.Persistence;

namespace ModularTemplate.Modules.SampleSales.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory<ISampleSalesModule> dbConnectionFactory)
    : IdempotentIntegrationEventHandlerBase<TIntegrationEvent, ISampleSalesModule>(decorated, dbConnectionFactory)
    where TIntegrationEvent : IIntegrationEvent
{
    protected override string Schema => Schemas.Sample;
}
