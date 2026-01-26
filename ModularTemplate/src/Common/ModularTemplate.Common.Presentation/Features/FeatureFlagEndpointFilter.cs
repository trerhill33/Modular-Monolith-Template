using Microsoft.AspNetCore.Http;
using ModularTemplate.Common.Application.Features;

namespace ModularTemplate.Common.Presentation.Features;

/// <summary>
/// Endpoint filter that checks if a feature is enabled before allowing the request to proceed.
/// Returns 404 Not Found if the feature is disabled, making the endpoint appear non-existent.
/// </summary>
internal sealed class FeatureFlagEndpointFilter : IEndpointFilter
{
    private readonly string _featureName;

    public FeatureFlagEndpointFilter(string featureName)
    {
        _featureName = featureName;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var featureFlagService = context.HttpContext.RequestServices
            .GetService(typeof(IFeatureFlagService)) as IFeatureFlagService;

        if (featureFlagService is null)
        {
            // If the service is not registered, allow the request (fail open for development)
            return await next(context);
        }

        var isEnabled = await featureFlagService.IsEnabledAsync(_featureName, context.HttpContext.RequestAborted);

        if (!isEnabled)
        {
            // Return 404 to hide the existence of the endpoint when feature is disabled
            return Microsoft.AspNetCore.Http.Results.NotFound();
        }

        return await next(context);
    }
}
