using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Sample.Application;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Domain.Catalogs;
using ModularTemplate.Modules.Sample.Domain.OrdersCache;
using ModularTemplate.Modules.Sample.Domain.Products;
using ModularTemplate.Modules.Sample.Infrastructure.EventBus;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence.Repositories;
using ModularTemplate.Modules.Sample.Presentation.IntegrationEvents;
using ProcessInboxJob = ModularTemplate.Modules.Sample.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.Sample.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.Sample.Infrastructure;

public static class SampleModule
{
    public static IServiceCollection AddSampleModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddModuleDataSource<ISampleModule>(databaseConnectionString)
            .AddPersistence(databaseConnectionString)
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddModuleDbContext<SampleDbContext>(databaseConnectionString, Schemas.Sample);

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IOrderCacheRepository, OrderCacheRepository>();
        services.AddScoped<IOrderCacheWriter, OrderCacheRepository>();
        services.AddScoped<IUnitOfWork<ISampleModule>>(sp => sp.GetRequiredService<SampleDbContext>());

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Integration event handlers
        services.AddIntegrationEventHandlers(AssemblyReference.Assembly);

        // SQS polling (disabled in development)
        services.AddSqsPolling<ProcessSqsJob>(environment);

        // Outbox pattern
        services.AddOptions<OutboxOptions>()
            .Bind(configuration.GetSection("Messaging:Outbox"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.ConfigureOptions<ConfigureProcessOutboxJob<ProcessOutboxJob>>();

        // Inbox pattern
        services.AddOptions<InboxOptions>()
            .Bind(configuration.GetSection("Messaging:Inbox"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.ConfigureOptions<ConfigureProcessInboxJob<ProcessInboxJob>>();

        return services;
    }
}
