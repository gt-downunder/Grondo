using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class DateTimeOffsetExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ToFormattedDate_ReturnsYyyyMmDd()
        {
            var date = new DateTimeOffset(2024, 3, 15, 10, 30, 0, TimeSpan.Zero);
            date.ToFormattedDate().Should().Be("2024-03-15");
        }

        [TestMethod]
        public void ToFormattedDate_Nullable_NullReturnsEmpty()
        {
            DateTimeOffset? date = null;
            date.ToFormattedDate().Should().BeEmpty();
        }

        [TestMethod]
        public void ToFormattedDate_Nullable_HasValue_ReturnsFormatted()
        {
            DateTimeOffset? date = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
            date.ToFormattedDate().Should().Be("2024-01-01");
        }

        [TestMethod]
        public void ToFormattedDateTime_ReturnsYyyyMmDdTHhMmSs()
        {
            var date = new DateTimeOffset(2024, 3, 15, 14, 30, 45, TimeSpan.Zero);
            date.ToFormattedDateTime().Should().Be("2024-03-15T14:30:45");
        }

        [TestMethod]
        public void ToFormattedDateTime_Nullable_NullReturnsEmpty()
        {
            DateTimeOffset? date = null;
            date.ToFormattedDateTime().Should().BeEmpty();
        }

        [TestMethod]
        public void FromFormattedDateOffset_ValidDate_ReturnsDateTimeOffset()
        {
            DateTimeOffset? result = "2024-03-15".FromFormattedDateOffset();
            result.Should().NotBeNull();
            result!.Value.Year.Should().Be(2024);
            result.Value.Month.Should().Be(3);
            result.Value.Day.Should().Be(15);
        }

        [TestMethod]
        public void FromFormattedDateOffset_InvalidDate_ReturnsNull() => "not-a-date".FromFormattedDateOffset().Should().BeNull();

        [TestMethod]
        public void FromFormattedDateTimeOffset_ValidDateTime_ReturnsDateTimeOffset()
        {
            DateTimeOffset? result = "2024-03-15T14:30:45".FromFormattedDateTimeOffset();
            result.Should().NotBeNull();
            result!.Value.Hour.Should().Be(14);
            result.Value.Minute.Should().Be(30);
        }

        [TestMethod]
        public void TryFromFormattedDateOffset_ValidDate_ReturnsTrue()
        {
            bool success = "2024-03-15".TryFromFormattedDateOffset(out DateTimeOffset result);
            success.Should().BeTrue();
            result.Year.Should().Be(2024);
        }

        [TestMethod]
        public void TryFromFormattedDateOffset_InvalidDate_ReturnsFalse() => "invalid".TryFromFormattedDateOffset(out _).Should().BeFalse();

        [TestMethod]
        public void TryFromFormattedDateTimeOffset_ValidDateTime_ReturnsTrue()
        {
            "2024-03-15T14:30:45".TryFromFormattedDateTimeOffset(out DateTimeOffset result).Should().BeTrue();
            result.Second.Should().Be(45);
        }

        [TestMethod]
        public void AddWeeks_AddsCorrectDays()
        {
            var date = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset result = date.AddWeeks(2);
            result.Day.Should().Be(15);
        }

        [TestMethod]
        public void TruncateMilliseconds_RemovesMilliseconds()
        {
            var date = new DateTimeOffset(2024, 1, 1, 12, 30, 45, 999, TimeSpan.Zero);
            DateTimeOffset truncated = date.TruncateMilliseconds();
            truncated.Millisecond.Should().Be(0);
            truncated.Second.Should().Be(45);
        }

        [TestMethod]
        [DataRow(2024, 1, 1, true)]   // Monday
        [DataRow(2024, 1, 5, true)]   // Friday
        [DataRow(2024, 1, 6, false)]  // Saturday
        [DataRow(2024, 1, 7, false)]  // Sunday
        public void IsWeekday_ReturnsExpectedResult(int y, int m, int d, bool expected)
        {
            var date = new DateTimeOffset(y, m, d, 0, 0, 0, TimeSpan.Zero);
            date.IsWeekday().Should().Be(expected);
        }

        [TestMethod]
        [DataRow(2024, 1, 6, true)]   // Saturday
        [DataRow(2024, 1, 7, true)]   // Sunday
        [DataRow(2024, 1, 1, false)]  // Monday
        public void IsWeekend_ReturnsExpectedResult(int y, int m, int d, bool expected)
        {
            var date = new DateTimeOffset(y, m, d, 0, 0, 0, TimeSpan.Zero);
            date.IsWeekend().Should().Be(expected);
        }

        // --- StartOfDay / EndOfDay ---

        [TestMethod]
        public void StartOfDay_ReturnsMidnight()
        {
            var date = new DateTimeOffset(2024, 6, 15, 14, 30, 45, TimeSpan.FromHours(5));
            DateTimeOffset result = date.StartOfDay();
            result.Hour.Should().Be(0);
            result.Minute.Should().Be(0);
            result.Day.Should().Be(15);
            result.Offset.Should().Be(TimeSpan.FromHours(5));
        }

        [TestMethod]
        public void EndOfDay_ReturnsEndOfDay()
        {
            var date = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset result = date.EndOfDay();
            result.Hour.Should().Be(23);
            result.Minute.Should().Be(59);
            result.Second.Should().Be(59);
        }

        // --- StartOfMonth / EndOfMonth ---

        [TestMethod]
        public void StartOfMonth_ReturnsFirstDay()
        {
            var date = new DateTimeOffset(2024, 3, 15, 10, 0, 0, TimeSpan.Zero);
            DateTimeOffset result = date.StartOfMonth();
            result.Day.Should().Be(1);
            result.Hour.Should().Be(0);
        }

        [TestMethod]
        public void EndOfMonth_ReturnsLastDay()
        {
            var date = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset result = date.EndOfMonth();
            result.Day.Should().Be(29); // 2024 is a leap year
            result.Hour.Should().Be(23);
        }

        // --- IsBetween ---

        [TestMethod]
        public void IsBetween_InRange_ReturnsTrue()
        {
            var date = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
            var start = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var end = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero);
            date.IsBetween(start, end).Should().BeTrue();
        }

        [TestMethod]
        public void IsBetween_OutOfRange_ReturnsFalse()
        {
            var date = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var start = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var end = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero);
            date.IsBetween(start, end).Should().BeFalse();
        }

        // --- ToRelativeTime ---

        [TestMethod]
        public void ToRelativeTime_RecentPast_ReturnsJustNow() => DateTimeOffset.UtcNow.AddSeconds(-5).ToRelativeTime().Should().Be("just now");

        [TestMethod]
        public void ToRelativeTime_MinutesAgo_ReturnsMinutes() => DateTimeOffset.UtcNow.AddMinutes(-5).ToRelativeTime().Should().Be("5 minutes ago");

        [TestMethod]
        public void ToRelativeTime_Future_ReturnsFutureFormat() => DateTimeOffset.UtcNow.AddHours(3).ToRelativeTime().Should().StartWith("in ");
    }
}
