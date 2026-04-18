using System.Net;
using System.Text.Json;
using Grondo.Extensions;
using Grondo.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Middleware that catches <see cref="ExceptionBase"/> (and optionally any unhandled exception)
    /// and serializes it as an RFC 7807 <see cref="ProblemDetails"/> JSON response.
    /// </summary>
    /// <param name="next">The next request delegate in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The middleware options.</param>
    public sealed partial class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        ExceptionHandlingOptions? options = null)
    {
        private readonly ExceptionHandlingOptions _options = options ?? new ExceptionHandlingOptions();

        [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Domain exception caught by middleware: {MessageHeader}")]
        private static partial void LogDomainException(ILogger logger, Exception ex, string messageHeader);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Unhandled exception caught by middleware.")]
        private static partial void LogUnhandledException(ILogger logger, Exception ex);

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (ExceptionBase ex)
            {
                LogDomainException(logger, ex, ex.MessageHeader);
                await WriteAsync(context, ex.ToProblemDetails(context.Request.Path, _options.ProblemTypeUriFormatter)).ConfigureAwait(false);
            }
            catch (Exception ex) when (_options.HandleUnexpectedExceptions)
            {
                LogUnhandledException(logger, ex);
                int statusCode = (int)HttpStatusCode.InternalServerError;
                var details = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Internal Server Error",
                    Detail = _options.IncludeUnexpectedExceptionDetails ? ex.Message : "An unexpected error occurred.",
                    Type = _options.ProblemTypeUriFormatter(statusCode),
                    Instance = context.Request.Path,
                };
                await WriteAsync(context, details).ConfigureAwait(false);
            }
        }

        private async Task WriteAsync(HttpContext context, ProblemDetails details)
        {
            if (context.Response.HasStarted) return;

            context.Response.Clear();
            context.Response.StatusCode = details.Status ?? (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            JsonSerializerOptions jsonOptions = _options.JsonSerializerOptions ?? JsonDefaults.Web;
            await JsonSerializer.SerializeAsync(context.Response.Body, details, details.GetType(), jsonOptions).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Options for <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    public sealed class ExceptionHandlingOptions
    {
        /// <summary>
        /// Whether to catch and convert exceptions that do not derive from <see cref="ExceptionBase"/>.
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool HandleUnexpectedExceptions { get; set; } = true;

        /// <summary>
        /// Whether to include the original exception message in the <c>Detail</c> field when an
        /// unexpected exception is caught. Defaults to <c>false</c> (redacted to a generic message).
        /// </summary>
        public bool IncludeUnexpectedExceptionDetails { get; set; }

        /// <summary>
        /// Custom JSON serializer options for the response body. If <c>null</c>, <see cref="JsonDefaults.Web"/> is used.
        /// </summary>
        public JsonSerializerOptions? JsonSerializerOptions { get; set; }

        /// <summary>
        /// Formatter producing the RFC 7807 <c>type</c> URI for a given HTTP status code.
        /// Defaults to <see cref="ExceptionBaseEx.DefaultProblemTypeUriFormatter"/>, which
        /// points to MDN documentation (e.g. <c>https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404</c>).
        /// Override to produce application-specific documentation URIs.
        /// </summary>
        public Func<int, string> ProblemTypeUriFormatter { get; set; } =
            ExceptionBaseEx.DefaultProblemTypeUriFormatter;
    }

    /// <summary>
    /// Extension methods for registering <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds <see cref="ExceptionHandlingMiddleware"/> to the request pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="options">Optional middleware options.</param>
        /// <returns>The application builder for chaining.</returns>
        public static IApplicationBuilder UseGrondoExceptionHandling(
            this IApplicationBuilder builder,
            ExceptionHandlingOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.UseMiddleware<ExceptionHandlingMiddleware>(options ?? new ExceptionHandlingOptions());
        }
    }
}
