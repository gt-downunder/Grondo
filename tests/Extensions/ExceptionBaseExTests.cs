using FluentAssertions;
using Grondo.Exceptions;
using Grondo.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ExceptionBaseExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ToProblemDetails_SetsStatusTitleAndDetail()
        {
            var ex = new EntityNotFoundException("User 42 was not found.");
            ProblemDetails details = ex.ToProblemDetails();

            details.Status.Should().Be(404);
            details.Title.Should().Be("Not found");
            details.Detail.Should().Be("User 42 was not found.");
            details.Type.Should().Be("https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404");
        }

        [TestMethod]
        public void ToProblemDetails_WithInstance_SetsInstance()
        {
            var ex = new BadRequestException("invalid");
            ProblemDetails details = ex.ToProblemDetails("/api/users/abc");
            details.Instance.Should().Be("/api/users/abc");
        }

        [TestMethod]
        public void ToProblemDetails_WithCustomTypeUriFormatter_UsesFormatter()
        {
            var ex = new EntityNotFoundException("missing");
            ProblemDetails details = ex.ToProblemDetails(
                typeUriFormatter: code => $"https://example.com/errors/{code}");

            details.Type.Should().Be("https://example.com/errors/404");
        }

        [TestMethod]
        public void ToProblemDetails_ValidationException_ReturnsValidationProblemDetails()
        {
            var errors = new Dictionary<string, string[]> { ["Name"] = ["Required."] };
            var ex = new ValidationException(errors);

            var details = ex.ToProblemDetails();

            details.Should().BeOfType<ValidationProblemDetails>();
            ((ValidationProblemDetails)details).Errors.Should().ContainKey("Name");
            details.Status.Should().Be(400);
        }

        [TestMethod]
        public void ToProblemDetails_CustomHeader_UsedAsTitle()
        {
            var ex = new ConflictException("A user with that email already exists.", "Email Conflict");
            ex.ToProblemDetails().Title.Should().Be("Email Conflict");
        }
    }
}
