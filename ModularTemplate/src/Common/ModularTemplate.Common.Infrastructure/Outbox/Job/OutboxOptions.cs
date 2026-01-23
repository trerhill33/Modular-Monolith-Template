namespace ModularTemplate.Common.Infrastructure.Outbox.Job;

/// <summary>
/// Configuration options for the outbox processor job.
/// </summary>
/// <remarks>
/// These options control how the outbox processor behaves, including
/// polling intervals, batch sizes, and retry policies.
/// </remarks>
public sealed class OutboxOptions
{
    /// <summary>
    /// Gets the interval in seconds between job executions.
    /// </summary>
    public int IntervalInSeconds { get; init; }

    /// <summary>
    /// Gets the number of messages to process per batch.
    /// </summary>
    public int BatchSize { get; init; }

    /// <summary>
    /// Gets the maximum number of retry attempts before a message becomes a dead letter.
    /// </summary>
    public int MaxRetries { get; init; }
}
