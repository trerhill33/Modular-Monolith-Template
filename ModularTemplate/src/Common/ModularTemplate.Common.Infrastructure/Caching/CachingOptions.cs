namespace ModularTemplate.Common.Infrastructure.Caching;

/// <summary>
/// Configuration options for distributed caching.
/// </summary>
public sealed class CachingOptions
{
    /// <summary>
    /// The configuration section name for caching options.
    /// </summary>
    public const string SectionName = "Caching";

    /// <summary>
    /// Gets the default cache expiration time in minutes.
    /// </summary>
    public int DefaultExpirationMinutes { get; init; }
}
