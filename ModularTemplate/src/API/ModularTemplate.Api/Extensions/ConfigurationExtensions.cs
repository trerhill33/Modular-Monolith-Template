namespace ModularTemplate.Api.Extensions;

/// <summary>
/// Extension methods for configuring module-specific settings.
/// </summary>
internal static class ConfigurationExtensions
{
    /// <summary>
    /// Adds module-specific configuration files to the configuration builder.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each module has its own configuration file following the pattern:
    /// - modules.{module}.json (base configuration, required)
    /// - modules.{module}.{environment}.json (environment overrides, optional)
    /// </para>
    /// <para>
    /// Configuration is layered: base file is loaded first, then environment-specific
    /// file is merged on top, allowing environment overrides while sharing common defaults.
    /// </para>
    /// </remarks>
    internal static void AddModuleConfiguration(
        this IConfigurationBuilder configurationBuilder,
        string[] modules,
        string environment)
    {
        foreach (var module in modules)
        {
            // Base module config (required)
            configurationBuilder.AddJsonFile(
                $"modules.{module}.json",
                 false,
                 true);

            // Environment-specific override (optional)
            configurationBuilder.AddJsonFile(
                $"modules.{module}.{environment}.json",
                true,
                true);
        }
    }
}
