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

        [TestMethod]
        public void AppendPath_AddsSegment_WithoutDoubleSlash()
        {
            var uri = new Uri("https://example.com/api/");
            uri.AppendPath("/users").AbsoluteUri.Should().Be("https://example.com/api/users");
        }

        [TestMethod]
        public void AppendPath_NoSlashes_InsertsSlash()
        {
            var uri = new Uri("https://example.com/api");
            uri.AppendPath("users").AbsoluteUri.Should().Be("https://example.com/api/users");
        }

        [TestMethod]
        public void AppendPath_PreservesQuery()
        {
            var uri = new Uri("https://example.com/api?x=1");
            uri.AppendPath("users").AbsoluteUri.Should().Contain("?x=1");
        }

        [TestMethod]
        public void GetQueryParameters_ParsesQueryString()
        {
            var uri = new Uri("https://example.com?a=1&b=two&c=");
            IReadOnlyDictionary<string, string> result = uri.GetQueryParameters();
            result["a"].Should().Be("1");
            result["b"].Should().Be("two");
            result["c"].Should().Be(string.Empty);
        }

        [TestMethod]
        public void GetQueryParameters_NoQuery_ReturnsEmpty()
        {
            var uri = new Uri("https://example.com/api");
            uri.GetQueryParameters().Should().BeEmpty();
        }

        [TestMethod]
        public void GetQueryParameters_DecodesValues()
        {
            var uri = new Uri("https://example.com?name=John%20Doe");
            uri.GetQueryParameters()["name"].Should().Be("John Doe");
        }

        [TestMethod]
        public void WithQueryParameter_AddsNewParameter()
        {
            var uri = new Uri("https://example.com/api");
            Uri result = uri.WithQueryParameter("x", "1");
            result.Query.Should().Contain("x=1");
        }

        [TestMethod]
        public void WithQueryParameter_ReplacesExistingParameter()
        {
            var uri = new Uri("https://example.com/api?x=1");
            Uri result = uri.WithQueryParameter("x", "2");
            result.GetQueryParameters()["x"].Should().Be("2");
        }

        [TestMethod]
        public void WithQueryParameter_EncodesValues()
        {
            var uri = new Uri("https://example.com");
            Uri result = uri.WithQueryParameter("name", "John Doe");
            result.Query.Should().Contain("name=John%20Doe");
        }
    }
}

