using Rtl.Core.IntegrationTests.Abstractions;

namespace Rtl.Module.SampleSales.IntegrationTests.Abstractions;

/// <summary>
/// Collection fixture for SampleSales module integration tests.
/// Uses the shared IntegrationTestWebAppFactory from Common.IntegrationTests.
/// </summary>
[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;
