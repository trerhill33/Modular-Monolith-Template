using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Domain.Entities;
using ModularTemplate.Common.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace ModularTemplate.Common.Infrastructure.Outbox.Data;

/// <summary>
/// EF Core interceptor that captures domain events and inserts them into the outbox.
/// </summary>
public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<InsertOutboxMessagesInterceptor> _logger;

    public InsertOutboxMessagesInterceptor(ILogger<InsertOutboxMessagesInterceptor> logger)
    {
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _logger.LogDebug("[OutboxInterceptor] SavingChanges (sync) triggered");

        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[OutboxInterceptor] SavingChangesAsync triggered");

        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void InsertOutboxMessages(DbContext context)
    {
        var entities = context
            .ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .ToList();

        _logger.LogDebug("[OutboxInterceptor] Found {EntityCount} entities in change tracker", entities.Count);

        var outboxMessages = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents;
                var eventCount = domainEvents.Count;

                _logger.LogDebug("[OutboxInterceptor] Entity {EntityType} has {EventCount} domain events",
                    entity.GetType().Name, eventCount);

                entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent =>
            {
                _logger.LogDebug("[OutboxInterceptor] Creating OutboxMessage for event {EventType}, Id={EventId}, OccurredOnUtc={OccurredOnUtc}",
                    domainEvent.GetType().Name, domainEvent.Id, domainEvent.OccurredOnUtc);

                return new OutboxMessage
                {
                    Id = domainEvent.Id,
                    Type = domainEvent.GetType().Name,
                    Content = JsonConvert.SerializeObject(domainEvent, SerializerSettings.Instance),
                    OccurredOnUtc = domainEvent.OccurredOnUtc
                };
            })
            .ToList();

        _logger.LogDebug("[OutboxInterceptor] Adding {OutboxCount} outbox messages to context", outboxMessages.Count);

        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
