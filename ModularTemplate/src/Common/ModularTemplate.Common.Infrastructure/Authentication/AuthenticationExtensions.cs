using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularTemplate.Common.Infrastructure.Application;

namespace ModularTemplate.Common.Infrastructure.Authentication;

/// <summary>
/// Extension methods for configuring authentication.
/// </summary>
internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register AuthenticationOptions from config section
        services.Configure<AuthenticationOptions>(
            configuration.GetSection(AuthenticationOptions.SectionName));

        // Register post-configure to derive values from ApplicationOptions
        services.ConfigureOptions<ConfigureAuthenticationOptions>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Configure JwtBearerOptions from our AuthenticationOptions
        services.ConfigureOptions<JwtBearerConfigureOptions>();

        return services;
    }
}
