using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.SampleSales.Application;
using ModularTemplate.Modules.SampleSales.Domain;
using ModularTemplate.Modules.SampleSales.Domain.Catalogs;
using ModularTemplate.Modules.SampleSales.Domain.OrdersCache;
using ModularTemplate.Modules.SampleSales.Domain.Products;
using ModularTemplate.Modules.SampleSales.Infrastructure.EventBus;
using ModularTemplate.Modules.SampleSales.Infrastructure.Persistence;
using ModularTemplate.Modules.SampleSales.Infrastructure.Persistence.Repositories;
using ModularTemplate.Modules.SampleSales.Presentation.IntegrationEvents;
using ProcessInboxJob = ModularTemplate.Modules.SampleSales.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.SampleSales.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.SampleSales.Infrastructure;

public static class SampleSalesModule
{
    public static IServiceCollection AddSampleSalesModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddModuleDataSource<ISampleSalesModule>(databaseConnectionString)
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
        services.AddScoped<IUnitOfWork<ISampleSalesModule>>(sp => sp.GetRequiredService<SampleDbContext>());

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
