using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Product.Domain;
using ModularTemplate.Modules.Product.Infrastructure.Persistence;
using ProcessInboxJob = ModularTemplate.Modules.Product.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.Product.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.Product.Infrastructure;

public static class ProductModule
{
    public static IServiceCollection AddProductModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddModuleDataSource<IProductModule>(databaseConnectionString)
            .AddPersistence(databaseConnectionString)
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddModuleDbContext<ProductDbContext>(databaseConnectionString, Schemas.Product);

        services.AddScoped<IUnitOfWork<IProductModule>>(sp => sp.GetRequiredService<ProductDbContext>());

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Integration event handlers
        services.AddIntegrationEventHandlers(Presentation.AssemblyReference.Assembly);

        // SQS polling (disabled in development)
        services.AddSqsPolling<EventBus.ProcessSqsJob>(environment);

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
