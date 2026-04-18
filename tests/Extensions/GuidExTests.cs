using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class GuidExTests : BaseExtensionTest
    {
        [TestMethod]
        public void IsEmpty_EmptyGuid_ReturnsTrue() => Guid.Empty.IsEmpty().Should().BeTrue();

        [TestMethod]
        public void IsEmpty_NewGuid_ReturnsFalse() => Guid.NewGuid().IsEmpty().Should().BeFalse();

        [TestMethod]
        public void ToShortString_Returns22Chars()
        {
            string result = Guid.NewGuid().ToShortString();
            result.Should().HaveLength(22);
        }

        [TestMethod]
        public void ToShortString_IsUrlSafe()
        {
            string result = Guid.NewGuid().ToShortString();
            result.Should().NotContain("/");
            result.Should().NotContain("+");
            result.Should().NotContain("=");
        }

        [TestMethod]
        public void ToShortString_DifferentGuids_ProduceDifferentStrings()
        {
            string a = Guid.NewGuid().ToShortString();
            string b = Guid.NewGuid().ToShortString();
            a.Should().NotBe(b);
        }

        [TestMethod]
        public void ToShortString_SameGuid_ProducesSameString()
        {
            var guid = Guid.NewGuid();
            guid.ToShortString().Should().Be(guid.ToShortString());
        }
    }
}

