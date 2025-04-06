namespace Api.Features.Users.Entities;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    DateTime CreatedOnUtc
);
