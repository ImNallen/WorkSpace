using Api.Features.Abstractions;
using Api.Features.Roles.Entities;
using Api.Features.Users.Events;
using Api.Services.Time;

namespace Api.Features.Users.Entities;

public class User : Entity
{
    private User(Guid id, Email email, Password password, Guid roleId)
        : base(id)
    {
        Email = email;
        Password = password;
        RoleId = roleId;
    }

    private User() { }

    public Email Email { get; private set; }

    public Password Password { get; private set; }

    public Guid RoleId { get; private set; }

    public virtual Role Role { get; private set; } = null!;

    public static User Create(Email email, Password password, Guid roleId, DateTime occuredOnUtc)
    {
        var user = new User(Guid.NewGuid(), email, password, roleId);

        user.RaiseDomainEvent(
            new UserCreatedDomainEvent(Guid.CreateVersion7(), occuredOnUtc, user)
        );

        return user;
    }
}
