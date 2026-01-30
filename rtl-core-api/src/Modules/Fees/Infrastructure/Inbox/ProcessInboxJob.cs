using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rtl.Core.Application.FeatureManagement;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Module.Fees.Domain;
using Rtl.Module.Fees.Infrastructure.Persistence;
using System.Reflection;

namespace Rtl.Module.Fees.Infrastructure.Inbox;

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
