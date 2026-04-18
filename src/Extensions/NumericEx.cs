using System.Numerics;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for numeric types implementing <see cref="INumber{T}"/>.
    /// </summary>
    public static class NumericEx
    {
        private static readonly string[] _binarySuffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
        private static readonly string[] _siSuffixes = ["B", "kB", "MB", "GB", "TB", "PB", "EB"];

        extension<T>(T number) where T : INumber<T>
        {
            /// <summary>Determines whether the numeric value is equal to zero.</summary>
            /// <returns><c>true</c> if the value equals zero; otherwise, <c>false</c>.</returns>
            public bool IsZero() => number == T.Zero;

            /// <summary>Determines whether the numeric value is greater than zero.</summary>
            /// <returns><c>true</c> if the value is greater than zero; otherwise, <c>false</c>.</returns>
            public bool IsPositive() => number > T.Zero;

            /// <summary>Determines whether the numeric value is less than zero.</summary>
            /// <returns><c>true</c> if the value is less than zero; otherwise, <c>false</c>.</returns>
            public bool IsNegative() => number < T.Zero;

            /// <summary>
            /// Returns the numeric value clamped to the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
            /// </summary>
            /// <param name="min">The minimum allowed value.</param>
            /// <param name="max">The maximum allowed value.</param>
            /// <returns>The clamped value.</returns>
            /// <exception cref="ArgumentException">Thrown if <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
            public T ClampTo(T min, T max)
            {
                if (min > max)
                    throw new ArgumentException($"min ({min}) cannot be greater than max ({max}).", nameof(min));
                return T.Clamp(number, min, max);
            }

            /// <summary>
            /// Determines whether the numeric value falls within the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
            /// </summary>
            /// <param name="min">The minimum allowed value (inclusive).</param>
            /// <param name="max">The maximum allowed value (inclusive).</param>
            /// <returns><c>true</c> if the value is in range; otherwise, <c>false</c>.</returns>
            public bool IsBetween(T min, T max) =>
                number >= min && number <= max;
        }

        extension(double number)
        {
            /// <summary>
            /// Rounds the value to the specified number of decimal places.
            /// </summary>
            /// <param name="decimals">The number of fractional digits (0-15).</param>
            /// <param name="mode">The rounding strategy. Defaults to <see cref="MidpointRounding.ToEven"/>.</param>
            /// <returns>The rounded value.</returns>
            public double RoundTo(int decimals, MidpointRounding mode = MidpointRounding.ToEven) =>
                Math.Round(number, decimals, mode);

            /// <summary>
            /// Calculates <paramref name="number"/> as a percentage of <paramref name="total"/>.
            /// </summary>
            /// <param name="total">The total value that represents 100%.</param>
            /// <returns>The percentage of <paramref name="total"/>, or 0 when <paramref name="total"/> is 0.</returns>
            public double PercentageOf(double total) =>
                Math.Abs(total) < double.Epsilon ? 0d : number / total * 100d;
        }

        extension(decimal number)
        {
            /// <summary>
            /// Rounds the value to the specified number of decimal places.
            /// </summary>
            /// <param name="decimals">The number of fractional digits.</param>
            /// <param name="mode">The rounding strategy. Defaults to <see cref="MidpointRounding.ToEven"/>.</param>
            /// <returns>The rounded value.</returns>
            public decimal RoundTo(int decimals, MidpointRounding mode = MidpointRounding.ToEven) =>
                Math.Round(number, decimals, mode);

            /// <summary>
            /// Calculates <paramref name="number"/> as a percentage of <paramref name="total"/>.
            /// </summary>
            /// <param name="total">The total value that represents 100%.</param>
            /// <returns>The percentage of <paramref name="total"/>, or 0 when <paramref name="total"/> is 0.</returns>
            public decimal PercentageOf(decimal total) =>
                total == 0m ? 0m : number / total * 100m;
        }

        extension(long bytes)
        {
            /// <summary>
            /// Converts a byte count to a human-readable size string such as "1.5 KB" or "3.2 MB".
            /// </summary>
            /// <param name="useSI">If <c>true</c>, uses SI (1000-based) units; otherwise uses binary (1024-based) units.</param>
            /// <returns>A human-readable byte size string.</returns>
            public string ToHumanByteSize(bool useSI = false)
            {
                if (bytes == 0) return "0 B";

                int divisor = useSI ? 1000 : 1024;
                string[] suffixes = useSI ? _siSuffixes : _binarySuffixes;
                double abs = Math.Abs(bytes);
                int order = (int)Math.Floor(Math.Log(abs, divisor));
                order = Math.Min(order, suffixes.Length - 1);
                double value = abs / Math.Pow(divisor, order);

                string formatted = Math.Abs(value - Math.Round(value)) < 0.01
                    ? value.ToString("F0", System.Globalization.CultureInfo.InvariantCulture)
                    : value.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
                return bytes < 0 ? $"-{formatted} {suffixes[order]}" : $"{formatted} {suffixes[order]}";
            }
        }

        extension(int number)
        {
            /// <summary>
            /// Converts the integer to its ordinal string representation (e.g., 1 → "1st", 2 → "2nd", 3 → "3rd").
            /// </summary>
            /// <returns>The ordinal string representation.</returns>
            public string ToOrdinal()
            {
                int abs = Math.Abs(number);
                int lastTwo = abs % 100;
                int lastOne = abs % 10;

                string suffix = lastTwo is >= 11 and <= 13
                    ? "th"
                    : lastOne switch
                    {
                        1 => "st",
                        2 => "nd",
                        3 => "rd",
                        _ => "th"
                    };

                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}", number, suffix);
            }

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of days.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of days.</returns>
            public TimeSpan Days() => TimeSpan.FromDays(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of hours.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of hours.</returns>
            public TimeSpan Hours() => TimeSpan.FromHours(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of minutes.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of minutes.</returns>
            public TimeSpan Minutes() => TimeSpan.FromMinutes(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of seconds.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of seconds.</returns>
            public TimeSpan Seconds() => TimeSpan.FromSeconds(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of milliseconds.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of milliseconds.</returns>
            public TimeSpan Milliseconds() => TimeSpan.FromMilliseconds(number);
        }

        extension(double number)
        {
            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of days.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of days.</returns>
            public TimeSpan Days() => TimeSpan.FromDays(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of hours.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of hours.</returns>
            public TimeSpan Hours() => TimeSpan.FromHours(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of minutes.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of minutes.</returns>
            public TimeSpan Minutes() => TimeSpan.FromMinutes(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of seconds.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of seconds.</returns>
            public TimeSpan Seconds() => TimeSpan.FromSeconds(number);

            /// <summary>Creates a <see cref="TimeSpan"/> representing the specified number of milliseconds.</summary>
            /// <returns>A <see cref="TimeSpan"/> of the specified number of milliseconds.</returns>
            public TimeSpan Milliseconds() => TimeSpan.FromMilliseconds(number);
        }
    }
}

