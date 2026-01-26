using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Features;
using ModularTemplate.Common.Infrastructure.Inbox.Handlers;
using ModularTemplate.Common.Infrastructure.Messaging;
using ModularTemplate.Common.Infrastructure.Serialization;
using Newtonsoft.Json;
using Quartz;
using System.Data;
using System.Reflection;

namespace ModularTemplate.Common.Infrastructure.Inbox.Job;

/// <summary>
/// Base class for Quartz jobs that process inbox messages and invoke integration event handlers.
/// </summary>
/// <remarks>
/// <para>
/// This abstract base class contains all the generic processing logic for the inbox pattern,
/// including message retrieval, handler invocation, retry logic with exponential backoff,
/// and dead-letter handling.
/// </para>
/// <para>
/// Module-specific implementations only need to provide the module name, database schema,
/// and the assembly containing the integration event handlers.
/// </para>
/// <para>
/// Processing can be disabled via the <see cref="InfrastructureFeatures.Inbox"/> feature flag.
/// When disabled, messages remain queued and will be processed when the feature is re-enabled.
/// </para>
/// </remarks>
[DisallowConcurrentExecution]
public abstract class ProcessInboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger logger) : IJob
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;
    private readonly IFeatureFlagService _featureFlagService = featureFlagService;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Gets the name of the module this job processes messages for.
    /// </summary>
    /// <remarks>Used for logging and identification purposes.</remarks>
    protected abstract string ModuleName { get; }

    /// <summary>
    /// Gets the database schema where the inbox tables reside.
    /// </summary>
    /// <example>"sample", "orders", "users"</example>
    protected abstract string Schema { get; }

    /// <summary>
    /// Gets the assembly containing the integration event handlers to invoke.
    /// </summary>
    protected abstract Assembly HandlersAssembly { get; }

    /// <summary>
    /// Executes the inbox processing job.
    /// </summary>
    /// <remarks>
    /// If the <see cref="InfrastructureFeatures.Inbox"/> feature flag is disabled,
    /// the job will skip processing. Messages remain in the inbox and will be
    /// processed when the feature is re-enabled.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        if (!_featureFlagService.IsEnabled(InfrastructureFeatures.Inbox))
        {
            _logger.LogDebug(
                "{Module} - Inbox processing is disabled via feature flag. Messages will remain queued.",
                ModuleName);
            return;
        }

        _logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);

        await using var connection = await _dbConnectionFactory.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync(connection, transaction);

        foreach (InboxMessageResponse inboxMessage in inboxMessages)
        {
            Exception? exception = null;

            try
            {
                // Check cancellation before potentially long deserialization
                context.CancellationToken.ThrowIfCancellationRequested();

                var integrationEventType = Type.GetType(inboxMessage.Type)!;
                var integrationEvent = (IIntegrationEvent)JsonConvert.DeserializeObject(
                    inboxMessage.Content,
                    integrationEventType,
                    SerializerSettings.Instance)!;

                using var scope = _serviceScopeFactory.CreateScope();

                var handlers = IntegrationEventHandlersFactory.GetHandlers(
                    integrationEvent.GetType(),
                    scope.ServiceProvider,
                    HandlersAssembly);

                foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                {
                    await integrationEventHandler.HandleAsync(integrationEvent, context.CancellationToken);
                }
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing inbox message {MessageId}",
                    ModuleName,
                    inboxMessage.Id);

                exception = caughtException;
            }

            await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
        }

        await transaction.CommitAsync();

        _logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql =
            $"""
             SELECT
                id AS {nameof(InboxMessageResponse.Id)},
                type AS {nameof(InboxMessageResponse.Type)},
                content AS {nameof(InboxMessageResponse.Content)},
                retry_count AS {nameof(InboxMessageResponse.RetryCount)}
             FROM {Schema}.inbox_messages
             WHERE processed_on_utc IS NULL
               AND (next_retry_at_utc IS NULL OR next_retry_at_utc <= @Now)
             ORDER BY occurred_on_utc
             LIMIT {_inboxOptions.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<InboxMessageResponse> inboxMessages = await connection.QueryAsync<InboxMessageResponse>(
            sql,
            new { Now = _dateTimeProvider.UtcNow },
            transaction: transaction);

        return inboxMessages.ToList();
    }

    private async Task UpdateInboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        InboxMessageResponse inboxMessage,
        Exception? exception)
    {
        var now = _dateTimeProvider.UtcNow;

        if (exception is null)
        {
            // Success: mark as processed
            var successSql =
                $"""
                UPDATE {Schema}.inbox_messages
                SET processed_on_utc = @ProcessedOnUtc,
                    error = NULL
                WHERE id = @Id
                """;

            await connection.ExecuteAsync(
                successSql,
                new { inboxMessage.Id, ProcessedOnUtc = now },
                transaction: transaction);
        }
        else
        {
            var newRetryCount = inboxMessage.RetryCount + 1;

            if (newRetryCount >= _inboxOptions.MaxRetries)
            {
                // Dead letter: mark as processed with error
                _logger.LogError(
                    "Message {MessageId} moved to dead letter after {Retries} retries",
                    inboxMessage.Id,
                    newRetryCount);

                var deadLetterSql =
                    $"""
                    UPDATE {Schema}.inbox_messages
                    SET processed_on_utc = @ProcessedOnUtc,
                        error = @Error,
                        retry_count = @RetryCount
                    WHERE id = @Id
                    """;

                await connection.ExecuteAsync(
                    deadLetterSql,
                    new
                    {
                        inboxMessage.Id,
                        ProcessedOnUtc = now,
                        Error = exception.ToString(),
                        RetryCount = newRetryCount
                    },
                    transaction: transaction);
            }
            else
            {
                // Schedule retry with exponential backoff
                var nextRetryAt = RetryPolicy.CalculateNextRetry(newRetryCount, now);

                _logger.LogWarning(
                    "Message {MessageId} scheduled for retry {Retry}/{Max} at {NextRetry}",
                    inboxMessage.Id,
                    newRetryCount,
                    _inboxOptions.MaxRetries,
                    nextRetryAt);

                var retrySql =
                    $"""
                    UPDATE {Schema}.inbox_messages
                    SET error = @Error,
                        retry_count = @RetryCount,
                        next_retry_at_utc = @NextRetryAtUtc
                    WHERE id = @Id
                    """;

                await connection.ExecuteAsync(
                    retrySql,
                    new
                    {
                        inboxMessage.Id,
                        Error = exception.ToString(),
                        RetryCount = newRetryCount,
                        NextRetryAtUtc = nextRetryAt
                    },
                    transaction: transaction);
            }
        }
    }

    /// <summary>
    /// Response model for inbox message queries.
    /// </summary>
    protected internal sealed record InboxMessageResponse(Guid Id, string Type, string Content, int RetryCount);
}
