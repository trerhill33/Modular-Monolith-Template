using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Events;
using ModularTemplate.Common.Infrastructure.Outbox.Handler;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence;

namespace ModularTemplate.Modules.Sales.Infrastructure.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    IDbConnectionFactory dbConnectionFactory)
    : IdempotentDomainEventHandlerBase<TDomainEvent>(decorated, dbConnectionFactory)
    where TDomainEvent : IDomainEvent
{
    protected override string Schema => Schemas.Sales;
}
