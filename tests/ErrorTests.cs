using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ErrorTests : BaseTest
    {
        [TestMethod]
        public void Constructor_SetsCodeAndMessage()
        {
            var err = new Error("user.not_found", "User 42 was not found");
            err.Code.Should().Be("user.not_found");
            err.Message.Should().Be("User 42 was not found");
        }

        [TestMethod]
        public void ToString_FormatsAsCodeColonMessage()
        {
            var err = new Error("x", "y");
            err.ToString().Should().Be("x: y");
        }

        [TestMethod]
        public void NotFound_ReturnsStandardCode()
        {
            var err = Error.NotFound("missing");
            err.Code.Should().Be("not_found");
            err.Message.Should().Be("missing");
        }

        [TestMethod]
        public void NotFound_WithoutMessage_UsesDefault()
        {
            var err = Error.NotFound();
            err.Code.Should().Be("not_found");
            err.Message.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void Validation_ReturnsStandardCode()
        {
            Error.Validation("bad").Code.Should().Be("validation");
        }

        [TestMethod]
        public void Unauthorized_ReturnsStandardCode()
        {
            Error.Unauthorized().Code.Should().Be("unauthorized");
        }

        [TestMethod]
        public void Forbidden_ReturnsStandardCode()
        {
            Error.Forbidden().Code.Should().Be("forbidden");
        }

        [TestMethod]
        public void Conflict_ReturnsStandardCode()
        {
            Error.Conflict("dup").Code.Should().Be("conflict");
        }

        [TestMethod]
        public void Unexpected_WithException_WrapsMessage()
        {
            var ex = new InvalidOperationException("boom");
            var err = Error.Unexpected(ex);
            err.Code.Should().Be("unexpected");
            err.Message.Should().Be("boom");
        }

        [TestMethod]
        public void Unexpected_WithNullException_Throws()
        {
            Action act = () => Error.Unexpected((Exception)null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Records_AreValueEqual()
        {
            var a = new Error("x", "y");
            var b = new Error("x", "y");
            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }
    }
}
