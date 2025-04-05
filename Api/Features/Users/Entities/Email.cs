﻿using System.Globalization;
using Api.Features.Abstractions;

namespace Api.Features.Users.Entities;

public sealed record Email
{
    public static readonly int MaxLength = 255;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static explicit operator string(Email email) => email.Value;

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Result.Failure<Email>(Errors.Empty(nameof(email)));
        }

        email = email.ToLower(CultureInfo.CurrentCulture);

        if (email.Split('@').Length != 2)
        {
            return Result.Failure<Email>(Errors.Invalid(nameof(email)));
        }

        if (email.Length > MaxLength)
        {
            return Result.Failure<Email>(Errors.TooLong(nameof(email), MaxLength));
        }

        return new Email(email);
    }
}
