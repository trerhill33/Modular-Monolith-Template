using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Application;
using ModularTemplate.Modules.Sales.Domain.Catalogs;
using ModularTemplate.Modules.Sales.Domain.OrdersCache;
using ModularTemplate.Modules.Sales.Domain.Products;
using ModularTemplate.Modules.Sales.Infrastructure.EventBus;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence.Repositories;
using ModularTemplate.Modules.Sales.Presentation.IntegrationEvents;
using ProcessInboxJob = ModularTemplate.Modules.Sales.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.Sales.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.Sales.Infrastructure;

public static class SalesModule
{
    public static IServiceCollection AddSalesModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddPersistence(databaseConnectionString)
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddModuleDbContext<SalesDbContext>(databaseConnectionString, Schemas.Sales);

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IOrderCacheRepository, OrderCacheRepository>();
        services.AddScoped<IOrderCacheWriter, OrderCacheRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SalesDbContext>());

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
        services.Configure<OutboxOptions>(configuration.GetSection("Sales:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob<ProcessOutboxJob>>();

        // Inbox pattern
        services.Configure<InboxOptions>(configuration.GetSection("Sales:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob<ProcessInboxJob>>();

        return services;
    }
}
