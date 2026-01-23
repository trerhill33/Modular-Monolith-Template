using Amazon.EventBridge;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Infrastructure.EventBus.Aws;
using ModularTemplate.Common.Infrastructure.EventBus.InMemory;
using Quartz;
using System.Reflection;

namespace ModularTemplate.Common.Infrastructure.EventBus;

/// <summary>
/// Extension methods for configuring messaging services based on environment.
/// </summary>
public static class MessagingServiceCollectionExtensions
{
    /// <summary>
    /// Adds messaging services configured for the current environment.
    /// Development: In-memory event bus (synchronous, no external dependencies)
    /// Production: EventBridge (publishing) + SQS (consuming)
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The host environment.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Always register the event dispatcher
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        if (environment.IsDevelopment())
        {
            // Local development: In-memory, synchronous dispatch
            services.AddScoped<IEventBus, InMemoryEventBus>();
        }
        else
        {
            // Production: EventBridge + SQS
            services.Configure<AwsMessagingOptions>(
                configuration.GetSection(AwsMessagingOptions.SectionName));

            // Register AWS SDK clients
            services.AddAWSService<IAmazonEventBridge>();
            services.AddAWSService<IAmazonSQS>();

            // EventBridge publisher
            services.AddScoped<IEventBus, EventBridgeEventBus>();
        }

        return services;
    }

    /// <summary>
    /// Adds SQS polling job for consuming events. Only active in non-development environments.
    /// </summary>
    /// <typeparam name="TJob">The SQS polling job type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="environment">The host environment.</param>
    /// <param name="pollingIntervalSeconds">Interval between SQS polls (default: 5 seconds).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSqsPolling<TJob>(
        this IServiceCollection services,
        IHostEnvironment environment,
        int pollingIntervalSeconds = 5)
        where TJob : class, IJob
    {
        if (!environment.IsDevelopment())
        {
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey(typeof(TJob).Name);
                q.AddJob<TJob>(opts => opts
                    .WithIdentity(jobKey)
                    .StoreDurably()); // Required when using multiple AddQuartz calls
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{typeof(TJob).Name}-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(pollingIntervalSeconds)
                        .RepeatForever()));
            });
        }

        return services;
    }

    /// <summary>
    /// Registers integration event handlers from the specified assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly containing integration event handlers.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddIntegrationEventHandlers(
        this IServiceCollection services,
        Assembly assembly)
    {
        // Find all types implementing IIntegrationEventHandler<T>
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, handlerType);
            }
        }

        return services;
    }
}
