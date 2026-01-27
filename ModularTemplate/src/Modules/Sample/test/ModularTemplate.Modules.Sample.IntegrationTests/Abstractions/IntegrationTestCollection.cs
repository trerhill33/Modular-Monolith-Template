using ModularTemplate.Common.IntegrationTests.Abstractions;

namespace ModularTemplate.Modules.Sample.IntegrationTests.Abstractions;

/// <summary>
/// Collection fixture for Sample module integration tests.
/// Uses the shared IntegrationTestWebAppFactory from Common.IntegrationTests.
/// </summary>
[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;
