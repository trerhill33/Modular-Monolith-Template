using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Modules.Organization.Domain;
using ModularTemplate.Modules.Organization.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Organization.Infrastructure.Outbox;

internal sealed class ProcessOutboxJob(
    IDbConnectionFactory<IOrganizationModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessOutboxJob> logger)
    : ProcessOutboxJobBase<IOrganizationModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, outboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Organization";

    protected override string Schema => Schemas.Organization;

    protected override Assembly HandlersAssembly => Application.AssemblyReference.Assembly;
}
