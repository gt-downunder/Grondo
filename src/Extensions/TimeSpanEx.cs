namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanEx
    {
        extension(TimeSpan timeSpan)
        {
            /// <summary>
            /// Converts the <see cref="TimeSpan"/> to a human-readable string such as "2 hours 5 minutes"
            /// or "3 days 1 hour".
            /// </summary>
            /// <param name="maxParts">The maximum number of time parts to include. If <c>null</c>, all parts are included.</param>
            /// <returns>A human-readable duration string. Returns "0 seconds" for <see cref="TimeSpan.Zero"/>.</returns>
            public string ToHumanReadable(int? maxParts = null)
            {
                if (timeSpan == TimeSpan.Zero)
                    return "0 seconds";

                TimeSpan abs = timeSpan < TimeSpan.Zero ? timeSpan.Negate() : timeSpan;
                var parts = new List<string>();

                if (abs.Days >= 365)
                {
                    int years = abs.Days / 365;
                    parts.Add(Pluralize(years, "year"));
                    abs -= TimeSpan.FromDays(years * 365);
                }

                if (abs.Days >= 30)
                {
                    int months = abs.Days / 30;
                    parts.Add(Pluralize(months, "month"));
                    abs -= TimeSpan.FromDays(months * 30);
                }

                if (abs.Days > 0)
                    parts.Add(Pluralize(abs.Days, "day"));

                if (abs.Hours > 0)
                    parts.Add(Pluralize(abs.Hours, "hour"));

                if (abs.Minutes > 0)
                    parts.Add(Pluralize(abs.Minutes, "minute"));

                if (abs.Seconds > 0)
                    parts.Add(Pluralize(abs.Seconds, "second"));

                if (maxParts.HasValue)
                    parts = parts.GetRange(0, Math.Min(maxParts.Value, parts.Count));

                string result = string.Join(" ", parts);
                return timeSpan < TimeSpan.Zero ? $"-{result}" : result;
            }

            /// <summary>
            /// Formats the <see cref="TimeSpan"/> as a clock string (HH:MM:SS), supporting durations longer than 24 hours.
            /// </summary>
            /// <returns>A string such as "01:05:30" or "27:10:00" for durations over a day.</returns>
            public string ToClockString()
            {
                TimeSpan abs = timeSpan < TimeSpan.Zero ? timeSpan.Negate() : timeSpan;
                string result = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0:D2}:{1:D2}:{2:D2}",
                    (int)abs.TotalHours,
                    abs.Minutes,
                    abs.Seconds);

                return timeSpan < TimeSpan.Zero ? "-" + result : result;
            }

            /// <summary>Determines whether the <see cref="TimeSpan"/> is greater than zero.</summary>
            /// <returns><c>true</c> if positive; otherwise, <c>false</c>.</returns>
            public bool IsPositive() => timeSpan > TimeSpan.Zero;

            /// <summary>Determines whether the <see cref="TimeSpan"/> is less than zero.</summary>
            /// <returns><c>true</c> if negative; otherwise, <c>false</c>.</returns>
            public bool IsNegative() => timeSpan < TimeSpan.Zero;
        }

        internal static string Pluralize(int count, string unit) =>
            string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0} {1}{2}",
                count,
                unit,
                count == 1 ? "" : "s");

        internal static string FormatRelativeTime(TimeSpan diff)
        {
            bool isFuture = diff.TotalSeconds < 0;
            TimeSpan absDiff = isFuture ? diff.Negate() : diff;

            string relative = absDiff.TotalSeconds switch
            {
                < 60 => "just now",
                < 3600 => Pluralize((int)absDiff.TotalMinutes, "minute"),
                < 86400 => Pluralize((int)absDiff.TotalHours, "hour"),
                < 2592000 => Pluralize((int)absDiff.TotalDays, "day"),
                < 31536000 => Pluralize((int)(absDiff.TotalDays / 30), "month"),
                _ => Pluralize((int)(absDiff.TotalDays / 365), "year")
            };

            if (relative == "just now") return relative;
            return isFuture
                ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "in {0}", relative)
                : string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} ago", relative);
        }
    }
}

