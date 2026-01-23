using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using ModularTemplate.Common.IntegrationTests.DatabaseProviders;
using Xunit;

namespace ModularTemplate.Common.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly ITestDatabaseProvider _databaseProvider;

    public IntegrationTestWebAppFactory()
    {
        _databaseProvider = TestDatabaseProviderFactory.Create();
        Console.WriteLine($"[IntegrationTests] Using database provider: {_databaseProvider.ProviderName}");
    }

    public string DatabaseProviderName => _databaseProvider.ProviderName;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _databaseProvider.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            // Common test service overrides can go here
        });
    }

    public async Task InitializeAsync()
    {
        await _databaseProvider.InitializeAsync();

        // Trigger app creation to run migrations
        _ = Services;

        // Pass service provider for reset operations
        _databaseProvider.SetServiceProvider(Services);
    }

    public async Task ResetDatabaseAsync() => await _databaseProvider.ResetAsync();

    public new async Task DisposeAsync()
    {
        await _databaseProvider.DisposeAsync();
        await base.DisposeAsync();
    }
}
