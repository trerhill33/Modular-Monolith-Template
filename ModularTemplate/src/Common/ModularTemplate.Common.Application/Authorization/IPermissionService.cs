using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Common.Application.Authorization;

/// <summary>
/// Service for retrieving user permissions.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Gets the permissions for a user.
    /// </summary>
    Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId);
}
