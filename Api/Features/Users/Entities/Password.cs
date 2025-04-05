using Api.Features.Abstractions;

namespace Api.Features.Users.Entities;

public record Password
{
    public static readonly int MinLength = 8;

    public static readonly int MaxLength = 255;

    private Password(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static implicit operator string(Password password) => password.Value;

    public static Result<Password> Create(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return Result.Failure<Password>(Errors.Empty(nameof(password)));
        }

        if (password.Length < MinLength)
        {
            return Result.Failure<Password>(Errors.TooShort(nameof(password), MinLength));
        }

        if (password.Length > MaxLength)
        {
            return Result.Failure<Password>(Errors.TooLong(nameof(password), MaxLength));
        }

        return new Password(password);
    }
}
