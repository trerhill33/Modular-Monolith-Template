using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularTemplate.Common.Application.Auditing;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Application.Identity;
using ModularTemplate.Common.Infrastructure.Auditing.Interceptors;
using ModularTemplate.Common.Infrastructure.Caching;
using ModularTemplate.Common.Infrastructure.Identity;
using ModularTemplate.Common.Infrastructure.Outbox.Persistence;

namespace ModularTemplate.Common.Infrastructure.Auditing;

/// <summary>
/// Extension methods for configuring auditing services.
/// </summary>
internal static class Startup
{
    /// <summary>
    /// Adds auditing services to the service collection.
    /// </summary>
    internal static IServiceCollection AddAuditingServices(this IServiceCollection services)
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
}
