using Api.Features.Abstractions;
using Api.Features.RolePermissions.Entities;
using Api.Features.Shared;
using Api.Features.Users.Entities;

namespace Api.Features.Roles.Entities;

public class Role : Entity
{
    public static readonly Role User = new(Guid.Parse("01960513-664b-7304-92de-d2544cfa1ff7"), Name.Create("User").Value);

    public static readonly Role Admin = new(Guid.Parse("01960514-3085-7e45-a2ce-dc3336bdbac3"), Name.Create("Admin").Value);

    private Role(Guid id, Name name)
        : base(id)
    {
        Name = name;
    }

    public Name Name { get; private set; }

    public virtual ICollection<User> Users { get; private set; } = [];

    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = [];

    public static Result<Role> Create(Name name)
    {
        var role = new Role(Guid.CreateVersion7(), name);

        return role;
    }
}
