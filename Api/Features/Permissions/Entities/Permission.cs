using Api.Features.Abstractions;
using Api.Features.RolePermissions.Entities;
using Api.Features.Shared;

namespace Api.Features.Permissions.Entities;

public class Permission : Entity
{
    public static readonly Permission Read = new(
        Guid.Parse("01960516-468d-7f11-90f2-e166cfda751f"),
        Name.Create("Read").Value
    );

    public static readonly Permission Write = new(
        Guid.Parse("01960516-cbce-7251-b410-071a6dd1a200"),
        Name.Create("Write").Value
    );

    public static readonly Permission Delete = new(
        Guid.Parse("01960517-4409-7812-a4c4-3b3c1b3c676c"),
        Name.Create("Delete").Value
    );

    public static readonly Permission Admin = new(
        Guid.Parse("01960518-2c6e-733c-a464-994496458d89"),
        Name.Create("Admin").Value
    );

    private Permission(Guid id, Name name)
        : base(id)
    {
        Name = name;
    }

    public Name Name { get; private set; }

    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = [];

    public static Result<Permission> Create(Name name)
    {
        var permission = new Permission(Guid.NewGuid(), name);

        return permission;
    }
}
