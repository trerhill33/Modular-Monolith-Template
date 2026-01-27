using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Orders.Application;
using ModularTemplate.Modules.Orders.Domain;
using ModularTemplate.Modules.Orders.Domain.Customers;
using ModularTemplate.Modules.Orders.Domain.Orders;
using ModularTemplate.Modules.Orders.Domain.ProductsCache;
using ModularTemplate.Modules.Orders.Infrastructure.EventBus;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence.Repositories;
using ModularTemplate.Modules.Orders.Presentation.IntegrationEvents;
using ProcessInboxJob = ModularTemplate.Modules.Orders.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.Orders.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.Orders.Infrastructure;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(
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
        services.AddModuleDbContext<OrdersDbContext>(databaseConnectionString, Schemas.Orders);

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductCacheRepository, ProductCacheRepository>();
        services.AddScoped<IProductCacheWriter, ProductCacheRepository>();
        services.AddScoped<IUnitOfWork<IOrdersModule>>(sp => sp.GetRequiredService<OrdersDbContext>());

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
        services.Configure<OutboxOptions>(configuration.GetSection("Features:Messaging:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob<ProcessOutboxJob>>();

        // Inbox pattern
        services.Configure<InboxOptions>(configuration.GetSection("Features:Messaging:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob<ProcessInboxJob>>();

        return services;
    }
}
