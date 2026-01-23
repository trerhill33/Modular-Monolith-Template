using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Identity;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Application;
using ModularTemplate.Common.Infrastructure.Auditing;
using ModularTemplate.Common.Infrastructure.Authentication;
using ModularTemplate.Common.Infrastructure.Authorization;
using ModularTemplate.Common.Infrastructure.Caching;
using ModularTemplate.Common.Infrastructure.Clock;
using ModularTemplate.Common.Infrastructure.EventBus;
using ModularTemplate.Common.Infrastructure.Identity;
using ModularTemplate.Common.Infrastructure.Outbox.Data;
using ModularTemplate.Common.Infrastructure.Persistence;
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
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The host environment.</param>
    /// <param name="databaseConnectionString">Database connection string.</param>
    /// <param name="redisConnectionString">Redis connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCommonInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string databaseConnectionString,
        string redisConnectionString)
    {
        // Register centralized application identity configuration
        services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptions.SectionName));

        // Register post-configure handlers to derive values from ApplicationOptions
        services.ConfigureOptions<ConfigureAwsMessagingOptions>();

        services.AddAuthenticationInternal(configuration);
        services.AddAuthorizationInternal();

        services
            .AddCoreServices()
            .AddAuditingServices()
            .AddPostgreSql(databaseConnectionString)
            .AddQuartzScheduler()
            .AddCaching(configuration, redisConnectionString)
            .AddMessaging(configuration, environment);

        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
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
        return services;
    }

    private static IServiceCollection AddPostgreSql(this IServiceCollection services, string connectionString)
    {
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);
        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();
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
        services.Configure<CachingOptions>(configuration.GetSection(CachingOptions.SectionName));

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
