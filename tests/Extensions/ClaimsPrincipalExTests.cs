using System.Security.Claims;
using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ClaimsPrincipalExTests : BaseExtensionTest
    {
        private static ClaimsPrincipal CreatePrincipal(params Claim[] claims) =>
            new(new ClaimsIdentity(claims, "TestAuth"));

        [TestMethod]
        public void GetClaimValue_ExistingClaim_ReturnsValue()
        {
            ClaimsPrincipal principal = CreatePrincipal(new Claim("role", "admin"));
            principal.GetClaimValue("role").Should().Be("admin");
        }

        [TestMethod]
        public void GetClaimValue_MissingClaim_ReturnsNull()
        {
            ClaimsPrincipal principal = CreatePrincipal();
            principal.GetClaimValue("role").Should().BeNull();
        }

        [TestMethod]
        public void GetClaimValue_NullPrincipal_ThrowsArgumentNullException()
        {
            ClaimsPrincipal principal = null!;
            Func<string?> act = () => principal.GetClaimValue("role");
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void HasClaim_ExistingClaim_ReturnsTrue()
        {
            ClaimsPrincipal principal = CreatePrincipal(new Claim("role", "admin"));
            principal.HasClaim("role").Should().BeTrue();
        }

        [TestMethod]
        public void HasClaim_MissingClaim_ReturnsFalse()
        {
            ClaimsPrincipal principal = CreatePrincipal();
            principal.HasClaim("role").Should().BeFalse();
        }

        [TestMethod]
        public void GetUserId_HasNameIdentifier_ReturnsId()
        {
            ClaimsPrincipal principal = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "user-123"));
            principal.GetUserId().Should().Be("user-123");
        }

        [TestMethod]
        public void GetUserId_NoNameIdentifier_ReturnsNull()
        {
            ClaimsPrincipal principal = CreatePrincipal();
            principal.GetUserId().Should().BeNull();
        }

        [TestMethod]
        public void GetEmail_HasEmailClaim_ReturnsEmail()
        {
            ClaimsPrincipal principal = CreatePrincipal(new Claim(ClaimTypes.Email, "user@example.com"));
            principal.GetEmail().Should().Be("user@example.com");
        }

        [TestMethod]
        public void GetEmail_NoEmailClaim_ReturnsNull()
        {
            ClaimsPrincipal principal = CreatePrincipal();
            principal.GetEmail().Should().BeNull();
        }

        [TestMethod]
        public void GetDisplayName_HasNameClaim_ReturnsName()
        {
            ClaimsPrincipal principal = CreatePrincipal(new Claim(ClaimTypes.Name, "John Doe"));
            principal.GetDisplayName().Should().Be("John Doe");
        }

        [TestMethod]
        public void GetDisplayName_NoNameClaim_ReturnsNull()
        {
            ClaimsPrincipal principal = CreatePrincipal();
            principal.GetDisplayName().Should().BeNull();
        }
    }
}

