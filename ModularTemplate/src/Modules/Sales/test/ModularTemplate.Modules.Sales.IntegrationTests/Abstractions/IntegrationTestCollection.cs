using ModularTemplate.Common.IntegrationTests.Abstractions;

namespace ModularTemplate.Modules.Sales.IntegrationTests.Abstractions;

/// <summary>
/// Collection fixture for Sales module integration tests.
/// Uses the shared IntegrationTestWebAppFactory from Common.IntegrationTests.
/// </summary>
[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;
