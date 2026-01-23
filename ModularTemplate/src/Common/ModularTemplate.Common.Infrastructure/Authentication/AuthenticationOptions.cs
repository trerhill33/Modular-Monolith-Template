namespace ModularTemplate.Common.Infrastructure.Authentication;

/// <summary>
/// Authentication configuration options.
/// </summary>
/// <remarks>
/// Values can be explicitly set or derived from <c>Application</c> settings.
/// <list type="bullet">
///   <item><c>Audience</c>: Defaults to {Application:ShortName}-api</item>
///   <item><c>KeycloakRealm</c>: Defaults to {Application:ShortName}</item>
/// </list>
/// </remarks>
public sealed class AuthenticationOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Authentication";

    /// <summary>
    /// Gets or sets the JWT audience claim.
    /// If not set, derived from Application:ShortName with "-api" suffix.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OpenID Connect metadata address.
    /// </summary>
    public string MetadataAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Keycloak base URL (e.g., "http://localhost:8080").
    /// Used to construct MetadataAddress if not explicitly set.
    /// </summary>
    public string KeycloakBaseUrl { get; set; } = "http://localhost:8080";

    /// <summary>
    /// Gets or sets the Keycloak realm name.
    /// If not set, derived from Application:ShortName.
    /// </summary>
    public string KeycloakRealm { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether HTTPS is required for metadata retrieval.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;
}
