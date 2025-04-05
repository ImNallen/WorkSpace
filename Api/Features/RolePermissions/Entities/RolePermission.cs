using Api.Features.Abstractions;
using Api.Features.Permissions.Entities;
using Api.Features.Roles.Entities;

namespace Api.Features.RolePermissions.Entities;

public class RolePermission : Entity
{
    public static readonly RolePermission UserRead = new(Guid.Parse("0196051a-17cf-72b6-b98d-4cb5a6c65b83"), Roles.Entities.Role.User.Id, Permissions.Entities.Permission.Read.Id);

    public static readonly RolePermission UserWrite = new(Guid.Parse("0196051a-5fce-7dd1-bd5d-03280dcc27e9"), Roles.Entities.Role.User.Id, Permissions.Entities.Permission.Write.Id);

    public static readonly RolePermission UserDelete = new(Guid.Parse("0196051a-a482-7439-9b61-41d1c45fd105"), Roles.Entities.Role.User.Id, Permissions.Entities.Permission.Delete.Id);

    public static readonly RolePermission AdminRead = new(Guid.Parse("0196051b-0f46-73e7-8da9-5a467b71157a"), Roles.Entities.Role.Admin.Id, Permissions.Entities.Permission.Read.Id);

    public static readonly RolePermission AdminWrite = new(Guid.Parse("0196051b-6543-7f2f-92da-37bcef6eb2d3"), Roles.Entities.Role.Admin.Id, Permissions.Entities.Permission.Write.Id);

    public static readonly RolePermission AdminDelete = new(Guid.Parse("0196051b-ada9-71ab-9022-4e99496803e0"), Roles.Entities.Role.Admin.Id, Permissions.Entities.Permission.Delete.Id);

    public static readonly RolePermission AdminAdmin = new(Guid.Parse("0196051b-ebe8-79e9-8913-459bb1cebfb5"), Roles.Entities.Role.Admin.Id, Permissions.Entities.Permission.Admin.Id);

    private RolePermission(Guid id, Guid roleId, Guid permissionId)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public virtual Role Role { get; private set; } = null!;

    public virtual Permission Permission { get; private set; } = null!;

    public static RolePermission Create(Guid roleId, Guid permissionId)
    {
        return new RolePermission(Guid.NewGuid(), roleId, permissionId);
    }
}
