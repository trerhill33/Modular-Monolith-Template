using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rtl.Core.Application.FeatureManagement;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Module.Organization.Domain;
using Rtl.Module.Organization.Infrastructure.Persistence;
using System.Reflection;

namespace Rtl.Module.Organization.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory<IOrganizationModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase<IOrganizationModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Organization";

    protected override string Schema => Schemas.Organization;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
