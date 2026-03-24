using System.Net;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Exception for 429 Too Many Requests responses.
    /// Indicates that the user has sent too many requests in a given amount of time.
    /// </summary>
    public class TooManyRequestsException : ExceptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyRequestsException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageHeader">The error message header.</param>
        /// <param name="innerException">The inner exception.</param>
        public TooManyRequestsException(
            string message,
            string? messageHeader = null,
            Exception? innerException = null)
            : base(message, messageHeader ?? "Too Many Requests", innerException)
        {
        }

        /// <summary>Gets the HTTP status code for this exception (429).</summary>
        public override HttpStatusCode StatusCode => (HttpStatusCode)429;

        /// <summary>Gets or sets the retry-after duration.</summary>
        public TimeSpan? RetryAfter { get; init; }
    }
}

