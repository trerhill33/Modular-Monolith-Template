using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularTemplate.Common.Application.Authorization;

namespace ModularTemplate.Common.Infrastructure.Authorization;

/// <summary>
/// Extension methods for configuring authorization.
/// </summary>
internal static class AuthorizationExtensions
{
    internal static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        // Add ASP.NET Core authorization services
        services.AddAuthorization();

        // Register default permission service (can be overridden by modules)
        services.TryAddScoped<IPermissionService, DefaultPermissionService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}
