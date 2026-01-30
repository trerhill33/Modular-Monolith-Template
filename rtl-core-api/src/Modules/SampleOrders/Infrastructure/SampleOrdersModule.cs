using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure;
using Rtl.Core.Infrastructure.EventBus;
using Rtl.Core.Infrastructure.Inbox.Job;
using Rtl.Core.Infrastructure.Outbox.Job;
using Rtl.Core.Infrastructure.Persistence;
using Rtl.Module.SampleOrders.Application;
using Rtl.Module.SampleOrders.Domain;
using Rtl.Module.SampleOrders.Domain.Customers;
using Rtl.Module.SampleOrders.Domain.Orders;
using Rtl.Module.SampleOrders.Domain.ProductsCache;
using Rtl.Module.SampleOrders.Infrastructure.EventBus;
using Rtl.Module.SampleOrders.Infrastructure.Persistence;
using Rtl.Module.SampleOrders.Infrastructure.Persistence.Repositories;
using Rtl.Module.SampleOrders.Presentation.IntegrationEvents;
using ProcessInboxJob = Rtl.Module.SampleOrders.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = Rtl.Module.SampleOrders.Infrastructure.Outbox.ProcessOutboxJob;

namespace Rtl.Module.SampleOrders.Infrastructure;

public static class SampleOrdersModule
{
    public static IServiceCollection AddSampleOrdersModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddModuleDataSource<ISampleOrdersModule>(databaseConnectionString)
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
        services.AddScoped<IUnitOfWork<ISampleOrdersModule>>(sp => sp.GetRequiredService<OrdersDbContext>());

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
