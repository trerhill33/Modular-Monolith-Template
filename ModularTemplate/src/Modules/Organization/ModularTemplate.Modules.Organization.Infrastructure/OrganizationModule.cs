using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Common.Infrastructure.Outbox.Job;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Modules.Organization.Domain;
using ModularTemplate.Modules.Organization.Infrastructure.Persistence;
using ProcessInboxJob = ModularTemplate.Modules.Organization.Infrastructure.Inbox.ProcessInboxJob;
using ProcessOutboxJob = ModularTemplate.Modules.Organization.Infrastructure.Outbox.ProcessOutboxJob;

namespace ModularTemplate.Modules.Organization.Infrastructure;

public static class OrganizationModule
{
    public static IServiceCollection AddOrganizationModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString)
    {
        services
            .AddModuleDataSource<IOrganizationModule>(databaseConnectionString)
            .AddPersistence(databaseConnectionString)
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        services.AddModuleDbContext<OrganizationDbContext>(databaseConnectionString, Schemas.Organization);

        services.AddScoped<IUnitOfWork<IOrganizationModule>>(sp => sp.GetRequiredService<OrganizationDbContext>());

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
