using Microsoft.AspNetCore.Authorization;

namespace ModularTemplate.Common.Infrastructure.Authorization;

/// <summary>
/// Authorization requirement for a specific permission.
/// </summary>
internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
