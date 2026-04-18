using FluentAssertions;
using Grondo.Extensions;
using Microsoft.Extensions.Configuration;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ConfigurationExTests : BaseExtensionTest
    {
        private IConfiguration BuildConfig(Dictionary<string, string?> data) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(data)
                .Build();

        [TestMethod]
        public void GetRequiredValue_KeyExists_ReturnsValue()
        {
            IConfiguration config = BuildConfig(new() { { "MyKey", "MyValue" } });
            config.GetRequiredValue("MyKey").Should().Be("MyValue");
        }

        [TestMethod]
        public void GetRequiredValue_KeyMissing_ThrowsInvalidOperationException()
        {
            IConfiguration config = BuildConfig(new() { { "Other", "Value" } });
            Func<string> act = () => config.GetRequiredValue("Missing");
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void GetRequiredValue_EmptyValue_ThrowsInvalidOperationException()
        {
            IConfiguration config = BuildConfig(new() { { "MyKey", "" } });
            Func<string> act = () => config.GetRequiredValue("MyKey");
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void GetRequiredValue_NullConfig_ThrowsArgumentNullException()
        {
            IConfiguration config = null!;
            Func<string> act = () => config.GetRequiredValue("key");
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetValueOrDefault_KeyExists_ReturnsValue()
        {
            IConfiguration config = BuildConfig(new() { { "Key", "Value" } });
            config.GetValueOrDefault("Key", "default").Should().Be("Value");
        }

        [TestMethod]
        public void GetValueOrDefault_KeyMissing_ReturnsDefault()
        {
            IConfiguration config = BuildConfig(new() { { "Other", "Value" } });
            config.GetValueOrDefault("Missing", "fallback").Should().Be("fallback");
        }

        [TestMethod]
        public void GetValueOrDefault_EmptyValue_ReturnsDefault()
        {
            IConfiguration config = BuildConfig(new() { { "Key", "" } });
            config.GetValueOrDefault("Key", "fallback").Should().Be("fallback");
        }

        [TestMethod]
        public void HasKey_KeyExistsWithValue_ReturnsTrue()
        {
            IConfiguration config = BuildConfig(new() { { "Key", "Value" } });
            config.HasKey("Key").Should().BeTrue();
        }

        [TestMethod]
        public void HasKey_KeyMissing_ReturnsFalse()
        {
            IConfiguration config = BuildConfig(new() { { "Other", "Value" } });
            config.HasKey("Missing").Should().BeFalse();
        }

        [TestMethod]
        public void HasKey_KeyExistsWithEmptyValue_ReturnsFalse()
        {
            IConfiguration config = BuildConfig(new() { { "Key", "" } });
            config.HasKey("Key").Should().BeFalse();
        }
    }
}

