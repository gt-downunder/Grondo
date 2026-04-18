using FluentAssertions;
using Grondo.Utilities;

namespace Grondo.Tests.Utilities
{
    [TestClass]
    public class StringFactoryTests : BaseTest
    {
        [TestMethod]
        public void CreateAlphabeticString_ReturnsRequestedLength()
        {
            string result = StringFactory.CreateAlphabeticString(32);
            result.Should().HaveLength(32);
            result.Should().MatchRegex(@"^[A-Za-z]+$");
        }

        [TestMethod]
        public void CreateAlphabeticString_ZeroLength_Throws() =>
            FluentActions.Invoking(() => StringFactory.CreateAlphabeticString(0))
                .Should().Throw<ArgumentOutOfRangeException>();

        [TestMethod]
        public void CreateNumericString_ReturnsDigitsOnly()
        {
            string result = StringFactory.CreateNumericString(16);
            result.Should().HaveLength(16);
            result.Should().MatchRegex(@"^[0-9]+$");
        }

        [TestMethod]
        public void CreateHexString_ReturnsHexOnly()
        {
            string result = StringFactory.CreateHexString(24);
            result.Should().HaveLength(24);
            result.Should().MatchRegex(@"^[0-9a-f]+$");
        }

        [TestMethod]
        public void CreateAlphabeticString_ProducesDifferentValues()
        {
            string a = StringFactory.CreateAlphabeticString(64);
            string b = StringFactory.CreateAlphabeticString(64);
            a.Should().NotBe(b);
        }
    }
}
