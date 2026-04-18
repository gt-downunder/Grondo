using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class TimeSpanExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ToHumanReadable_Zero_ReturnsZeroSeconds() => TimeSpan.Zero.ToHumanReadable().Should().Be("0 seconds");

        [TestMethod]
        public void ToHumanReadable_Seconds_ReturnsSeconds() => TimeSpan.FromSeconds(45).ToHumanReadable().Should().Be("45 seconds");

        [TestMethod]
        public void ToHumanReadable_OnSecond_ReturnsSingular() => TimeSpan.FromSeconds(1).ToHumanReadable().Should().Be("1 second");

        [TestMethod]
        public void ToHumanReadable_Minutes_ReturnsMinutes() => TimeSpan.FromMinutes(5).ToHumanReadable().Should().Be("5 minutes");

        [TestMethod]
        public void ToHumanReadable_Hours_ReturnsHours() => TimeSpan.FromHours(2).ToHumanReadable().Should().Be("2 hours");

        [TestMethod]
        public void ToHumanReadable_Days_ReturnsDays() => TimeSpan.FromDays(3).ToHumanReadable().Should().Be("3 days");

        [TestMethod]
        public void ToHumanReadable_Complex_ReturnsMultipleParts()
        {
            var ts = new TimeSpan(2, 5, 30, 0);
            string result = ts.ToHumanReadable();
            result.Should().Contain("2 days");
            result.Should().Contain("5 hours");
            result.Should().Contain("30 minutes");
        }

        [TestMethod]
        public void ToHumanReadable_Negative_PrependsMinus() => TimeSpan.FromHours(-3).ToHumanReadable().Should().StartWith("-");

        [TestMethod]
        public void ToHumanReadable_Years_ReturnsYears() => TimeSpan.FromDays(400).ToHumanReadable().Should().Contain("1 year");

        [TestMethod]
        public void ToHumanReadable_MinutesAndSeconds_IncludesSeconds()
        {
            var ts = new TimeSpan(0, 0, 1, 30);
            string result = ts.ToHumanReadable();
            result.Should().Contain("1 minute");
            result.Should().Contain("30 seconds");
        }

        [TestMethod]
        public void ToHumanReadable_HoursMinutesSeconds_IncludesAll()
        {
            var ts = new TimeSpan(0, 2, 15, 45);
            string result = ts.ToHumanReadable();
            result.Should().Contain("2 hours");
            result.Should().Contain("15 minutes");
            result.Should().Contain("45 seconds");
        }

        [TestMethod]
        public void ToHumanReadable_MaxParts_LimitsParts()
        {
            var ts = new TimeSpan(2, 5, 30, 45);
            string result = ts.ToHumanReadable(maxParts: 2);
            result.Should().Contain("2 days");
            result.Should().Contain("5 hours");
            result.Should().NotContain("minutes");
            result.Should().NotContain("seconds");
        }

        [TestMethod]
        public void ToHumanReadable_MaxPartsOne_ReturnsSinglePart()
        {
            var ts = new TimeSpan(2, 5, 30, 45);
            string result = ts.ToHumanReadable(maxParts: 1);
            result.Should().Be("2 days");
        }

        [TestMethod]
        public void ToClockString_FormatsHoursMinutesSeconds() =>
            new TimeSpan(1, 5, 30).ToClockString().Should().Be("01:05:30");

        [TestMethod]
        public void ToClockString_OverTwentyFourHours_KeepsHours() =>
            new TimeSpan(1, 3, 10, 0).ToClockString().Should().Be("27:10:00");

        [TestMethod]
        public void ToClockString_Negative_HasMinusPrefix() =>
            TimeSpan.FromSeconds(-65).ToClockString().Should().Be("-00:01:05");

        [TestMethod]
        public void IsPositive_PositiveTimeSpan_ReturnsTrue() =>
            TimeSpan.FromSeconds(1).IsPositive().Should().BeTrue();

        [TestMethod]
        public void IsNegative_NegativeTimeSpan_ReturnsTrue() =>
            TimeSpan.FromSeconds(-1).IsNegative().Should().BeTrue();

        [TestMethod]
        public void IsPositive_Zero_ReturnsFalse() =>
            TimeSpan.Zero.IsPositive().Should().BeFalse();
    }
}

