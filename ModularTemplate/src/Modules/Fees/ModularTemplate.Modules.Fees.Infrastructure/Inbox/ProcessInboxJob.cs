using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Modules.Fees.Domain;
using ModularTemplate.Modules.Fees.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Fees.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory<IFeesModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase<IFeesModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Fees";

    protected override string Schema => Schemas.Fees;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
