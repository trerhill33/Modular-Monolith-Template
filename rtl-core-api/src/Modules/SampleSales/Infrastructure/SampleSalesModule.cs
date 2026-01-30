using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure;
using Rtl.Core.Infrastructure.EventBus;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Core.Infrastructure.Outbox.Job;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleSales.Application;
using Rtl.Module.SampleSales.Domain;
using Rtl.Module.SampleSales.Domain.Catalogs;
using Rtl.Module.SampleSales.Domain.OrdersCache;
using Rtl.Module.SampleSales.Domain.Products;
using Rtl.Module.SampleSales.Infrastructure.EventBus;
using Rtl.Module.SampleSales.Infrastructure.Persistence;
using Rtl.Module.SampleSales.Infrastructure.Persistence.Repositories;
using Rtl.Module.SampleSales.Presentation.IntegrationEvents;
using ProcessInboxJob = Rtl.Module.SampleSales.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = Rtl.Module.SampleSales.Infrastructure.Outbox.ProcessOutboxJob;

namespace Rtl.Module.SampleSales.Infrastructure;

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
