using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class NumericExTests : BaseExtensionTest
    {
        [TestMethod]
        public void IsZero_Zero_ReturnsTrue() => 0.IsZero().Should().BeTrue();

        [TestMethod]
        public void IsZero_Positive_ReturnsFalse() => 5.IsZero().Should().BeFalse();

        [TestMethod]
        public void IsZero_Negative_ReturnsFalse() => (-3).IsZero().Should().BeFalse();

        [TestMethod]
        public void IsZero_DoubleZero_ReturnsTrue() => 0.0.IsZero().Should().BeTrue();

        [TestMethod]
        public void IsPositive_PositiveInt_ReturnsTrue() => 42.IsPositive().Should().BeTrue();

        [TestMethod]
        public void IsPositive_Zero_ReturnsFalse() => 0.IsPositive().Should().BeFalse();

        [TestMethod]
        public void IsPositive_Negative_ReturnsFalse() => (-1).IsPositive().Should().BeFalse();

        [TestMethod]
        public void IsPositive_PositiveDouble_ReturnsTrue() => 3.14.IsPositive().Should().BeTrue();

        [TestMethod]
        public void IsNegative_NegativeInt_ReturnsTrue() => (-7).IsNegative().Should().BeTrue();

        [TestMethod]
        public void IsNegative_Zero_ReturnsFalse() => 0.IsNegative().Should().BeFalse();

        [TestMethod]
        public void IsNegative_Positive_ReturnsFalse() => 1.IsNegative().Should().BeFalse();

        [TestMethod]
        public void IsNegative_NegativeDecimal_ReturnsTrue() => (-0.5m).IsNegative().Should().BeTrue();

        // --- ToHumanByteSize ---

        [TestMethod]
        public void ToHumanByteSize_Zero_ReturnsZeroB() => 0L.ToHumanByteSize().Should().Be("0 B");

        [TestMethod]
        public void ToHumanByteSize_Bytes_ReturnsBytes() => 500L.ToHumanByteSize().Should().Be("500 B");

        [TestMethod]
        public void ToHumanByteSize_Kilobytes_ReturnsKB() => 1024L.ToHumanByteSize().Should().Be("1 KB");

        [TestMethod]
        public void ToHumanByteSize_Megabytes_ReturnsMB() => (1536L * 1024).ToHumanByteSize().Should().Be("1.5 MB");

        [TestMethod]
        public void ToHumanByteSize_Gigabytes_ReturnsGB() => (1L * 1024 * 1024 * 1024).ToHumanByteSize().Should().Be("1 GB");

        [TestMethod]
        public void ToHumanByteSize_SI_Uses1000Base() => 1000L.ToHumanByteSize(useSI: true).Should().Be("1 kB");

        [TestMethod]
        public void ToHumanByteSize_Negative_ReturnsNegative() => (-1024L).ToHumanByteSize().Should().Be("-1 KB");

        // --- ToOrdinal ---

        [TestMethod]
        [DataRow(1, "1st")]
        [DataRow(2, "2nd")]
        [DataRow(3, "3rd")]
        [DataRow(4, "4th")]
        [DataRow(11, "11th")]
        [DataRow(12, "12th")]
        [DataRow(13, "13th")]
        [DataRow(21, "21st")]
        [DataRow(22, "22nd")]
        [DataRow(23, "23rd")]
        [DataRow(101, "101st")]
        [DataRow(111, "111th")]
        [DataRow(0, "0th")]
        [DataRow(-1, "-1st")]
        public void ToOrdinal_ReturnsCorrectSuffix(int input, string expected) =>
            input.ToOrdinal().Should().Be(expected);

        // --- Fluent TimeSpan ---

        [TestMethod]
        public void Days_Int_ReturnsCorrectTimeSpan() => 3.Days().Should().Be(TimeSpan.FromDays(3));

        [TestMethod]
        public void Hours_Int_ReturnsCorrectTimeSpan() => 2.Hours().Should().Be(TimeSpan.FromHours(2));

        [TestMethod]
        public void Minutes_Int_ReturnsCorrectTimeSpan() => 30.Minutes().Should().Be(TimeSpan.FromMinutes(30));

        [TestMethod]
        public void Seconds_Int_ReturnsCorrectTimeSpan() => 45.Seconds().Should().Be(TimeSpan.FromSeconds(45));

        [TestMethod]
        public void Milliseconds_Int_ReturnsCorrectTimeSpan() => 500.Milliseconds().Should().Be(TimeSpan.FromMilliseconds(500));

        [TestMethod]
        public void Days_Double_ReturnsCorrectTimeSpan() => 1.5.Days().Should().Be(TimeSpan.FromDays(1.5));

        [TestMethod]
        public void Hours_Double_ReturnsCorrectTimeSpan() => 2.5.Hours().Should().Be(TimeSpan.FromHours(2.5));

        [TestMethod]
        public void Minutes_Double_ReturnsCorrectTimeSpan() => 30.5.Minutes().Should().Be(TimeSpan.FromMinutes(30.5));

        [TestMethod]
        public void Seconds_Double_ReturnsCorrectTimeSpan() => 1.5.Seconds().Should().Be(TimeSpan.FromSeconds(1.5));

        [TestMethod]
        public void Milliseconds_Double_ReturnsCorrectTimeSpan() => 100.5.Milliseconds().Should().Be(TimeSpan.FromMilliseconds(100.5));
    }
}

