namespace Api.Features.Users.Entities;

public record UserDto(Guid Id, string Email, Guid RoleId);
