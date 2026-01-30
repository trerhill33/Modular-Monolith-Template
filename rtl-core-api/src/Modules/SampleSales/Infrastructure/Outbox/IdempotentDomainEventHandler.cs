using Rtl.Core.Application.Messaging;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain.Events;
using Rtl.Core.Infrastructure.Outbox.Handler;
using Rtl.Module.SampleSales.Domain;
using Rtl.Module.SampleSales.Infrastructure.Persistence;

namespace Rtl.Module.SampleSales.Infrastructure.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    IDbConnectionFactory<ISampleSalesModule> dbConnectionFactory)
    : IdempotentDomainEventHandlerBase<TDomainEvent, ISampleSalesModule>(decorated, dbConnectionFactory)
    where TDomainEvent : IDomainEvent
{
    protected override string Schema => Schemas.Sample;
}
