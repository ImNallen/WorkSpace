using System.Globalization;
using Api.Features.Abstractions;

namespace Api.Features.Shared;

public sealed record Name
{
    public static readonly int MaxLength = 255;

    public static readonly int MinLength = 2;

    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static explicit operator string(Name name) => name.Value;

    public static Result<Name> Create(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Name>(Errors.Empty(nameof(name)));
        }

        name = name.ToLower(CultureInfo.CurrentCulture);

        if (name.Length > MaxLength)
        {
            return Result.Failure<Name>(Errors.TooLong(nameof(Name), MaxLength));
        }

        if (name.Length < MinLength)
        {
            return Result.Failure<Name>(Errors.TooShort(nameof(Name), MinLength));
        }

        return new Name(name);
    }
}
