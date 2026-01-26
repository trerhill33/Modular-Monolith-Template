using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using ModularTemplate.Common.Infrastructure.Auditing;
using ModularTemplate.Common.Infrastructure.Caching;
using ModularTemplate.Common.Infrastructure.Outbox.Data;

namespace ModularTemplate.Common.Infrastructure.Persistence;

/// <summary>
/// Extension methods for configuring module DbContexts with standard conventions.
/// </summary>
public static class ModuleDbContextExtensions
{
    public static IServiceCollection AddModuleDbContext<TDbContext>(
        this IServiceCollection services,
        string connectionString,
        string schema,
        Action<DbContextOptionsBuilder>? configure = null)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    schema))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(
                    sp.GetRequiredService<CacheWriteGuardInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>(),
                    sp.GetRequiredService<AuditableEntitiesInterceptor>(),
                    sp.GetRequiredService<AuditTrailInterceptor>(),
                    sp.GetRequiredService<InsertOutboxMessagesInterceptor>());

            // Apply any module-specific configuration
            configure?.Invoke(options);
        });

        return services;
    }
}
