using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Sales.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Sales";

    protected override string Schema => Schemas.Sales;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
