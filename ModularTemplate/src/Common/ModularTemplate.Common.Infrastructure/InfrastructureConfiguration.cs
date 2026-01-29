using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Auditing;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Application.Features;
using ModularTemplate.Common.Application.Identity;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Application;
using ModularTemplate.Common.Infrastructure.Auditing;
using ModularTemplate.Common.Infrastructure.Auditing.Interceptors;
using ModularTemplate.Common.Infrastructure.Authentication;
using ModularTemplate.Common.Infrastructure.Authorization;
using ModularTemplate.Common.Infrastructure.Caching;
using ModularTemplate.Common.Infrastructure.Clock;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Features;
using ModularTemplate.Common.Infrastructure.Identity;
using ModularTemplate.Common.Infrastructure.Outbox.Persistence;
using ModularTemplate.Common.Infrastructure.Persistence;
using ModularTemplate.Common.Infrastructure.Resilience;
using Npgsql;
using Quartz;
using StackExchange.Redis;

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

        services.AddAuthenticationInternal(configuration);
        services.AddAuthorizationInternal();

        services
            .AddCoreServices()
            .AddResilienceOptions(configuration)
            .AddFeatureFlags(configuration)
            .AddAuditingServices()
            .AddPostgreSql(databaseConnectionString)
            .AddQuartzScheduler()
            .AddCaching(configuration, redisConnectionString)
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddResilienceOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ResilienceOptions>()
            .Bind(configuration.GetSection(ResilienceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    }

    private static IServiceCollection AddFeatureFlags(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<IFeatureFlagService>(sp =>
            new ConfigurationFeatureFlagService(configuration));
        return services;
    }

    private static IServiceCollection AddAuditingServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentUserService, CurrentUserService>();
        services.TryAddScoped<AuditableEntitiesInterceptor>();
        services.TryAddScoped<SoftDeleteInterceptor>();
        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();
        services.TryAddScoped<ICacheWriteScope, CacheWriteScope>();
        services.TryAddScoped<CacheWriteGuardInterceptor>();

        // Audit trail services
        services.TryAddScoped<AuditTrailInterceptor>();
        services.TryAddScoped<IAuditContext, AuditContext>();

        return services;
    }

    private static IServiceCollection AddPostgreSql(this IServiceCollection services, string connectionString)
    {
        var npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);
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

    private static IServiceCollection AddQuartzScheduler(this IServiceCollection services)
    {
        services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration,
        string redisConnectionString)
    {
        services.AddOptions<CachingOptions>()
            .Bind(configuration.GetSection(CachingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch (Exception ex)
        {
            LogRedisFallback(ex, redisConnectionString);
            services.AddDistributedMemoryCache();
        }

        services.TryAddScoped<ICacheService, CacheService>();
        return services;
    }

    private static void LogRedisFallback(Exception ex, string connectionString)
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger("InfrastructureConfiguration");
        logger.LogWarning(
            ex,
            "Failed to connect to Redis at '{ConnectionString}'. Falling back to in-memory distributed cache. " +
            "This is not suitable for production multi-instance deployments",
            connectionString);
    }
}
