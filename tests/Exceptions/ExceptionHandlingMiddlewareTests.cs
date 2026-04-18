using System.Text;
using System.Text.Json;
using FluentAssertions;
using Grondo.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Grondo.Tests.Exceptions
{
    [TestClass]
    public class ExceptionHandlingMiddlewareTests : BaseExceptionTest
    {
        private static async Task<(int status, string contentType, string body)> RunAsync(RequestDelegate next, ExceptionHandlingOptions? options = null)
        {
            var middleware = new ExceptionHandlingMiddleware(next, NullLogger<ExceptionHandlingMiddleware>.Instance, options);
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            await using var ms = new MemoryStream();
            context.Response.Body = ms;

            await middleware.InvokeAsync(context);

            ms.Position = 0;
            string body = await new StreamReader(ms, Encoding.UTF8).ReadToEndAsync();
            return (context.Response.StatusCode, context.Response.ContentType ?? string.Empty, body);
        }

        [TestMethod]
        public async Task NoException_PassesThrough()
        {
            (int status, _, string body) = await RunAsync(ctx => { ctx.Response.StatusCode = 200; return Task.CompletedTask; });
            status.Should().Be(200);
            body.Should().BeEmpty();
        }

        [TestMethod]
        public async Task ExceptionBase_IsConvertedToProblemDetails()
        {
            (int status, string contentType, string body) = await RunAsync(_ => throw new EntityNotFoundException("User not found"));

            status.Should().Be(404);
            contentType.Should().StartWith("application/problem+json");

            using var doc = JsonDocument.Parse(body);
            doc.RootElement.GetProperty("status").GetInt32().Should().Be(404);
            doc.RootElement.GetProperty("detail").GetString().Should().Be("User not found");
            doc.RootElement.GetProperty("instance").GetString().Should().Be("/api/test");
        }

        [TestMethod]
        public async Task UnexpectedException_Handled_WhenEnabled()
        {
            (int status, _, string body) = await RunAsync(
                _ => throw new InvalidOperationException("boom"),
                new ExceptionHandlingOptions { HandleUnexpectedExceptions = true });

            status.Should().Be(500);
            using var doc = JsonDocument.Parse(body);
            doc.RootElement.GetProperty("detail").GetString().Should().Be("An unexpected error occurred.");
        }

        [TestMethod]
        public async Task UnexpectedException_IncludesDetails_WhenOptedIn()
        {
            (_, _, string body) = await RunAsync(
                _ => throw new InvalidOperationException("specific cause"),
                new ExceptionHandlingOptions { HandleUnexpectedExceptions = true, IncludeUnexpectedExceptionDetails = true });

            using var doc = JsonDocument.Parse(body);
            doc.RootElement.GetProperty("detail").GetString().Should().Be("specific cause");
        }

        [TestMethod]
        public async Task UnexpectedException_Rethrown_WhenDisabled()
        {
            await FluentActions.Invoking(() => RunAsync(
                    _ => throw new InvalidOperationException("boom"),
                    new ExceptionHandlingOptions { HandleUnexpectedExceptions = false }))
                .Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
