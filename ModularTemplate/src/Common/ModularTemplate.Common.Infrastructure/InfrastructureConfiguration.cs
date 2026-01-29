using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Infrastructure.Application;
using ModularTemplate.Common.Infrastructure.Auditing;
using ModularTemplate.Common.Infrastructure.Authentication;
using ModularTemplate.Common.Infrastructure.Authorization;
using ModularTemplate.Common.Infrastructure.Caching;
using ModularTemplate.Common.Infrastructure.Clock;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.FeatureManagement;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Common.Infrastructure.Quartz;
using ModularTemplate.Common.Infrastructure.Resilience;
using Npgsql;

namespace ModularTemplate.Common.Infrastructure;

/// <summary>
/// Extension methods for configuring the infrastructure layer.
/// </summary>
public static class InfrastructureConfiguration
{
    /// <summary>
    /// Adds infrastructure layer services.
    /// </summary>
    public static IServiceCollection AddCommonInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString,
        string redisConnectionString)
    {
        // Register post-configure handlers to derive values from ApplicationOptions
        // Note: ApplicationOptions must be registered before this via AddApplicationOptions()
        services.ConfigureOptions<ConfigureAwsMessagingOptions>();

        // Each feature registers itself via its Startup class
        services
            .AddClockServices()
            .AddResilienceServices(configuration)
            .AddFeatureManagementServices(configuration)
            .AddAuditingServices()
            .AddPersistenceServices(databaseConnectionString)
            .AddQuartzServices()
            .AddCachingServices(configuration, redisConnectionString)
            .AddMessagingServices(configuration, environment)
            .AddAuthenticationServices(configuration)
            .AddAuthorizationServices();

        return services;
    }

    /// <summary>
    /// Registers a module-specific database data source and connection factory.
    /// </summary>
    public static IServiceCollection AddModuleDataSource<TModule>(
        this IServiceCollection services,
        string connectionString)
        where TModule : class
    {
        var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.AddKeyedSingleton<NpgsqlDataSource>(typeof(TModule), dataSource);

        // Use factory delegate to resolve the keyed NpgsqlDataSource
        services.AddScoped<IDbConnectionFactory<TModule>>(sp =>
        {
            var keyedDataSource = sp.GetRequiredKeyedService<NpgsqlDataSource>(typeof(TModule));
            return new DbConnectionFactory<TModule>(keyedDataSource);
        });

        return services;
    }
}
