using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace ModularTemplate.Common.IntegrationTests.DatabaseProviders;

public interface ITestDatabaseProvider : IAsyncLifetime
{
    string ProviderName { get; }

    void ConfigureWebHost(IWebHostBuilder builder);
    void SetServiceProvider(IServiceProvider serviceProvider);
    Task ResetAsync();
}
