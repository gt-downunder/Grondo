using System.Net;

namespace Grondo.Exceptions
{
    /// <summary>
    /// Exception for validation errors with field-level details.
    /// Returns a 400 Bad Request status code with detailed validation errors.
    /// </summary>
    public class ValidationException : ExceptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="errors">The validation errors by field name.</param>
        /// <param name="messageHeader">The error message header.</param>
        public ValidationException(
            IDictionary<string, string[]> errors,
            string? messageHeader = null)
            : base("One or more validation errors occurred.", messageHeader ?? "Validation Failed")
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a single error.
        /// </summary>
        /// <param name="fieldName">The name of the field with the error.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="messageHeader">The error message header.</param>
        public ValidationException(
            string fieldName,
            string errorMessage,
            string? messageHeader = null)
            : base($"{fieldName}: {errorMessage}", messageHeader ?? "Validation Failed")
        {
            Errors = new Dictionary<string, string[]>
            {
                [fieldName] = new[] { errorMessage }
            };
        }

        /// <summary>Gets the HTTP status code for this exception (400).</summary>
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        /// <summary>Gets the validation errors by field name.</summary>
        public IDictionary<string, string[]> Errors { get; }
    }
}

