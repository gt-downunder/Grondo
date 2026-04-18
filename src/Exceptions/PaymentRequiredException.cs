using System.Net;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Represents an HTTP 402 Payment Required error, typically used to signal that
    /// access is gated behind payment, subscription, or quota requirements.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="messageHeader">An optional short header (defaults to "Payment Required").</param>
    /// <param name="innerException">An optional inner exception.</param>
    public class PaymentRequiredException(
        string message,
        string? messageHeader = null,
        Exception? innerException = null)
        : ExceptionBase(message, messageHeader ?? DefaultMessageHeader, innerException)
    {
        private const string DefaultMessageHeader = "Payment Required";

        /// <inheritdoc />
        public override HttpStatusCode StatusCode => HttpStatusCode.PaymentRequired;
    }
}
