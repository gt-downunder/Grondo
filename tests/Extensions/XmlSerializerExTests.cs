using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class XmlSerializerExTests : BaseExtensionTest
    {
        public class TestEntity
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        [TestMethod]
        public void Serialize_ValidEntity_ReturnsXmlString()
        {
            new TestEntity { Name = "Test", Value = 42 }.Serialize().Should().Contain("<Name>Test</Name>")
                .And.Contain("<Value>42</Value>");
        }

        [TestMethod]
        public void Serialize_NullEntity_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(TestEntity)!.Serialize())
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Deserialize_ValidXml_ReturnsEntity()
        {
            var entity = new TestEntity { Name = "Hello", Value = 7 };
            TestEntity? result = entity.Serialize().Deserialize<TestEntity>();

            result.Should().NotBeNull();
            result!.Name.Should().Be("Hello");
            result.Value.Should().Be(7);
        }

        [TestMethod]
        public void Deserialize_NullOrWhitespace_ReturnsDefault()
        {
            "".Deserialize<TestEntity>().Should().BeNull();
        }

        [TestMethod]
        public void RoundTrip_ComplexEntity_PreservesData()
        {
            var original = new TestEntity { Name = "Round Trip", Value = 999 };
            TestEntity? deserialized = original.Serialize().Deserialize<TestEntity>();

            deserialized.Should().NotBeNull();
            deserialized!.Name.Should().Be(original.Name);
            deserialized.Value.Should().Be(original.Value);
        }
    }
}

