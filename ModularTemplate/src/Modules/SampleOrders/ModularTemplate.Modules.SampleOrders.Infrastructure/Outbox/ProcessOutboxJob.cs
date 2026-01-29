using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Modules.SampleOrders.Domain;
using ModularTemplate.Modules.SampleOrders.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.SampleOrders.Infrastructure.Outbox;

internal sealed class ProcessOutboxJob(
    IDbConnectionFactory<ISampleOrdersModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessOutboxJob> logger)
    : ProcessOutboxJobBase<ISampleOrdersModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, outboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "SampleOrders";

    protected override string Schema => Schemas.Orders;

    protected override Assembly HandlersAssembly => Application.AssemblyReference.Assembly;
}
