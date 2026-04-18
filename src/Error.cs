using System.Diagnostics.CodeAnalysis;

namespace Grondo
{
    /// <summary>
    /// Represents a structured, typed error with a machine-readable code and a human-readable message.
    /// Use with <see cref="Result{T, TError}"/> for domain-specific error handling beyond plain strings.
    /// </summary>
    /// <param name="Code">A short, stable identifier for the error (e.g. "user.not_found").</param>
    /// <param name="Message">A human-readable description of the error.</param>
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "'Error' is an established name for this concept in functional libraries (ErrorOr, FluentResults, LanguageExt).")]
    public record Error(string Code, string Message)
    {
        /// <summary>A generic "not found" error.</summary>
        public static Error NotFound(string message = "The requested resource was not found.")
        {
            ArgumentNullException.ThrowIfNull(message);
            return new Error("not_found", message);
        }

        /// <summary>A generic "validation failed" error.</summary>
        public static Error Validation(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return new Error("validation", message);
        }

        /// <summary>A generic "unauthorized" error.</summary>
        public static Error Unauthorized(string message = "Authentication is required.")
        {
            ArgumentNullException.ThrowIfNull(message);
            return new Error("unauthorized", message);
        }

        /// <summary>A generic "forbidden" error.</summary>
        public static Error Forbidden(string message = "Access is denied.")
        {
            ArgumentNullException.ThrowIfNull(message);
            return new Error("forbidden", message);
        }

        /// <summary>A generic "conflict" error.</summary>
        public static Error Conflict(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return new Error("conflict", message);
        }

        /// <summary>A generic "unexpected" error wrapping an exception.</summary>
        /// <param name="exception">The exception to wrap.</param>
        public static Error Unexpected(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return new Error("unexpected", exception.Message);
        }

        /// <summary>A generic "unexpected" error with a custom message.</summary>
        public static Error Unexpected(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return new Error("unexpected", message);
        }

        /// <summary>Returns the error formatted as "Code: Message".</summary>
        public override string ToString() => $"{Code}: {Message}";
    }
}
