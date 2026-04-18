using Grondo.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides conversions from <see cref="ExceptionBase"/> to RFC 7807 <see cref="ProblemDetails"/>.
    /// </summary>
    public static class ExceptionBaseEx
    {
        /// <summary>
        /// Default formatter for the RFC 7807 <c>type</c> URI. Produces a link to the MDN
        /// documentation page for the supplied HTTP status code, e.g.
        /// <c>https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404</c>.
        /// </summary>
        public static Func<int, string> DefaultProblemTypeUriFormatter { get; } =
            static code => $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{code}";

        extension(ExceptionBase exception)
        {
            /// <summary>
            /// Converts the exception to a <see cref="ProblemDetails"/> instance suitable for
            /// JSON error responses conforming to RFC 7807.
            /// </summary>
            /// <param name="instance">An optional URI reference identifying the specific occurrence.</param>
            /// <param name="typeUriFormatter">
            /// An optional formatter producing the RFC 7807 <c>type</c> URI for the status code.
            /// Defaults to <see cref="DefaultProblemTypeUriFormatter"/> (MDN documentation link).
            /// </param>
            /// <returns>A populated <see cref="ProblemDetails"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the exception is null.</exception>
            public ProblemDetails ToProblemDetails(string? instance = null, Func<int, string>? typeUriFormatter = null)
            {
                ArgumentNullException.ThrowIfNull(exception);

                ProblemDetails details = exception is ValidationException validation
                    ? new ValidationProblemDetails(ToMutableErrors(validation.Errors))
                    : new ProblemDetails();

                int statusCode = (int)exception.StatusCode;
                Func<int, string> formatter = typeUriFormatter ?? DefaultProblemTypeUriFormatter;

                details.Status = statusCode;
                details.Title = string.IsNullOrEmpty(exception.MessageHeader) ? null : exception.MessageHeader;
                details.Detail = exception.Message;
                details.Type = formatter(statusCode);

                if (!string.IsNullOrEmpty(instance))
                    details.Instance = instance;

                return details;
            }
        }

        private static Dictionary<string, string[]> ToMutableErrors(IReadOnlyDictionary<string, IReadOnlyList<string>> errors)
        {
            var result = new Dictionary<string, string[]>(errors.Count);
            foreach (KeyValuePair<string, IReadOnlyList<string>> kvp in errors)
            {
                result[kvp.Key] = [.. kvp.Value];
            }

            return result;
        }
    }
}
