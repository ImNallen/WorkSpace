using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Api.Features.Abstractions;

public static class Ensure
{
    public static void NotNullOrEmpty(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = default
    )
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(paramName);
        }
    }

    public static void NotNull(
        [NotNull] object? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = default
    )
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    public static void GreaterThanZero(
        decimal value,
        [CallerArgumentExpression(nameof(value))] string? paramName = default
    )
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }
    }

    public static void StartDatePrecedesEndDate(
        DateTime start,
        DateTime end,
        [CallerArgumentExpression(nameof(end))] string? paramName = default
    )
    {
        if (start >= end)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }
    }
}
