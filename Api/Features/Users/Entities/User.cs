using Api.Features.Abstractions;
using Api.Features.Roles.Entities;
using Api.Features.Shared;
using Api.Features.Users.Events;
using Api.Services.Time;

namespace Api.Features.Users.Entities;

public class User : Entity
{
    private User(
        Guid id,
        Email email,
        Password password,
        Guid roleId,
        Name firstName,
        Name lastName,
        DateTime createdOnUtc
    )
        : base(id)
    {
        Email = email;
        Password = password;
        RoleId = roleId;
        FirstName = firstName;
        LastName = lastName;
        CreatedOnUtc = createdOnUtc;
    }

    private User() { }

    public Email Email { get; private set; }

    public Password Password { get; private set; }

    public Name FirstName { get; private set; }

    public Name LastName { get; private set; }

    public Guid RoleId { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public virtual Role Role { get; private set; } = null!;

    public static User Create(
        Email email,
        Password password,
        Guid roleId,
        Name firstName,
        Name lastName,
        DateTime createdOnUtc
    )
    {
        var user = new User(
            Guid.NewGuid(),
            email,
            password,
            roleId,
            firstName,
            lastName,
            createdOnUtc
        );

        user.RaiseDomainEvent(
            new UserCreatedDomainEvent(Guid.CreateVersion7(), createdOnUtc, user)
        );

        return user;
    }
}
