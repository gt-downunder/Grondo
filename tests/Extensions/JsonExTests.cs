using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class JsonExTests : BaseExtensionTest
    {
        private record TestDto(string Name, int Value);

        [TestMethod]
        public void ToJson_Object_ReturnsJsonString()
        {
            new TestDto("Test", 42).ToJson().Should().Contain("\"Name\"").And.Contain("\"Value\"");
        }

        [TestMethod]
        public void ToJson_Indented_ReturnsFormattedJson()
        {
            var dto = new TestDto("Test", 42);
            string json = dto.ToJson(indented: true);

            json.Should().Contain("\n");
        }

        [TestMethod]
        public void FromJson_ValidJson_ReturnsObject()
        {
            string json = "{\"Name\":\"Hello\",\"Value\":99}";
            TestDto? result = json.FromJson<TestDto>();

            result.Should().NotBeNull();
            result!.Name.Should().Be("Hello");
            result.Value.Should().Be(99);
        }

        [TestMethod]
        public void FromJson_CaseInsensitive_MatchesDifferentCasing()
        {
            string json = "{\"name\":\"Hello\",\"value\":99}";
            TestDto? result = json.FromJson<TestDto>(caseInsensitive: true);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Hello");
        }

        [TestMethod]
        public void FromJson_NullJson_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(string)!.FromJson<TestDto>())
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void TryFromJson_ValidJson_ReturnsTrueAndResult()
        {
            "{\"Name\":\"Test\",\"Value\":1}".TryFromJson<TestDto>(out TestDto? result).Should().BeTrue();
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test");
        }

        [TestMethod]
        public void TryFromJson_InvalidJson_ReturnsFalse()
        {
            "not-json".TryFromJson<TestDto>(out TestDto? result).Should().BeFalse();
            result.Should().BeNull();
        }

        [TestMethod]
        public void TryFromJson_NullOrWhitespace_ReturnsFalse()
        {
            "".TryFromJson<TestDto>(out TestDto? result).Should().BeFalse();
            result.Should().BeNull();
        }

        [TestMethod]
        public void RoundTrip_SerializeAndDeserialize_PreservesData()
        {
            new TestDto("RoundTrip", 123).ToJson().FromJson<TestDto>().Should().Be(new TestDto("RoundTrip", 123));
        }
    }
}

