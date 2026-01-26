using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Orders.Infrastructure.Outbox;

internal sealed class ProcessOutboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessOutboxJob> logger)
    : ProcessOutboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, outboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Orders";

    protected override string Schema => Schemas.Orders;

    protected override Assembly HandlersAssembly => Application.AssemblyReference.Assembly;
}
