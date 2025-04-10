using Microsoft.AspNetCore.Authorization;

namespace Api.Services.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
