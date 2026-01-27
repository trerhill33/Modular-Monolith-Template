using Dapper;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Events;
using ModularTemplate.Common.Infrastructure.Outbox.Data;
using System.Data.Common;

namespace ModularTemplate.Common.Infrastructure.Outbox.Handler;

/// <summary>
/// Base decorator that ensures idempotent domain event handling by tracking consumed messages.
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event.</typeparam>
/// <typeparam name="TModule">The module marker interface type.</typeparam>
/// <remarks>
/// <para>
/// This decorator wraps a domain event handler and ensures that the same event is not
/// processed twice by the same handler. It uses a database table to track which handlers
/// have already consumed a given message.
/// </para>
/// <para>
/// Module-specific implementations only need to provide the database schema where the
/// outbox_message_consumers table resides.
/// </para>
/// </remarks>
public abstract class IdempotentDomainEventHandlerBase<TDomainEvent, TModule> : DomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
    where TModule : class
{
    private readonly IDomainEventHandler<TDomainEvent> _decorated;
    private readonly IDbConnectionFactory<TModule> _dbConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotentDomainEventHandlerBase{TDomainEvent, TModule}"/> class.
    /// </summary>
    /// <param name="decorated">The inner handler to decorate with idempotency.</param>
    /// <param name="dbConnectionFactory">Factory for creating database connections.</param>
    protected IdempotentDomainEventHandlerBase(
        IDomainEventHandler<TDomainEvent> decorated,
        IDbConnectionFactory<TModule> dbConnectionFactory)
    {
        _decorated = decorated;
        _dbConnectionFactory = dbConnectionFactory;
    }

    /// <summary>
    /// Gets the database schema where the outbox_message_consumers table resides.
    /// </summary>
    protected abstract string Schema { get; }

    /// <summary>
    /// Handles the domain event with idempotency checking.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public override async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dbConnectionFactory.OpenConnectionAsync();

        var outboxMessageConsumer = new OutboxMessageConsumer(domainEvent.Id, _decorated.GetType().Name);

        if (await OutboxConsumerExistsAsync(connection, outboxMessageConsumer))
        {
            return;
        }

        await _decorated.Handle(domainEvent, cancellationToken);

        await InsertOutboxConsumerAsync(connection, outboxMessageConsumer);
    }

    private async Task<bool> OutboxConsumerExistsAsync(
        DbConnection dbConnection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        var sql =
            $"""
            SELECT EXISTS(
                SELECT 1
                FROM {Schema}.outbox_message_consumers
                WHERE outbox_message_id = @OutboxMessageId AND
                      name = @Name
            )
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, outboxMessageConsumer);
    }

    private async Task InsertOutboxConsumerAsync(
        DbConnection dbConnection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        var sql =
            $"""
            INSERT INTO {Schema}.outbox_message_consumers(outbox_message_id, name)
            VALUES (@OutboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, outboxMessageConsumer);
    }
}
