using System.Globalization;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DateTime"/> formatting, parsing, and week arithmetic.
    /// </summary>
    public static class DateTimeEx
    {
        /// <summary>
        /// Standard date format string: "yyyy-MM-dd".
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Standard date-time format string: "yyyy-MM-ddTHH:mm:ss".
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        extension(DateTime date)
        {
            /// <summary>
            /// Formats the specified <see cref="DateTime"/> as a string using the standard date format ("yyyy-MM-dd").
            /// </summary>
            /// <returns>A string representation of the date.</returns>
            public string ToFormattedDate() =>
                date.ToString(DateFormat, CultureInfo.InvariantCulture);

            /// <summary>
            /// Formats the specified <see cref="DateTime"/> as a string using the standard date-time format ("yyyy-MM-ddTHH:mm:ss").
            /// </summary>
            /// <returns>A string representation of the date-time.</returns>
            public string ToFormattedDateTime() =>
                date.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

            /// <summary>
            /// Adds the specified number of weeks to the given <see cref="DateTime"/>.
            /// </summary>
            /// <param name="weeks">The number of weeks to add.</param>
            /// <returns>A new <see cref="DateTime"/> offset by the specified number of weeks.</returns>
            public DateTime AddWeeks(int weeks) =>
                date.AddDays((double)weeks * 7);

            /// <summary>
            /// Returns a new <see cref="DateTime"/> with milliseconds truncated.
            /// Useful for scenarios where precision beyond seconds is not required.
            /// </summary>
            /// <returns>A new <see cref="DateTime"/> with milliseconds set to zero.</returns>
            public DateTime TruncateMilliseconds() =>
                new(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);

            /// <summary>
            /// Determines whether the specified <see cref="DateTime"/> falls on a weekday (Monday through Friday).
            /// </summary>
            /// <returns><c>true</c> if the date is a weekday; otherwise, <c>false</c>.</returns>
            public bool IsWeekday() =>
                date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;

            /// <summary>
            /// Determines whether the specified <see cref="DateTime"/> falls on a weekend (Saturday or Sunday).
            /// </summary>
            /// <returns><c>true</c> if the date is a weekend; otherwise, <c>false</c>.</returns>
            public bool IsWeekend() =>
                date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

            /// <summary>
            /// Returns a new <see cref="DateTime"/> representing the start of the day (00:00:00.000).
            /// </summary>
            /// <returns>A new <see cref="DateTime"/> at midnight of the same day.</returns>
            public DateTime StartOfDay() =>
                date.Date;

            /// <summary>
            /// Returns a new <see cref="DateTime"/> representing the end of the day (23:59:59.9999999).
            /// </summary>
            /// <returns>A new <see cref="DateTime"/> at the last tick of the same day.</returns>
            public DateTime EndOfDay() =>
                date.Date.AddDays(1).AddTicks(-1);

            /// <summary>
            /// Returns a new <see cref="DateTime"/> representing the first day of the month.
            /// </summary>
            /// <returns>A new <see cref="DateTime"/> at the start of the first day of the month.</returns>
            public DateTime StartOfMonth() =>
                new(date.Year, date.Month, 1, 0, 0, 0, date.Kind);

            /// <summary>
            /// Returns a new <see cref="DateTime"/> representing the last day of the month.
            /// </summary>
            /// <returns>A new <see cref="DateTime"/> at the end of the last day of the month.</returns>
            public DateTime EndOfMonth() =>
                new DateTime(date.Year, date.Month, 1, 0, 0, 0, date.Kind).AddMonths(1).AddTicks(-1);

            /// <summary>
            /// Determines whether the specified <see cref="DateTime"/> falls between two dates, inclusive.
            /// </summary>
            /// <param name="start">The start of the range.</param>
            /// <param name="end">The end of the range.</param>
            /// <returns><c>true</c> if the date is between <paramref name="start"/> and <paramref name="end"/>; otherwise, <c>false</c>.</returns>
            public bool IsBetween(DateTime start, DateTime end) =>
                date >= start && date <= end;

            /// <summary>
            /// Converts the specified <see cref="DateTime"/> to a human-readable relative time string
            /// such as "3 hours ago" or "in 2 days".
            /// </summary>
            /// <returns>A relative time string.</returns>
            public string ToRelativeTime() =>
                TimeSpanEx.FormatRelativeTime(DateTime.UtcNow - date.ToUniversalTime());
        }

        extension(DateTime? date)
        {
            /// <summary>
            /// Formats the specified nullable <see cref="DateTime"/> as a string using the standard date format ("yyyy-MM-dd").
            /// Returns an empty string if the value is <c>null</c>.
            /// </summary>
            /// <returns>A string representation of the date, or an empty string if the value is <c>null</c>.</returns>
            public string ToFormattedDate() =>
                date.HasValue ? date.Value.ToFormattedDate() : string.Empty;

            /// <summary>
            /// Formats the specified nullable <see cref="DateTime"/> as a string using the standard date-time format ("yyyy-MM-ddTHH:mm:ss").
            /// Returns an empty string if the value is <c>null</c>.
            /// </summary>
            /// <returns>A string representation of the date-time, or an empty string if the value is <c>null</c>.</returns>
            public string ToFormattedDateTime() =>
                date.HasValue ? date.Value.ToFormattedDateTime() : string.Empty;
        }

        extension(string date)
        {
            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-dd" into a <see cref="DateTime"/>.
            /// Returns <c>null</c> if parsing fails.
            /// </summary>
            /// <returns>A <see cref="DateTime"/> if parsing succeeds; otherwise, <c>null</c>.</returns>
            public DateTime? FromFormattedDate() =>
                DateTime.TryParseExact(date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)
                    ? result
                    : null;

            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-ddTHH:mm:ss" into a <see cref="DateTime"/>.
            /// Returns <c>null</c> if parsing fails.
            /// </summary>
            /// <returns>A <see cref="DateTime"/> if parsing succeeds; otherwise, <c>null</c>.</returns>
            public DateTime? FromFormattedDateTime() =>
                DateTime.TryParseExact(date, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime result)
                    ? result
                    : null;

            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-dd" into a <see cref="DateTime"/>.
            /// </summary>
            /// <param name="result">When this method returns, contains the parsed <see cref="DateTime"/> if parsing succeeded.</param>
            /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
            public bool TryFromFormattedDate(out DateTime result) =>
                DateTime.TryParseExact(date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-ddTHH:mm:ss" into a <see cref="DateTime"/>.
            /// </summary>
            /// <param name="result">When this method returns, contains the parsed <see cref="DateTime"/> if parsing succeeded.</param>
            /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
            public bool TryFromFormattedDateTime(out DateTime result) =>
                DateTime.TryParseExact(date, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
