using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Modules.Product.Domain;
using ModularTemplate.Modules.Product.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Product.Infrastructure.Outbox;

internal sealed class ProcessOutboxJob(
    IDbConnectionFactory<IProductModule> dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    IFeatureFlagService featureFlagService,
    ILogger<ProcessOutboxJob> logger)
    : ProcessOutboxJobBase<IProductModule>(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, outboxOptions, featureFlagService, logger)
{
    protected override string ModuleName => "Product";

    protected override string Schema => Schemas.Product;

    protected override Assembly HandlersAssembly => Application.AssemblyReference.Assembly;
}
