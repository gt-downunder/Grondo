using System.Text.Json;
using FluentAssertions;
using Grondo.Utilities;

namespace Grondo.Tests.Utilities
{
    [TestClass]
    public class JsonDefaultsTests : BaseTest
    {
        private sealed class Payload
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
        }

        [TestMethod]
        public void CamelCase_SerializesPropertiesInCamelCase()
        {
            string json = JsonSerializer.Serialize(new Payload { FirstName = "Ada", LastName = "Lovelace" }, JsonDefaults.CamelCase);
            json.Should().Contain("\"firstName\"").And.Contain("\"lastName\"");
        }

        [TestMethod]
        public void SnakeCase_SerializesPropertiesInSnakeCase()
        {
            string json = JsonSerializer.Serialize(new Payload { FirstName = "Ada" }, JsonDefaults.SnakeCase);
            json.Should().Contain("\"first_name\"");
        }

        [TestMethod]
        public void NoNulls_OmitsNullProperties()
        {
            string json = JsonSerializer.Serialize(new Payload { FirstName = "Ada", LastName = null }, JsonDefaults.NoNulls);
            json.Should().NotContain("LastName").And.NotContain("last_name");
        }

        [TestMethod]
        public void Indented_ProducesMultipleLines()
        {
            string json = JsonSerializer.Serialize(new Payload { FirstName = "Ada" }, JsonDefaults.Indented);
            json.Should().Contain(Environment.NewLine);
        }

        [TestMethod]
        public void CaseInsensitive_DeserializesMixedCase()
        {
            Payload? payload = JsonSerializer.Deserialize<Payload>("{\"FIRSTNAME\":\"Ada\"}", JsonDefaults.CaseInsensitive);
            payload!.FirstName.Should().Be("Ada");
        }

        [TestMethod]
        public void Web_UsesCamelCaseAndOmitsNulls()
        {
            string json = JsonSerializer.Serialize(new Payload { FirstName = "Ada", LastName = null }, JsonDefaults.Web);
            json.Should().Contain("\"firstName\"").And.NotContain("lastName");
        }
    }
}
