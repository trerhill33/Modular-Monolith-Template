using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ModularTemplate.Api.Shared.HealthChecks;

/// <summary>
/// Configuration options for the SQS queue depth health check.
/// </summary>
public sealed class SqsQueueDepthHealthCheckOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "HealthChecks:SqsQueueDepth";

    /// <summary>
    /// Gets or sets the SQS queue URL to check.
    /// </summary>
    public string QueueUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message count threshold for degraded health.
    /// Default is 1000 messages.
    /// </summary>
    public int DegradedThreshold { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the message count threshold for unhealthy status.
    /// Default is 10000 messages.
    /// </summary>
    public int UnhealthyThreshold { get; set; } = 10000;

    /// <summary>
    /// Gets or sets whether to include dead letter queue metrics if available.
    /// </summary>
    public bool IncludeDeadLetterQueue { get; set; } = true;

    /// <summary>
    /// Gets or sets the dead letter queue URL to check.
    /// </summary>
    public string? DeadLetterQueueUrl { get; set; }

    /// <summary>
    /// Gets or sets the threshold for messages in the dead letter queue that indicates unhealthy status.
    /// Default is 1 message (any message in DLQ is concerning).
    /// </summary>
    public int DeadLetterQueueUnhealthyThreshold { get; set; } = 1;
}

/// <summary>
/// Health check that monitors SQS queue depth and dead letter queue status.
/// </summary>
public sealed class SqsQueueDepthHealthCheck : IHealthCheck
{
    private readonly IAmazonSQS? _sqsClient;
    private readonly SqsQueueDepthHealthCheckOptions _options;
    private readonly ILogger<SqsQueueDepthHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqsQueueDepthHealthCheck"/> class.
    /// </summary>
    /// <param name="sqsClient">The AWS SQS client (optional - may be null if AWS is not configured).</param>
    /// <param name="options">Configuration options for the health check.</param>
    /// <param name="logger">The logger instance.</param>
    public SqsQueueDepthHealthCheck(
        IAmazonSQS? sqsClient,
        IOptions<SqsQueueDepthHealthCheckOptions> options,
        ILogger<SqsQueueDepthHealthCheck> logger)
    {
        _sqsClient = sqsClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // If SQS is not configured, skip this check gracefully
        if (_sqsClient is null || string.IsNullOrEmpty(_options.QueueUrl))
        {
            return HealthCheckResult.Healthy(
                "SQS queue depth check skipped: AWS SQS is not configured",
                new Dictionary<string, object> { ["skipped"] = true });
        }

        try
        {
            var data = new Dictionary<string, object>();

            // Get main queue attributes
            var mainQueueMetrics = await GetQueueMetricsAsync(_options.QueueUrl, cancellationToken);
            data["mainQueue"] = new
            {
                url = _options.QueueUrl,
                approximateMessageCount = mainQueueMetrics.ApproximateMessageCount,
                approximateNotVisibleCount = mainQueueMetrics.ApproximateNotVisibleCount,
                approximateDelayedCount = mainQueueMetrics.ApproximateDelayedCount
            };

            var status = DetermineMainQueueStatus(mainQueueMetrics);

            // Check dead letter queue if configured
            if (_options.IncludeDeadLetterQueue && !string.IsNullOrEmpty(_options.DeadLetterQueueUrl))
            {
                var dlqMetrics = await GetQueueMetricsAsync(_options.DeadLetterQueueUrl, cancellationToken);
                data["deadLetterQueue"] = new
                {
                    url = _options.DeadLetterQueueUrl,
                    approximateMessageCount = dlqMetrics.ApproximateMessageCount
                };

                if (dlqMetrics.ApproximateMessageCount >= _options.DeadLetterQueueUnhealthyThreshold)
                {
                    status = HealthStatus.Unhealthy;
                    data["deadLetterQueueAlert"] = true;
                }
            }

            var totalMessages = mainQueueMetrics.ApproximateMessageCount +
                               mainQueueMetrics.ApproximateNotVisibleCount +
                               mainQueueMetrics.ApproximateDelayedCount;

            data["totalMessagesInFlight"] = totalMessages;

            var description = status switch
            {
                HealthStatus.Healthy => $"SQS queue healthy: {totalMessages} messages in flight",
                HealthStatus.Degraded => $"SQS queue degraded: {totalMessages} messages in flight (threshold: {_options.DegradedThreshold})",
                _ => $"SQS queue unhealthy: {totalMessages} messages in flight (threshold: {_options.UnhealthyThreshold})"
            };

            return new HealthCheckResult(status, description, data: data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check SQS queue depth for {QueueUrl}", _options.QueueUrl);
            return HealthCheckResult.Unhealthy("Failed to check SQS queue depth", ex);
        }
    }

    private async Task<QueueMetrics> GetQueueMetricsAsync(string queueUrl, CancellationToken cancellationToken)
    {
        var request = new GetQueueAttributesRequest
        {
            QueueUrl = queueUrl,
            AttributeNames =
            [
                "ApproximateNumberOfMessages",
                "ApproximateNumberOfMessagesNotVisible",
                "ApproximateNumberOfMessagesDelayed"
            ]
        };

        var response = await _sqsClient!.GetQueueAttributesAsync(request, cancellationToken);

        return new QueueMetrics(
            GetAttributeValue(response, "ApproximateNumberOfMessages"),
            GetAttributeValue(response, "ApproximateNumberOfMessagesNotVisible"),
            GetAttributeValue(response, "ApproximateNumberOfMessagesDelayed"));
    }

    private static int GetAttributeValue(GetQueueAttributesResponse response, string attributeName)
    {
        if (response.Attributes.TryGetValue(attributeName, out var value) &&
            int.TryParse(value, out var intValue))
        {
            return intValue;
        }

        return 0;
    }

    private HealthStatus DetermineMainQueueStatus(QueueMetrics metrics)
    {
        var totalMessages = metrics.ApproximateMessageCount +
                           metrics.ApproximateNotVisibleCount +
                           metrics.ApproximateDelayedCount;

        if (totalMessages >= _options.UnhealthyThreshold)
        {
            return HealthStatus.Unhealthy;
        }

        if (totalMessages >= _options.DegradedThreshold)
        {
            return HealthStatus.Degraded;
        }

        return HealthStatus.Healthy;
    }

    private sealed record QueueMetrics(
        int ApproximateMessageCount,
        int ApproximateNotVisibleCount,
        int ApproximateDelayedCount);
}
