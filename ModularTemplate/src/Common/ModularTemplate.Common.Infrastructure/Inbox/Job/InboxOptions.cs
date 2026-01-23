namespace ModularTemplate.Common.Infrastructure.Inbox.Job;

/// <summary>
/// Configuration options for the inbox processor job.
/// </summary>
/// <remarks>
/// These options control how the inbox processor behaves, including
/// polling intervals, batch sizes, and retry policies.
/// </remarks>
public sealed class InboxOptions
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
