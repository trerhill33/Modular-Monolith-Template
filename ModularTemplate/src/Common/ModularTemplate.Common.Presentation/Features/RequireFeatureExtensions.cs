using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ModularTemplate.Common.Presentation.Features;

/// <summary>
/// Extension methods for requiring feature flags on endpoints.
/// </summary>
public static class RequireFeatureExtensions
{
    /// <summary>
    /// Requires that a feature flag is enabled for this endpoint.
    /// If the feature is disabled, the endpoint returns 404 Not Found.
    /// </summary>
    /// <param name="builder">The route handler builder.</param>
    /// <param name="featureName">The configuration path to the feature flag (e.g., "Sales:Features:CatalogV2Pagination").</param>
    /// <returns>The route handler builder for chaining.</returns>
    public static RouteHandlerBuilder RequireFeature(this RouteHandlerBuilder builder, string featureName)
    {
        builder.AddEndpointFilter(new FeatureFlagEndpointFilter(featureName));
        return builder;
    }
}
