using Microsoft.AspNetCore.Http;
using ModularTemplate.Common.Application.Features;

namespace ModularTemplate.Common.Presentation.Features;

/// <summary>
/// Endpoint filter that checks if a feature is enabled before allowing the request to proceed.
/// Returns 404 Not Found if the feature is disabled, making the endpoint appear non-existent.
/// </summary>
internal sealed class FeatureFlagEndpointFilter(string featureName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (context.HttpContext.RequestServices
            .GetService(typeof(IFeatureFlagService)) is not IFeatureFlagService featureFlagService)
        {
            // If the service is not registered, allow the request (fail open for development)
            return await next(context);
        }

        var isEnabled = await featureFlagService.IsEnabledAsync(featureName, context.HttpContext.RequestAborted);

        if (!isEnabled)
        {
            return Microsoft.AspNetCore.Http.Results.NotFound();
        }

        return await next(context);
    }
}
