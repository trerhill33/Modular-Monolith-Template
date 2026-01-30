using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rtl.Core.Application.FeatureManagement;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Module.Sales.Domain;
using Rtl.Module.Sales.Infrastructure.Persistence;
using System.Reflection;

namespace Rtl.Module.Sales.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory<ISalesModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase<ISalesModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Sales";

    protected override string Schema => Schemas.Sales;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
