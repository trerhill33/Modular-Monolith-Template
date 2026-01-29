using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Events;
using ModularTemplate.Common.Infrastructure.Outbox.Handler;
using ModularTemplate.Modules.SampleOrders.Domain;
using ModularTemplate.Modules.SampleOrders.Infrastructure.Persistence;

namespace ModularTemplate.Modules.SampleOrders.Infrastructure.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    IDbConnectionFactory<ISampleOrdersModule> dbConnectionFactory)
    : IdempotentDomainEventHandlerBase<TDomainEvent, ISampleOrdersModule>(decorated, dbConnectionFactory)
    where TDomainEvent : IDomainEvent
{
    protected override string Schema => Schemas.Orders;
}
