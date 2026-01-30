using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rtl.Core.Application.FeatureManagement;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Module.Product.Domain;
using Rtl.Module.Product.Infrastructure.Persistence;
using System.Reflection;

namespace Rtl.Module.Product.Infrastructure.Inbox;

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
