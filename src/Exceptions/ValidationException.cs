using System.Collections.ObjectModel;
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
        /// The supplied errors are copied so the exception's <see cref="Errors"/> view
        /// is not affected by later mutations of the caller's collection.
        /// </summary>
        /// <param name="errors">The validation errors by field name.</param>
        /// <param name="messageHeader">The error message header.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="errors"/> is null.</exception>
        public ValidationException(
            IDictionary<string, string[]> errors,
            string? messageHeader = null)
            : base("One or more validation errors occurred.", messageHeader ?? "Validation Failed")
        {
            ArgumentNullException.ThrowIfNull(errors);
            Errors = Freeze(AsReadOnlySource(errors), errors.Count);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class from an
        /// already read-only error collection. Values are copied defensively.
        /// </summary>
        /// <param name="errors">The validation errors by field name.</param>
        /// <param name="messageHeader">The error message header.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="errors"/> is null.</exception>
        public ValidationException(
            IReadOnlyDictionary<string, IReadOnlyList<string>> errors,
            string? messageHeader = null)
            : base("One or more validation errors occurred.", messageHeader ?? "Validation Failed")
        {
            ArgumentNullException.ThrowIfNull(errors);
            Errors = Freeze(errors, errors.Count);
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
            var single = new Dictionary<string, IReadOnlyList<string>>(1)
            {
                [fieldName] = new ReadOnlyCollection<string>([errorMessage]),
            };
            Errors = new ReadOnlyDictionary<string, IReadOnlyList<string>>(single);
        }

        /// <summary>Gets the HTTP status code for this exception (400).</summary>
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        /// <summary>Gets the validation errors by field name as an immutable, read-only view.</summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; }

        private static IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> AsReadOnlySource(IDictionary<string, string[]> source)
        {
            // string[] already implements IReadOnlyList<string>, so this is a type-only view
            // with no per-element allocation — Freeze() does the defensive copy.
            foreach (KeyValuePair<string, string[]> kvp in source)
                yield return new KeyValuePair<string, IReadOnlyList<string>>(kvp.Key, kvp.Value);
        }

        private static ReadOnlyDictionary<string, IReadOnlyList<string>> Freeze(IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> source, int capacity)
        {
            var copy = new Dictionary<string, IReadOnlyList<string>>(capacity);
            foreach (KeyValuePair<string, IReadOnlyList<string>> kvp in source)
            {
                IReadOnlyList<string> messages = kvp.Value ?? [];
                copy[kvp.Key] = new ReadOnlyCollection<string>([.. messages]);
            }

            return new ReadOnlyDictionary<string, IReadOnlyList<string>>(copy);
        }
    }
}

