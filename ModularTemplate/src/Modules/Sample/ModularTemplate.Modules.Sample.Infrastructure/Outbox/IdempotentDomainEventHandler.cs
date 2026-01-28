using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Events;
using ModularTemplate.Common.Infrastructure.Outbox.Handler;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence;

namespace ModularTemplate.Modules.Sample.Infrastructure.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    IDbConnectionFactory<ISampleModule> dbConnectionFactory)
    : IdempotentDomainEventHandlerBase<TDomainEvent, ISampleModule>(decorated, dbConnectionFactory)
    where TDomainEvent : IDomainEvent
{
    protected override string Schema => Schemas.Sample;
}
