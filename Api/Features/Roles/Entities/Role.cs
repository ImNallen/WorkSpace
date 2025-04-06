using Api.Features.Abstractions;
using Api.Features.Permissions.Entities;
using Api.Features.Shared;
using Api.Features.Users.Entities;

namespace Api.Features.Roles.Entities;

public class Role : Entity
{
    private Role(Guid id, Name name, List<Permission> permissions)
        : base(id)
    {
        Name = name;
        Permissions = permissions;
    }

    private Role() { }

    public Name Name { get; private set; }

    public virtual ICollection<User> Users { get; private set; } = [];

    public virtual ICollection<Permission> Permissions { get; private set; }

    public static Result<Role> Create(Name name, List<Permission> permissions)
    {
        var role = new Role(Guid.CreateVersion7(), name, permissions);

        return role;
    }

    public Result AddPermission(Permission permission)
    {
        if (Permissions.Any(p => p.Id == permission.Id))
        {
            return Result.Failure(Errors.NotUnique(permission.Name.Value));
        }

        Permissions.Add(permission);

        return Result.Success();
    }

    public void RemovePermission(Permission permission)
    {
        Permissions.Remove(permission);
    }
}
