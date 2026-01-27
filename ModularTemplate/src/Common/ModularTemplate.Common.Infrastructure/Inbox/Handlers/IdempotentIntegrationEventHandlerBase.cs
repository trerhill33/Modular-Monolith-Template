using Dapper;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Data;
using System.Data.Common;

namespace ModularTemplate.Common.Infrastructure.Inbox.Handlers;

/// <summary>
/// Base decorator that ensures idempotent integration event handling by tracking consumed messages.
/// </summary>
/// <typeparam name="TIntegrationEvent">The type of integration event.</typeparam>
/// <typeparam name="TModule">The module marker interface type.</typeparam>
/// <remarks>
/// <para>
/// This decorator wraps an integration event handler and ensures that the same event is not
/// processed twice by the same handler. It uses a database table to track which handlers
/// have already consumed a given message.
/// </para>
/// <para>
/// Module-specific implementations only need to provide the database schema where the
/// inbox_message_consumers table resides.
/// </para>
/// </remarks>
public abstract class IdempotentIntegrationEventHandlerBase<TIntegrationEvent, TModule> : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
    where TModule : class
{
    private readonly IIntegrationEventHandler<TIntegrationEvent> _decorated;
    private readonly IDbConnectionFactory<TModule> _dbConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotentIntegrationEventHandlerBase{TIntegrationEvent, TModule}"/> class.
    /// </summary>
    /// <param name="decorated">The inner handler to decorate with idempotency.</param>
    /// <param name="dbConnectionFactory">Factory for creating database connections.</param>
    protected IdempotentIntegrationEventHandlerBase(
        IIntegrationEventHandler<TIntegrationEvent> decorated,
        IDbConnectionFactory<TModule> dbConnectionFactory)
    {
        _decorated = decorated;
        _dbConnectionFactory = dbConnectionFactory;
    }

    /// <summary>
    /// Gets the database schema where the inbox_message_consumers table resides.
    /// </summary>
    protected abstract string Schema { get; }

    /// <summary>
    /// Handles the integration event with idempotency checking.
    /// </summary>
    /// <param name="integrationEvent">The integration event to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public override async Task HandleAsync(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, _decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }

        await _decorated.HandleAsync(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
    }

    private async Task<bool> InboxConsumerExistsAsync(
        DbConnection dbConnection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        string sql =
            $"""
            SELECT EXISTS(
                SELECT 1
                FROM {Schema}.inbox_message_consumers
                WHERE inbox_message_id = @InboxMessageId AND
                      name = @Name
            )
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }

    private async Task InsertInboxConsumerAsync(
        DbConnection dbConnection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        string sql =
            $"""
            INSERT INTO {Schema}.inbox_message_consumers(inbox_message_id, name)
            VALUES (@InboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
