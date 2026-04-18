using Microsoft.AspNetCore.Mvc;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Provides conversions from <see cref="ExceptionBase"/> to RFC 7807 <see cref="ProblemDetails"/>.
    /// </summary>
    public static class ExceptionBaseEx
    {
        extension(ExceptionBase exception)
        {
            /// <summary>
            /// Converts the exception to a <see cref="ProblemDetails"/> instance suitable for
            /// JSON error responses conforming to RFC 7807.
            /// </summary>
            /// <param name="instance">An optional URI reference identifying the specific occurrence.</param>
            /// <returns>A populated <see cref="ProblemDetails"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the exception is null.</exception>
            public ProblemDetails ToProblemDetails(string? instance = null)
            {
                ArgumentNullException.ThrowIfNull(exception);

                ProblemDetails details = exception is ValidationException validation
                    ? new ValidationProblemDetails(validation.Errors)
                    : new ProblemDetails();

                details.Status = (int)exception.StatusCode;
                details.Title = string.IsNullOrEmpty(exception.MessageHeader) ? null : exception.MessageHeader;
                details.Detail = exception.Message;
                details.Type = $"https://httpstatuses.io/{(int)exception.StatusCode}";

                if (!string.IsNullOrEmpty(instance))
                    details.Instance = instance;

                return details;
            }
        }
    }
}
