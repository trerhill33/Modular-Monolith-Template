using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace ModularTemplate.Api.Shared.HealthChecks;

/// <summary>
/// Configuration for a module health check.
/// </summary>
public sealed class ModuleHealthCheckOptions
{
    /// <summary>
    /// Gets or sets the module name.
    /// </summary>
    public required string ModuleName { get; set; }

    /// <summary>
    /// Gets or sets the database schema for this module.
    /// </summary>
    public required string Schema { get; set; }

    /// <summary>
    /// Gets or sets the age threshold in seconds for pending outbox messages to indicate degraded health.
    /// </summary>
    public int OutboxDegradedThresholdSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the age threshold in seconds for pending outbox messages to indicate unhealthy status.
    /// </summary>
    public int OutboxUnhealthyThresholdSeconds { get; set; } = 300;

    /// <summary>
    /// Gets or sets the count threshold for pending outbox messages to indicate degraded health.
    /// </summary>
    public int OutboxDegradedCountThreshold { get; set; } = 50;

    /// <summary>
    /// Gets or sets the count threshold for pending outbox messages to indicate unhealthy status.
    /// </summary>
    public int OutboxUnhealthyCountThreshold { get; set; } = 500;
}

/// <summary>
/// Health check for an individual module that monitors its outbox and inbox queues.
/// </summary>
public sealed class ModuleHealthCheck : IHealthCheck
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ModuleHealthCheckOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleHealthCheck"/> class.
    /// </summary>
    /// <param name="dataSource">The PostgreSQL data source.</param>
    /// <param name="options">Configuration options for the module health check.</param>
    public ModuleHealthCheck(NpgsqlDataSource dataSource, ModuleHealthCheckOptions options)
    {
        _dataSource = dataSource;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                ["module"] = _options.ModuleName,
                ["schema"] = _options.Schema
            };

            // Check outbox status
            var outboxStatus = await CheckOutboxAsync(cancellationToken);
            data["outbox"] = new
            {
                pendingCount = outboxStatus.PendingCount,
                oldestPendingAgeSeconds = outboxStatus.OldestPendingAgeSeconds,
                failedCount = outboxStatus.FailedCount
            };

            // Check inbox status
            var inboxStatus = await CheckInboxAsync(cancellationToken);
            data["inbox"] = new
            {
                pendingCount = inboxStatus.PendingCount,
                oldestPendingAgeSeconds = inboxStatus.OldestPendingAgeSeconds,
                failedCount = inboxStatus.FailedCount
            };

            // Determine overall status
            var outboxHealth = DetermineOutboxStatus(outboxStatus);
            var inboxHealth = DetermineOutboxStatus(inboxStatus); // Using same thresholds for inbox

            var overallStatus = (outboxHealth, inboxHealth) switch
            {
                (HealthStatus.Unhealthy, _) => HealthStatus.Unhealthy,
                (_, HealthStatus.Unhealthy) => HealthStatus.Unhealthy,
                (HealthStatus.Degraded, _) => HealthStatus.Degraded,
                (_, HealthStatus.Degraded) => HealthStatus.Degraded,
                _ => HealthStatus.Healthy
            };

            data["outboxStatus"] = outboxHealth.ToString();
            data["inboxStatus"] = inboxHealth.ToString();

            var description = overallStatus switch
            {
                HealthStatus.Healthy => $"Module {_options.ModuleName} healthy",
                HealthStatus.Degraded => $"Module {_options.ModuleName} degraded: outbox={outboxStatus.PendingCount} pending, inbox={inboxStatus.PendingCount} pending",
                _ => $"Module {_options.ModuleName} unhealthy: outbox={outboxStatus.PendingCount} pending/{outboxStatus.FailedCount} failed, inbox={inboxStatus.PendingCount} pending/{inboxStatus.FailedCount} failed"
            };

            return new HealthCheckResult(overallStatus, description, data: data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Failed to check module {_options.ModuleName} health", ex);
        }
    }

    private async Task<MessageQueueStatus> CheckOutboxAsync(CancellationToken cancellationToken)
    {
        return await CheckMessageTableAsync("outbox_messages", cancellationToken);
    }

    private async Task<MessageQueueStatus> CheckInboxAsync(CancellationToken cancellationToken)
    {
        return await CheckMessageTableAsync("inbox_messages", cancellationToken);
    }

    private async Task<MessageQueueStatus> CheckMessageTableAsync(string tableName, CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var query = $"""
            SELECT
                COUNT(*) FILTER (WHERE processed_on_utc IS NULL) as pending_count,
                COALESCE(EXTRACT(EPOCH FROM (NOW() - MIN(occurred_on_utc))) FILTER (WHERE processed_on_utc IS NULL), 0) as oldest_age_seconds,
                COUNT(*) FILTER (WHERE processed_on_utc IS NULL AND error IS NOT NULL) as failed_count
            FROM {_options.Schema}.{tableName}
            WHERE processed_on_utc IS NULL
               OR (processed_on_utc IS NULL AND occurred_on_utc > NOW() - INTERVAL '1 day')
            """;

        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new MessageQueueStatus(
                reader.GetInt64(0),
                reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                reader.GetInt64(2));
        }

        return new MessageQueueStatus(0, 0, 0);
    }

    private HealthStatus DetermineOutboxStatus(MessageQueueStatus status)
    {
        if (status.OldestPendingAgeSeconds >= _options.OutboxUnhealthyThresholdSeconds ||
            status.PendingCount >= _options.OutboxUnhealthyCountThreshold)
        {
            return HealthStatus.Unhealthy;
        }

        if (status.OldestPendingAgeSeconds >= _options.OutboxDegradedThresholdSeconds ||
            status.PendingCount >= _options.OutboxDegradedCountThreshold)
        {
            return HealthStatus.Degraded;
        }

        return HealthStatus.Healthy;
    }

    private sealed record MessageQueueStatus(
        long PendingCount,
        double OldestPendingAgeSeconds,
        long FailedCount);
}
