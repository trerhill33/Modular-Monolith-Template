using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ModularTemplate.Api.Shared.RateLimiting;

/// <summary>
/// Extension methods for rate limiting configuration.
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>
    /// The name of the fixed window rate limiter policy.
    /// </summary>
    public const string FixedWindowPolicy = "fixed";

    /// <summary>
    /// Adds rate limiting services with configurable options from appsettings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            var rateLimitingOptions = configuration
                .GetSection(RateLimitingOptions.SectionName)
                .Get<RateLimitingOptions>() ?? new RateLimitingOptions();

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(FixedWindowPolicy, limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromSeconds(rateLimitingOptions.WindowInSeconds);
                limiterOptions.PermitLimit = rateLimitingOptions.PermitLimit;
                limiterOptions.QueueLimit = rateLimitingOptions.QueueLimit;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }
}
