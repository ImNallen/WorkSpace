namespace Api.Features.Abstractions;

public static class Errors
{
    public static Error InvalidCredentials =>
        Error.Validation("Error.InvalidCredentials", "Invalid credentials.");

    public static Error NotVerified =>
        Error.Validation("Error.NotVerified", "User is not verified.");

    public static Error RefreshTokenExpired =>
        Error.Validation("Error.RefreshTokenExpired", "Refresh token expired.");

    public static Error EmailNotUnique =>
        Error.Validation("Error.EmailNotUnique", "Email is already in use.");

    public static Error RoleNotFound => Error.Validation("Error.RoleNotFound", "Role not found.");

    public static Error Empty(string name)
    {
        return Error.Validation("Errors.Empty", $"{name} can't be empty.");
    }

    public static Error Invalid(string name)
    {
        return Error.Validation("Error.Invalid", $"{name} was invalid.");
    }

    public static Error NotFound(string name)
    {
        return Error.NotFound("Error.NotFound", $"{name} not found.");
    }

    public static Error NotUnique(string name)
    {
        return Error.Conflict("Error.NotUnique", $"{name} is already in use.");
    }

    public static Error TooShort(string name, int minLength)
    {
        return Error.Validation(
            "Error.TooShort",
            $"{name} is too short. Min length is {minLength}."
        );
    }

    public static Error TooLong(string name, int maxLength)
    {
        return Error.Validation("Error.TooLong", $"{name} is too long. Max length is {maxLength}.");
    }
}
