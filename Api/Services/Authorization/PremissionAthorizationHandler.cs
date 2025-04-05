using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Authorization;

public class PermissionAuthorizationHandler(IUnitOfWork unitOfWork)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return;
        }

        Guid userId = context.User.GetUserId();

        User? user = await unitOfWork
            .Users.Where(x => x.Id == userId)
            .Include(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return;
        }

        var userPermissions = user.Role.RolePermissions.Select(x => x.Permission.Name.Value).ToHashSet();

        if (userPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
