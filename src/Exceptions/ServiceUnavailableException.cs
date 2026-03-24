using System.Net;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Exception for 503 Service Unavailable responses.
    /// Indicates that the server is temporarily unable to handle the request.
    /// </summary>
    public class ServiceUnavailableException : ExceptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageHeader">The error message header.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceUnavailableException(
            string message,
            string? messageHeader = null,
            Exception? innerException = null)
            : base(message, messageHeader ?? "Service Unavailable", innerException)
        {
        }

        /// <summary>Gets the HTTP status code for this exception (503).</summary>
        public override HttpStatusCode StatusCode => HttpStatusCode.ServiceUnavailable;

        /// <summary>Gets or sets the retry-after duration.</summary>
        public TimeSpan? RetryAfter { get; init; }
    }
}

