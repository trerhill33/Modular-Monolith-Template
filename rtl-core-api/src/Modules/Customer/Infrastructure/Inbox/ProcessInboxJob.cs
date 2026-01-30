using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rtl.Core.Application.FeatureManagement;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Module.Customer.Domain;
using Rtl.Module.Customer.Infrastructure.Persistence;
using System.Reflection;

namespace Rtl.Module.Customer.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory<ICustomerModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase<ICustomerModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Customer";

    protected override string Schema => Schemas.Customer;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
