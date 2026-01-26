using Microsoft.Extensions.Configuration;
using ModularTemplate.Common.Application.Features;

namespace ModularTemplate.Common.Infrastructure.Features;

/// <summary>
/// Feature flag service that reads feature flags from IConfiguration.
/// Supports hierarchical configuration paths using colon notation (e.g., "Sales:Features:CatalogV2Pagination").
/// </summary>
internal sealed class ConfigurationFeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;

    public ConfigurationFeatureFlagService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public Task<bool> IsEnabledAsync(string featureName, CancellationToken ct = default)
    {
        return Task.FromResult(IsEnabled(featureName));
    }

    /// <inheritdoc />
    public bool IsEnabled(string featureName)
    {
        // Get the value from configuration
        var value = _configuration[featureName];

        // If the value is not set, default to false (feature disabled)
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // Parse as boolean, default to false if parsing fails
        return bool.TryParse(value, out var isEnabled) && isEnabled;
    }
}
