using Amazon.SQS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Infrastructure.EventBus.Aws;

namespace ModularTemplate.Modules.Sales.Infrastructure.EventBus;

internal sealed class ProcessSqsJob(
    IAmazonSQS sqsClient,
    IEventDispatcher eventDispatcher,
    IOptions<AwsMessagingOptions> options,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessSqsJob> logger)
    : SqsPollingJobBase(sqsClient, eventDispatcher, options, featureFlagService, logger)
{
    protected override string ModuleName => "Sales";
}
