using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Fees.Contracts;
using ModularTemplate.Modules.Fees.Domain;
using ModularTemplate.Modules.Fees.Domain.FeeSchedules;
using ModularTemplate.Modules.Fees.Infrastructure.Persistence;
using ModularTemplate.Modules.Fees.Infrastructure.Persistence.Repositories;
using ModularTemplate.Modules.Fees.Infrastructure.Services;
using ProcessInboxJob = ModularTemplate.Modules.Fees.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.Fees.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.Fees.Infrastructure;

public static class FeesModule
{
    public static IServiceCollection AddFeesModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddModuleDataSource<IFeesModule>(databaseConnectionString)
            .AddPersistence(databaseConnectionString)
            .AddServices()
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddModuleDbContext<FeesDbContext>(databaseConnectionString, Schemas.Fees);

        services.AddScoped<IUnitOfWork<IFeesModule>>(sp => sp.GetRequiredService<FeesDbContext>());

        // Repositories
        services.AddScoped<IFeeScheduleRepository, FeeScheduleRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register the public contract implementation
        // This allows other modules to inject IFeeCalculator
        services.AddScoped<IFeeCalculator, FeeCalculator>();

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
        services.Configure<OutboxOptions>(configuration.GetSection("Features:Messaging:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob<ProcessOutboxJob>>();

        // Inbox pattern
        services.Configure<InboxOptions>(configuration.GetSection("Features:Messaging:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob<ProcessInboxJob>>();

        return services;
    }
}
