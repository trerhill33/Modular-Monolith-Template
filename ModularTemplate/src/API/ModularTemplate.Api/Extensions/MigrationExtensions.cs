using Microsoft.EntityFrameworkCore;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence;

namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for database migration management.
/// </summary>
internal static class MigrationExtensions
{
    /// <summary>
    /// Applies pending migrations for all module DbContexts when running in Development environment.
    /// In non-development environments (Staging, Production), migrations should be applied
    /// through CI/CD pipelines for controlled deployment.
    /// </summary>
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return app;
        }

        using var scope = app.ApplicationServices.CreateScope();

        ApplyMigration<SalesDbContext>(scope);
        ApplyMigration<OrdersDbContext>(scope);

        return app;
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        context.Database.Migrate();
    }
}
