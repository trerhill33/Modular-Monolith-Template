using ModularTemplate.Common.Presentation;

namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for exception handling configuration.
/// </summary>
internal static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Adds global exception handling services.
    /// </summary>
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    /// <summary>
    /// Uses global exception handling middleware.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();

        return app;
    }
}
