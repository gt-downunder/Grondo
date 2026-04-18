using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class DateTimeExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ToFormattedDate_NullDateTime_ReturnsEmptyString()
        {
            string result = default(DateTime?).ToFormattedDate();

            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void ToFormattedDate_ValidDateTime_Success()
        {
            DateTime? testDate = DateTime.Now;

            string result = testDate.ToFormattedDate();

            result.Should().Be(testDate.Value.ToString("yyyy-MM-dd"));
        }

        [TestMethod]
        public void ToFormattedDateTime_NullDateTime_ReturnsEmptyString()
        {
            string result = default(DateTime?).ToFormattedDateTime();

            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void ToFormattedDateTime_ValidDateTime_Success()
        {
            DateTime? testDate = DateTime.Now;

            string result = testDate.ToFormattedDateTime();

            result.Should().Be(testDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2023-13-32")]
        [DataRow("2023-03-23T11:51:28")]
        [DataRow("yyyy-MM-dd")]
        public void FromFormattedDate_InvalidFormat_ReturnsNull(string testDate)
        {
            DateTime? result = testDate.FromFormattedDate();

            result.Should().BeNull();
        }

        [TestMethod]
        public void FromFormattedDate_ValidFormat_Success()
        {
            const string TestDate = "2023-03-23";

            DateTime? result = TestDate.FromFormattedDate();

            result.Should().Be(new DateTime(2023, 3, 23));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2023-03-23")]
        [DataRow("2023-13-31T11:51:28")]
        [DataRow("yyyy-MM-ddTHH:mm:ss")]
        public void FromFormattedDateTime_InvalidFormat_ReturnsNull(string testDate)
        {
            DateTime? result = testDate.FromFormattedDateTime();

            result.Should().BeNull();
        }

        [TestMethod]
        public void FromFormattedDateTime_ValidFormat_Success()
        {
            const string TestDate = "2023-03-23T11:57:28";

            DateTime? result = TestDate.FromFormattedDateTime();

            result.Should().Be(new DateTime(2023, 3, 23, 11, 57, 28));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void AddWeeks_Succeeds(int weeksToAdd)
        {
            DateTime now = DateTime.Now;
            DateTime result = now.AddWeeks(weeksToAdd);

            result.DayOfWeek.Should().Be(now.DayOfWeek);
            result.Should().BeExactly(new TimeSpan(weeksToAdd * 7, 0, 0, 0, 0)).After(now);
        }

        [TestMethod]
        public void TruncateMilliseconds_Succeeds()
        {
            var dt = new DateTime(1999, 12, 31, 0, 0, 0, 999);
            dt.Millisecond.Should().Be(999);

            DateTime result = dt.TruncateMilliseconds();
            result.Millisecond.Should().Be(0);
        }

        [TestMethod]
        [DataRow("2019-09-30")] // Monday
        [DataRow("2019-10-01")] // Tuesday
        [DataRow("2019-10-02")] // Wednesday
        [DataRow("2019-10-03")] // Thursday
        [DataRow("2019-10-04")] // Friday
        public void IsWeekday_MondayToFriday_ReturnsTrue(string dateTime)
        {
            var dt = Convert.ToDateTime(dateTime);
            dt.IsWeekday().Should().BeTrue();
        }

        [TestMethod]
        [DataRow("2019-10-05")] // Saturday
        [DataRow("2019-10-06")] // Sunday
        public void IsWeekday_SaturdayAndSunday_ReturnsFalse(string dateTime)
        {
            var dt = Convert.ToDateTime(dateTime);
            dt.IsWeekday().Should().BeFalse();
        }

        [TestMethod]
        [DataRow("2019-10-05")] // Saturday
        [DataRow("2019-10-06")] // Sunday
        public void IsWeekend_SaturdayAndSunday_ReturnsTrue(string dateTime)
        {
            var dt = Convert.ToDateTime(dateTime);
            dt.IsWeekend().Should().BeTrue();
        }

        [TestMethod]
        [DataRow("2019-09-30")] // Monday
        [DataRow("2019-10-01")] // Tuesday
        [DataRow("2019-10-02")] // Wednesday
        [DataRow("2019-10-03")] // Thursday
        [DataRow("2019-10-04")] // Friday
        public void IsWeekend_MondayToFriday_ReturnsFalse(string dateTime)
        {
            var dt = Convert.ToDateTime(dateTime);
            dt.IsWeekend().Should().BeFalse();
        }

        // --- StartOfDay / EndOfDay ---

        [TestMethod]
        public void StartOfDay_ReturnsMidnight()
        {
            var dt = new DateTime(2024, 6, 15, 14, 30, 45, 999);
            DateTime result = dt.StartOfDay();
            result.Should().Be(new DateTime(2024, 6, 15, 0, 0, 0, 0));
        }

        [TestMethod]
        public void EndOfDay_ReturnsEndOfDay()
        {
            var dt = new DateTime(2024, 6, 15, 0, 0, 0);
            DateTime result = dt.EndOfDay();
            result.Should().Be(new DateTime(2024, 6, 15).AddDays(1).AddTicks(-1));
        }

        // --- StartOfMonth / EndOfMonth ---

        [TestMethod]
        public void StartOfMonth_ReturnsFirstDay()
        {
            var dt = new DateTime(2024, 3, 15, 10, 0, 0);
            DateTime result = dt.StartOfMonth();
            result.Should().Be(new DateTime(2024, 3, 1, 0, 0, 0));
        }

        [TestMethod]
        public void EndOfMonth_ReturnsLastDay()
        {
            var dt = new DateTime(2024, 2, 1);
            DateTime result = dt.EndOfMonth();
            result.Day.Should().Be(29); // 2024 is a leap year
            result.Hour.Should().Be(23);
            result.Minute.Should().Be(59);
        }

        [TestMethod]
        public void EndOfMonth_NonLeapYear_February28()
        {
            var dt = new DateTime(2023, 2, 10);
            dt.EndOfMonth().Day.Should().Be(28);
        }

        // --- IsBetween ---

        [TestMethod]
        public void IsBetween_InRange_ReturnsTrue()
        {
            var dt = new DateTime(2024, 6, 15);
            var start = new DateTime(2024, 1, 1);
            var end = new DateTime(2024, 12, 31);
            dt.IsBetween(start, end).Should().BeTrue();
        }

        [TestMethod]
        public void IsBetween_OnBoundary_ReturnsTrue()
        {
            var dt = new DateTime(2024, 1, 1);
            dt.IsBetween(dt, new DateTime(2024, 12, 31)).Should().BeTrue();
        }

        [TestMethod]
        public void IsBetween_OutOfRange_ReturnsFalse()
        {
            var dt = new DateTime(2025, 1, 1);
            var start = new DateTime(2024, 1, 1);
            var end = new DateTime(2024, 12, 31);
            dt.IsBetween(start, end).Should().BeFalse();
        }

        // --- ToRelativeTime ---

        [TestMethod]
        public void ToRelativeTime_RecentPast_ReturnsJustNow() => DateTime.UtcNow.AddSeconds(-5).ToRelativeTime().Should().Be("just now");

        [TestMethod]
        public void ToRelativeTime_MinutesAgo_ReturnsMinutes() => DateTime.UtcNow.AddMinutes(-5).ToRelativeTime().Should().Be("5 minutes ago");

        [TestMethod]
        public void ToRelativeTime_HoursAgo_ReturnsHours() => DateTime.UtcNow.AddHours(-3).ToRelativeTime().Should().Be("3 hours ago");

        [TestMethod]
        public void ToRelativeTime_Future_ReturnsFutureFormat() => DateTime.UtcNow.AddHours(3).ToRelativeTime().Should().StartWith("in ");
    }
}
