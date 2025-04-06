using Api.Features.Abstractions;
using Api.Features.Roles.Entities;
using Api.Features.Shared;

namespace Api.Features.Permissions.Entities;

public class Permission : Entity
{
    private Permission(Guid id, Name name)
        : base(id)
    {
        Name = name;
    }

    public Name Name { get; private set; }

    public virtual ICollection<Role> Roles { get; private set; } = [];

    public static Result<Permission> Create(Name name)
    {
        var permission = new Permission(Guid.NewGuid(), name);

        return permission;
    }
}
