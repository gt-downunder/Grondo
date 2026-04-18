using System.Globalization;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DateTimeOffset"/>, mirroring the methods in <see cref="DateTimeEx"/>.
    /// </summary>
    public static class DateTimeOffsetEx
    {
        extension(DateTimeOffset date)
        {
            /// <summary>
            /// Formats the specified <see cref="DateTimeOffset"/> as a string using the standard date format ("yyyy-MM-dd").
            /// </summary>
            /// <returns>A string representation of the date.</returns>
            public string ToFormattedDate() =>
                date.ToString(DateTimeEx.DateFormat, CultureInfo.InvariantCulture);

            /// <summary>
            /// Formats the specified <see cref="DateTimeOffset"/> as a string using the standard date-time format ("yyyy-MM-ddTHH:mm:ss").
            /// </summary>
            /// <returns>A string representation of the date-time.</returns>
            public string ToFormattedDateTime() =>
                date.ToString(DateTimeEx.DateTimeFormat, CultureInfo.InvariantCulture);

            /// <summary>
            /// Adds the specified number of weeks to the given <see cref="DateTimeOffset"/>.
            /// </summary>
            /// <param name="weeks">The number of weeks to add.</param>
            /// <returns>A new <see cref="DateTimeOffset"/> offset by the specified number of weeks.</returns>
            public DateTimeOffset AddWeeks(int weeks) =>
                date.AddDays((double)weeks * 7);

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> with milliseconds truncated.
            /// Useful for scenarios where precision beyond seconds is not required.
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> with milliseconds set to zero.</returns>
            public DateTimeOffset TruncateMilliseconds() =>
                new(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Offset);

            /// <summary>
            /// Determines whether the specified <see cref="DateTimeOffset"/> falls on a weekday (Monday through Friday).
            /// </summary>
            /// <returns><c>true</c> if the date is a weekday; otherwise, <c>false</c>.</returns>
            public bool IsWeekday() =>
                date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;

            /// <summary>
            /// Determines whether the specified <see cref="DateTimeOffset"/> falls on a weekend (Saturday or Sunday).
            /// </summary>
            /// <returns><c>true</c> if the date is a weekend; otherwise, <c>false</c>.</returns>
            public bool IsWeekend() =>
                date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> representing the start of the day (00:00:00.000).
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> at midnight of the same day.</returns>
            public DateTimeOffset StartOfDay() =>
                new(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> representing the end of the day (23:59:59.9999999).
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> at the last tick of the same day.</returns>
            public DateTimeOffset EndOfDay() =>
                new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset).AddDays(1).AddTicks(-1);

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> representing the first day of the month.
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> at the start of the first day of the month.</returns>
            public DateTimeOffset StartOfMonth() =>
                new(date.Year, date.Month, 1, 0, 0, 0, date.Offset);

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> representing the last day of the month.
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> at the end of the last day of the month.</returns>
            public DateTimeOffset EndOfMonth() =>
                new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, date.Offset).AddMonths(1).AddTicks(-1);

            /// <summary>
            /// Determines whether the specified <see cref="DateTimeOffset"/> falls between two dates, inclusive.
            /// </summary>
            /// <param name="start">The start of the range.</param>
            /// <param name="end">The end of the range.</param>
            /// <returns><c>true</c> if the date is between <paramref name="start"/> and <paramref name="end"/>; otherwise, <c>false</c>.</returns>
            public bool IsBetween(DateTimeOffset start, DateTimeOffset end) =>
                date >= start && date <= end;

            /// <summary>
            /// Converts the specified <see cref="DateTimeOffset"/> to a human-readable relative time string
            /// such as "3 hours ago" or "in 2 days".
            /// </summary>
            /// <returns>A relative time string.</returns>
            public string ToRelativeTime() =>
                TimeSpanEx.FormatRelativeTime(DateTimeOffset.UtcNow - date.ToUniversalTime());

            /// <summary>
            /// Returns the start of the week containing the specified date, preserving its offset.
            /// </summary>
            /// <param name="firstDayOfWeek">The day considered the first day of the week. Defaults to <see cref="DayOfWeek.Monday"/>.</param>
            /// <returns>A new <see cref="DateTimeOffset"/> at the start of the week.</returns>
            public DateTimeOffset StartOfWeek(DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
            {
                int diff = (7 + (date.DayOfWeek - firstDayOfWeek)) % 7;
                return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset).AddDays(-diff);
            }

            /// <summary>
            /// Returns the end of the week containing the specified date (last tick of the last day).
            /// </summary>
            /// <param name="firstDayOfWeek">The day considered the first day of the week. Defaults to <see cref="DayOfWeek.Monday"/>.</param>
            /// <returns>A new <see cref="DateTimeOffset"/> at the last tick of the week.</returns>
            public DateTimeOffset EndOfWeek(DayOfWeek firstDayOfWeek = DayOfWeek.Monday) =>
                date.StartOfWeek(firstDayOfWeek).AddDays(7).AddTicks(-1);

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> representing the first day of the year.
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> at the start of the first day of the year.</returns>
            public DateTimeOffset StartOfYear() =>
                new(date.Year, 1, 1, 0, 0, 0, date.Offset);

            /// <summary>
            /// Returns a new <see cref="DateTimeOffset"/> representing the end of the last day of the year.
            /// </summary>
            /// <returns>A new <see cref="DateTimeOffset"/> at the last tick of the year.</returns>
            public DateTimeOffset EndOfYear() =>
                new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, date.Offset).AddYears(1).AddTicks(-1);

            /// <summary>
            /// Calculates the age in whole years from the specified date to the current UTC time.
            /// </summary>
            /// <returns>The age in whole years.</returns>
            public int Age() =>
                date.AgeAt(DateTimeOffset.UtcNow);

            /// <summary>
            /// Calculates the age in whole years from the specified date to a reference date.
            /// </summary>
            /// <param name="asOf">The reference date.</param>
            /// <returns>The age in whole years.</returns>
            public int AgeAt(DateTimeOffset asOf)
            {
                int years = asOf.Year - date.Year;
                if (asOf.Month < date.Month || (asOf.Month == date.Month && asOf.Day < date.Day))
                    years--;
                return years;
            }

            /// <summary>
            /// Determines whether the specified date falls on the current UTC day.
            /// </summary>
            /// <returns><c>true</c> if the date is today (UTC); otherwise, <c>false</c>.</returns>
            public bool IsToday() =>
                date.UtcDateTime.Date == DateTime.UtcNow.Date;

            /// <summary>
            /// Determines whether the specified date is strictly before the current UTC time.
            /// </summary>
            /// <returns><c>true</c> if the date is in the past; otherwise, <c>false</c>.</returns>
            public bool IsInPast() =>
                date < DateTimeOffset.UtcNow;

            /// <summary>
            /// Determines whether the specified date is strictly after the current UTC time.
            /// </summary>
            /// <returns><c>true</c> if the date is in the future; otherwise, <c>false</c>.</returns>
            public bool IsInFuture() =>
                date > DateTimeOffset.UtcNow;

            /// <summary>
            /// Returns the number of whole days from the current UTC date to the specified date.
            /// </summary>
            /// <returns>The number of days until the date (negative if in the past).</returns>
            public int DaysUntil() =>
                (int)(date.UtcDateTime.Date - DateTime.UtcNow.Date).TotalDays;

            /// <summary>
            /// Determines whether the specified date falls in a leap year.
            /// </summary>
            /// <returns><c>true</c> if the date's year is a leap year; otherwise, <c>false</c>.</returns>
            public bool IsLeapYear() =>
                DateTime.IsLeapYear(date.Year);
        }

        extension(DateTimeOffset? date)
        {
            /// <summary>
            /// Formats the specified nullable <see cref="DateTimeOffset"/> as a string using the standard date format ("yyyy-MM-dd").
            /// Returns an empty string if the value is <c>null</c>.
            /// </summary>
            /// <returns>A string representation of the date, or an empty string if the value is <c>null</c>.</returns>
            public string ToFormattedDate() =>
                date.HasValue ? date.Value.ToFormattedDate() : string.Empty;

            /// <summary>
            /// Formats the specified nullable <see cref="DateTimeOffset"/> as a string using the standard date-time format ("yyyy-MM-ddTHH:mm:ss").
            /// Returns an empty string if the value is <c>null</c>.
            /// </summary>
            /// <returns>A string representation of the date-time, or an empty string if the value is <c>null</c>.</returns>
            public string ToFormattedDateTime() =>
                date.HasValue ? date.Value.ToFormattedDateTime() : string.Empty;
        }

        extension(string date)
        {
            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-dd" into a <see cref="DateTimeOffset"/>.
            /// Returns <c>null</c> if parsing fails.
            /// </summary>
            /// <returns>A <see cref="DateTimeOffset"/> if parsing succeeds; otherwise, <c>null</c>.</returns>
            public DateTimeOffset? FromFormattedDateOffset() =>
                DateTimeOffset.TryParseExact(date, DateTimeEx.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result)
                    ? result
                    : null;

            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-ddTHH:mm:ss" into a <see cref="DateTimeOffset"/>.
            /// Returns <c>null</c> if parsing fails.
            /// </summary>
            /// <returns>A <see cref="DateTimeOffset"/> if parsing succeeds; otherwise, <c>null</c>.</returns>
            public DateTimeOffset? FromFormattedDateTimeOffset() =>
                DateTimeOffset.TryParseExact(date, DateTimeEx.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result)
                    ? result
                    : null;

            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-dd" into a <see cref="DateTimeOffset"/>.
            /// </summary>
            /// <param name="result">When this method returns, contains the parsed <see cref="DateTimeOffset"/> if parsing succeeded.</param>
            /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
            public bool TryFromFormattedDateOffset(out DateTimeOffset result) =>
                DateTimeOffset.TryParseExact(date, DateTimeEx.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

            /// <summary>
            /// Attempts to parse a string formatted as "yyyy-MM-ddTHH:mm:ss" into a <see cref="DateTimeOffset"/>.
            /// </summary>
            /// <param name="result">When this method returns, contains the parsed <see cref="DateTimeOffset"/> if parsing succeeded.</param>
            /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
            public bool TryFromFormattedDateTimeOffset(out DateTimeOffset result) =>
                DateTimeOffset.TryParseExact(date, DateTimeEx.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
