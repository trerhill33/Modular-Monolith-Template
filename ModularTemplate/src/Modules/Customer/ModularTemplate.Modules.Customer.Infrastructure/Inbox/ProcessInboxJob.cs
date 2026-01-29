using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.FeatureManagement;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Modules.Customer.Domain;
using ModularTemplate.Modules.Customer.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Customer.Infrastructure.Inbox;

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
