using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class SetExTests : BaseExtensionTest
    {
        [TestMethod]
        public void EqualsSerializedSet_MatchingSets_ReturnsTrue()
        {
            new HashSet<string> { "a", "b", "c" }.EqualsSerializedSet("[\"a\",\"b\",\"c\"]").Should().BeTrue();
        }

        [TestMethod]
        public void EqualsSerializedSet_DifferentSets_ReturnsFalse()
        {
            new HashSet<string> { "a", "b" }.EqualsSerializedSet("[\"x\",\"y\"]").Should().BeFalse();
        }

        [TestMethod]
        public void EqualsSerializedSet_InvalidJson_ReturnsFalse()
        {
            new HashSet<string> { "a" }.EqualsSerializedSet("not-json").Should().BeFalse();
        }

        [TestMethod]
        public void EqualsSerializedSet_NullSet_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(ISet<string>)!.EqualsSerializedSet("[\"a\"]"))
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void EqualsSerializedSet_NullOrWhitespaceJson_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => new HashSet<string> { "a" }.EqualsSerializedSet("  "))
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void EqualsSerializedSet_IntSets_ReturnsTrue()
        {
            new HashSet<int> { 1, 2, 3 }.EqualsSerializedSet("[1,2,3]").Should().BeTrue();
        }
    }
}

