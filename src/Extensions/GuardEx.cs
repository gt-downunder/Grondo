using System.Numerics;
using System.Text.RegularExpressions;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides fluent guard clause extension methods for argument validation.
    /// </summary>
    public static class GuardEx
    {
        extension<T>(T? value) where T : class
        {
            /// <summary>
            /// Throws <see cref="ArgumentNullException"/> if the value is null.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The non-null value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
            public T ThrowIfNull(string? paramName = null) =>
                value ?? throw new ArgumentNullException(paramName);
        }

        extension(string? value)
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the string is null or empty.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The non-null, non-empty string value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the value is empty.</exception>
            public string ThrowIfNullOrEmpty(string? paramName = null) =>
                string.IsNullOrEmpty(value)
                    ? throw (value is null
                        ? new ArgumentNullException(paramName)
                        : new ArgumentException("Value cannot be empty.", paramName))
                    : value;

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the string is null, empty, or whitespace.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The non-null, non-empty string value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the value is empty or whitespace.</exception>
            public string ThrowIfNullOrWhiteSpace(string? paramName = null) =>
                string.IsNullOrWhiteSpace(value)
                    ? throw (value is null
                        ? new ArgumentNullException(paramName)
                        : new ArgumentException("Value cannot be empty or whitespace.", paramName))
                    : value;

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the string exceeds maximum length.
            /// </summary>
            /// <param name="maxLength">The maximum allowed length.</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated string value.</returns>
            /// <exception cref="ArgumentException">Thrown if the string exceeds maximum length.</exception>
            public string ThrowIfTooLong(int maxLength, string? paramName = null)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLength, nameof(maxLength));

                if (value?.Length > maxLength)
                    throw new ArgumentException(
                        $"Value cannot exceed {maxLength} characters. Actual: {value.Length}",
                        paramName);

                return value!;
            }

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the string is shorter than minimum length.
            /// </summary>
            /// <param name="minLength">The minimum required length.</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated string value.</returns>
            /// <exception cref="ArgumentException">Thrown if the string is shorter than minimum length.</exception>
            public string ThrowIfTooShort(int minLength, string? paramName = null)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(minLength, nameof(minLength));

                if (value?.Length < minLength)
                    throw new ArgumentException(
                        $"Value must be at least {minLength} characters. Actual: {(value is null ? 0 : value.Length)}",
                        paramName);

                return value!;
            }

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the string does not match the specified regex pattern.
            /// </summary>
            /// <param name="regexPattern">The regular expression pattern to validate against.</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated string value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the value or pattern is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the value does not match the pattern.</exception>
            public string ThrowIfInvalidFormat(string regexPattern, string? paramName = null)
            {
                ArgumentNullException.ThrowIfNull(value, paramName);
                ArgumentNullException.ThrowIfNull(regexPattern);
                if (!Regex.IsMatch(value, regexPattern))
                    throw new ArgumentException($"Value does not match the required format '{regexPattern}'.", paramName);
                return value;
            }
        }

        extension<T>(T value) where T : struct
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the value equals its type's default.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The non-default value.</returns>
            /// <exception cref="ArgumentException">Thrown if the value is the default value.</exception>
            public T ThrowIfDefault(string? paramName = null) =>
                EqualityComparer<T>.Default.Equals(value, default)
                    ? throw new ArgumentException("Value cannot be the default value.", paramName)
                    : value;
        }

        extension<T>(IEnumerable<T>? value)
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the collection is null or empty.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The non-null, non-empty collection.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the value is empty.</exception>
            public IEnumerable<T> ThrowIfEmpty(string? paramName = null)
            {
                ArgumentNullException.ThrowIfNull(value, paramName);
                if (!value.Any()) throw new ArgumentException("Collection cannot be empty.", paramName);
                return value;
            }

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the collection contains the specified item.
            /// </summary>
            /// <param name="item">The item to check for.</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated collection.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the collection contains the item.</exception>
            public IEnumerable<T> ThrowIfContains(T item, string? paramName = null)
            {
                ArgumentNullException.ThrowIfNull(value, paramName);

                if (value.Contains(item))
                    throw new ArgumentException(
                        $"Collection cannot contain {item}",
                        paramName);

                return value;
            }

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the collection does not contain the specified item.
            /// </summary>
            /// <param name="item">The item to check for.</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated collection.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the collection does not contain the item.</exception>
            public IEnumerable<T> ThrowIfDoesNotContain(T item, string? paramName = null)
            {
                ArgumentNullException.ThrowIfNull(value, paramName);

                if (!value.Contains(item))
                    throw new ArgumentException(
                        $"Collection must contain {item}",
                        paramName);

                return value;
            }
        }

        extension<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the enum value is not defined.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated enum value.</returns>
            /// <exception cref="ArgumentException">Thrown if the value is not a defined enum value.</exception>
            public TEnum ThrowIfNotDefined(string? paramName = null)
            {
                if (!Enum.IsDefined(value))
                    throw new ArgumentException(
                        $"Value {value} is not a valid {typeof(TEnum).Name}",
                        paramName);

                return value;
            }
        }

        extension(DateTime value)
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the date is in the past.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated date value.</returns>
            /// <exception cref="ArgumentException">Thrown if the date is in the past.</exception>
            public DateTime ThrowIfInPast(string? paramName = null)
            {
                if (value < DateTime.UtcNow)
                    throw new ArgumentException(
                        $"Date cannot be in the past. Value: {value:O}",
                        paramName);

                return value;
            }

            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the date is in the future.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated date value.</returns>
            /// <exception cref="ArgumentException">Thrown if the date is in the future.</exception>
            public DateTime ThrowIfInFuture(string? paramName = null)
            {
                if (value > DateTime.UtcNow)
                    throw new ArgumentException(
                        $"Date cannot be in the future. Value: {value:O}",
                        paramName);

                return value;
            }

            /// <summary>
            /// Throws <see cref="ArgumentOutOfRangeException"/> if the date is not within the specified range.
            /// </summary>
            /// <param name="min">The minimum allowed date (inclusive).</param>
            /// <param name="max">The maximum allowed date (inclusive).</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated date value.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the date is outside the range.</exception>
            public DateTime ThrowIfNotInRange(DateTime min, DateTime max, string? paramName = null)
            {
                if (value < min || value > max)
                    throw new ArgumentOutOfRangeException(
                        paramName,
                        value,
                        $"Date must be between {min:O} and {max:O}");

                return value;
            }
        }

        extension(Guid value)
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the GUID is empty.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated GUID value.</returns>
            /// <exception cref="ArgumentException">Thrown if the GUID is empty.</exception>
            public Guid ThrowIfEmpty(string? paramName = null)
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("GUID cannot be empty", paramName);

                return value;
            }
        }

        extension<T>(T value) where T : INumber<T>
        {
            /// <summary>
            /// Throws <see cref="ArgumentOutOfRangeException"/> if the value is negative (less than zero).
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated non-negative value.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
            public T ThrowIfNegative(string? paramName = null) =>
                value < T.Zero
                    ? throw new ArgumentOutOfRangeException(paramName, value, "Value must not be negative.")
                    : value;

            /// <summary>
            /// Throws <see cref="ArgumentOutOfRangeException"/> if the value is negative or zero.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated positive value.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative or zero.</exception>
            public T ThrowIfNegativeOrZero(string? paramName = null) =>
                value <= T.Zero
                    ? throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.")
                    : value;

            /// <summary>
            /// Throws <see cref="ArgumentOutOfRangeException"/> if the value is zero.
            /// </summary>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated non-zero value.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is zero.</exception>
            public T ThrowIfZero(string? paramName = null) =>
                value == T.Zero
                    ? throw new ArgumentOutOfRangeException(paramName, value, "Value must not be zero.")
                    : value;

            /// <summary>
            /// Throws <see cref="ArgumentOutOfRangeException"/> if the value is outside the specified range [min, max].
            /// </summary>
            /// <param name="min">The minimum allowed value (inclusive).</param>
            /// <param name="max">The maximum allowed value (inclusive).</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated value within range.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range.</exception>
            public T ThrowIfOutOfRange(T min, T max, string? paramName = null) =>
                value < min || value > max
                    ? throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.")
                    : value;
        }

        extension<T>(T value)
        {
            /// <summary>
            /// Throws <see cref="ArgumentException"/> if the predicate returns <c>true</c> for the value.
            /// </summary>
            /// <param name="predicate">The condition that, when true, causes an exception to be thrown.</param>
            /// <param name="message">The error message for the exception.</param>
            /// <param name="paramName">The name of the parameter.</param>
            /// <returns>The validated value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown if the predicate returns true.</exception>
            public T ThrowIf(Func<T, bool> predicate, string message, string? paramName = null)
            {
                ArgumentNullException.ThrowIfNull(predicate);
                if (predicate(value))
                    throw new ArgumentException(message, paramName);
                return value;
            }
        }
    }
}

