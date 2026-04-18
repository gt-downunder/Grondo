using System.Text.RegularExpressions;
using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class RegexExTests : BaseExtensionTest
    {
        [TestMethod]
        public void RegexIsMatch_Matches_ReturnsTrue() =>
            "hello123".RegexIsMatch(@"[a-z]+\d+").Should().BeTrue();

        [TestMethod]
        public void RegexIsMatch_NoMatch_ReturnsFalse() =>
            "HELLO".RegexIsMatch(@"\d+").Should().BeFalse();

        [TestMethod]
        public void RegexIsMatch_IgnoreCase_Matches() =>
            "HELLO".RegexIsMatch(@"hello", RegexOptions.IgnoreCase).Should().BeTrue();

        [TestMethod]
        public void RegexMatch_ReturnsFirstMatch() =>
            "abc123def456".RegexMatch(@"\d+").Should().Be("123");

        [TestMethod]
        public void RegexMatch_NoMatch_ReturnsNull() =>
            "abc".RegexMatch(@"\d+").Should().BeNull();

        [TestMethod]
        public void RegexMatches_ReturnsAllMatches() =>
            "abc123def456".RegexMatches(@"\d+").Should().BeEquivalentTo(["123", "456"]);

        [TestMethod]
        public void RegexMatches_NoMatch_ReturnsEmpty() =>
            "abcdef".RegexMatches(@"\d+").Should().BeEmpty();

        [TestMethod]
        public void RegexReplace_String_ReplacesAll() =>
            "abc123def456".RegexReplace(@"\d+", "X").Should().Be("abcXdefX");

        [TestMethod]
        public void RegexReplace_Evaluator_AppliesFunctionToEachMatch()
        {
            string result = "a1 b2 c3".RegexReplace(@"\d", m => (int.Parse(m.Value, System.Globalization.CultureInfo.InvariantCulture) * 10).ToString(System.Globalization.CultureInfo.InvariantCulture));
            result.Should().Be("a10 b20 c30");
        }
    }
}
