using FluentAssertions;
using Grondo.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class EnvironmentExTests : BaseExtensionTest
    {
        private class TestHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = "";
            public string ApplicationName { get; set; } = "";
            public string ContentRootPath { get; set; } = "";
            public IFileProvider ContentRootFileProvider { get; set; } = null!;
        }

        [TestMethod]
        public void IsLocal_WhenLocal_ReturnsTrue()
        {
            var env = new TestHostEnvironment { EnvironmentName = "local" };
            env.IsLocal().Should().BeTrue();
        }

        [TestMethod]
        public void IsLocal_WhenLocalCaseInsensitive_ReturnsTrue()
        {
            var env = new TestHostEnvironment { EnvironmentName = "LOCAL" };
            env.IsLocal().Should().BeTrue();
        }

        [TestMethod]
        public void IsLocal_WhenNotLocal_ReturnsFalse()
        {
            var env = new TestHostEnvironment { EnvironmentName = "production" };
            env.IsLocal().Should().BeFalse();
        }

        [TestMethod]
        public void IsTest_WhenTest_ReturnsTrue()
        {
            var env = new TestHostEnvironment { EnvironmentName = "test" };
            env.IsTest().Should().BeTrue();
        }

        [TestMethod]
        public void IsTest_WhenNotTest_ReturnsFalse()
        {
            var env = new TestHostEnvironment { EnvironmentName = "dev" };
            env.IsTest().Should().BeFalse();
        }

        [TestMethod]
        public void IsUat_WhenUat_ReturnsTrue()
        {
            var env = new TestHostEnvironment { EnvironmentName = "uat" };
            env.IsUat().Should().BeTrue();
        }

        [TestMethod]
        public void IsUat_WhenNotUat_ReturnsFalse()
        {
            var env = new TestHostEnvironment { EnvironmentName = "staging" };
            env.IsUat().Should().BeFalse();
        }

        [TestMethod]
        public void IsLocal_NullEnv_ThrowsArgumentNullException()
        {
            IHostEnvironment env = null!;
            Action act = () => env.IsLocal();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void IsTest_NullEnv_ThrowsArgumentNullException()
        {
            IHostEnvironment env = null!;
            Action act = () => env.IsTest();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void IsUat_NullEnv_ThrowsArgumentNullException()
        {
            IHostEnvironment env = null!;
            Action act = () => env.IsUat();
            act.Should().Throw<ArgumentNullException>();
        }
    }
}

