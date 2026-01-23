using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace ModularTemplate.Common.Infrastructure.Authentication;

/// <summary>
/// Configures JWT Bearer authentication options from <see cref="AuthenticationOptions"/>.
/// </summary>
/// <remarks>
/// Uses the post-configured <see cref="AuthenticationOptions"/> which derives values
/// from <c>Application</c> settings when not explicitly set.
/// </remarks>
internal sealed class JwtBearerConfigureOptions(IOptions<AuthenticationOptions> authOptions)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authOptions = authOptions.Value;

    public void Configure(JwtBearerOptions options) => Configure(string.Empty, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.Audience = _authOptions.Audience;
        options.MetadataAddress = _authOptions.MetadataAddress;
        options.RequireHttpsMetadata = _authOptions.RequireHttpsMetadata;
    }
}
