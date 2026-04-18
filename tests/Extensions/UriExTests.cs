using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class UriExTests : BaseExtensionTest
    {
        [TestMethod]
        public void DumpProperties_ValidUri_ReturnsProperties()
        {
            var uri = new Uri("https://example.com:8080/path?q=1#frag");
            IReadOnlyList<string> result = uri.DumpProperties();

            result.Should().NotBeEmpty();
            result.Should().Contain(s => s.Contains("Host: example.com"));
            result.Should().Contain(s => s.Contains("Port: 8080"));
            result.Should().Contain(s => s.Contains("Scheme: https"));
        }

        [TestMethod]
        public void DumpProperties_NullUri_ThrowsArgumentNullException()
        {
            Uri uri = null!;
            Func<IReadOnlyList<string>> act = () => uri.DumpProperties();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void DumpProperties_IncludesSegments()
        {
            var uri = new Uri("https://example.com/api/users");
            IReadOnlyList<string> result = uri.DumpProperties();

            result.Should().Contain(s => s.StartsWith("Segment"));
        }
    }
}

