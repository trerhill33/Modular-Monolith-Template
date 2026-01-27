using Microsoft.EntityFrameworkCore;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence;
using Npgsql;

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
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app, IHostEnvironment environment, string connectionString)
    {
        if (!environment.IsDevelopment())
        {
            return app;
        }

        // Ensure the database exists before running migrations
        EnsureDatabaseExists(connectionString);

        using var scope = app.ApplicationServices.CreateScope();

        ApplyMigration<SampleDbContext>(scope);
        ApplyMigration<OrdersDbContext>(scope);

        return app;
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;

        // Connect to the default 'postgres' database to check/create our target database
        builder.Database = "postgres";

        using var connection = new NpgsqlConnection(builder.ConnectionString);
        connection.Open();

        // Check if database exists
        using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @dbname";
        checkCmd.Parameters.AddWithValue("dbname", databaseName!);

        var exists = checkCmd.ExecuteScalar() != null;

        if (!exists)
        {
            // Create the database
            using var createCmd = connection.CreateCommand();
            createCmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
            createCmd.ExecuteNonQuery();

            Console.WriteLine($"Created database: {databaseName}");
        }
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        context.Database.Migrate();
    }
}
