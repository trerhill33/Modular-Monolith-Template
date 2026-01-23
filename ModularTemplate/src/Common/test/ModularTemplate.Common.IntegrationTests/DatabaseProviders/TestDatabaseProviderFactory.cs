namespace ModularTemplate.Common.IntegrationTests.DatabaseProviders;

public enum TestDatabaseType
{
    PostgreSql,
    InMemory
}

public static class TestDatabaseProviderFactory
{
    public static ITestDatabaseProvider Create(TestDatabaseType? type = null)
    {
        var providerType = type ?? GetProviderFromEnvironment();

        return providerType switch
        {
            TestDatabaseType.PostgreSql => new PostgreSqlTestDatabaseProvider(),
            TestDatabaseType.InMemory => new InMemoryTestDatabaseProvider(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private static TestDatabaseType GetProviderFromEnvironment()
    {
        var envValue = Environment.GetEnvironmentVariable("TEST_DB_PROVIDER");

        if (string.IsNullOrEmpty(envValue))
        {
            return IsDockerAvailable() ? TestDatabaseType.PostgreSql : TestDatabaseType.InMemory;
        }

        return envValue.ToLowerInvariant() switch
        {
            "postgresql" or "postgres" or "docker" => TestDatabaseType.PostgreSql,
            "inmemory" or "memory" or "ef" => TestDatabaseType.InMemory,
            _ => TestDatabaseType.PostgreSql
        };
    }

    private static bool IsDockerAvailable()
    {
        try
        {
            using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "info",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            process?.WaitForExit(5000);
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
