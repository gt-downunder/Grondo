using System.Net;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Exception for 422 Unprocessable Entity responses.
    /// Indicates that the server understands the content type and syntax is correct,
    /// but was unable to process the contained instructions.
    /// </summary>
    public class UnprocessableEntityException(
        string message,
        string? messageHeader = null,
        Exception? innerException = null)
        : ExceptionBase(message, messageHeader ?? "Unprocessable Entity", innerException)
    {
        /// <summary>Gets the HTTP status code for this exception (422).</summary>
        public override HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
    }
}

