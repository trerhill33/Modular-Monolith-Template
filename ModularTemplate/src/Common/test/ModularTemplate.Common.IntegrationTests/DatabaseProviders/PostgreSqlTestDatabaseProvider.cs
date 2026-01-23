using Microsoft.AspNetCore.Hosting;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace ModularTemplate.Common.IntegrationTests.DatabaseProviders;

public sealed class PostgreSqlTestDatabaseProvider : ITestDatabaseProvider
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithDatabase("modulartemplate_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7")
        .Build();

    private Respawner? _respawner;
    private NpgsqlConnection? _dbConnection;

    public string ProviderName => "PostgreSQL (Docker)";

    public void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        builder.UseSetting("ConnectionStrings:Cache", _redisContainer.GetConnectionString());
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        // Not needed for PostgreSQL - Respawn handles reset
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["sample", "orders", "sales"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner is null || _dbConnection is null)
        {
            await InitializeRespawnerAsync();
        }

        await _respawner!.ResetAsync(_dbConnection!);
    }

    public async Task DisposeAsync()
    {
        if (_dbConnection is not null)
        {
            await _dbConnection.DisposeAsync();
        }
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
