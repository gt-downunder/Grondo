using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class SpanExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ContainsIgnoreCase_MatchingCase_ReturnsTrue() => "Hello World".AsSpan().ContainsIgnoreCase("hello").Should().BeTrue();

        [TestMethod]
        public void ContainsIgnoreCase_DifferentCase_ReturnsTrue() => "Hello World".AsSpan().ContainsIgnoreCase("WORLD").Should().BeTrue();

        [TestMethod]
        public void ContainsIgnoreCase_NotFound_ReturnsFalse() => "Hello World".AsSpan().ContainsIgnoreCase("xyz").Should().BeFalse();

        [TestMethod]
        public void IsNumeric_ValidNumber_ReturnsTrue() => "123.45".AsSpan().IsNumeric().Should().BeTrue();

        [TestMethod]
        public void IsNumeric_NegativeNumber_ReturnsTrue() => "-42".AsSpan().IsNumeric().Should().BeTrue();

        [TestMethod]
        public void IsNumeric_NonNumeric_ReturnsFalse() => "abc".AsSpan().IsNumeric().Should().BeFalse();

        [TestMethod]
        public void IsNumeric_Empty_ReturnsFalse() => "".AsSpan().IsNumeric().Should().BeFalse();

        [TestMethod]
        public void SafeTrim_WithWhitespace_ReturnsTrimmed() => "  hello  ".AsSpan().SafeTrim().ToString().Should().Be("hello");

        [TestMethod]
        public void SafeTrim_EmptySpan_ReturnsEmpty() => "".AsSpan().SafeTrim().IsEmpty.Should().BeTrue();

        [TestMethod]
        public void SafeTrim_NoWhitespace_ReturnsSame() => "test".AsSpan().SafeTrim().ToString().Should().Be("test");
    }
}

