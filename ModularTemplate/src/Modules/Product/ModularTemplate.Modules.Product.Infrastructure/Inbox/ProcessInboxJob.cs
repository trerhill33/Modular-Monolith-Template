using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.FeatureManagement;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Modules.Product.Domain;
using ModularTemplate.Modules.Product.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Product.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory<IProductModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase<IProductModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Product";

    protected override string Schema => Schemas.Product;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
