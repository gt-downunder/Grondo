using System.Diagnostics;

namespace Grondo
{
    /// <summary>
    /// Represents a validation result that can accumulate multiple errors.
    /// </summary>
    /// <typeparam name="T">The type of the validated value.</typeparam>
    [DebuggerDisplay("{IsValid ? \"Valid(\" + _value + \")\" : \"Invalid(\" + Errors.Count + \" errors)\"}")]
    public readonly struct Validation<T> : IEquatable<Validation<T>>
    {
        private readonly T? _value;
        private readonly IReadOnlyList<string>? _errors;

        private Validation(T value)
        {
            _value = value;
            _errors = null;
            IsValid = true;
        }

        private Validation(IReadOnlyList<string> errors)
        {
            _value = default;
            _errors = errors;
            IsValid = false;
        }

        /// <summary>Gets a value indicating whether the validation succeeded.</summary>
        public bool IsValid { get; }

        /// <summary>Gets a value indicating whether the validation failed.</summary>
        public bool IsInvalid => !IsValid;

        /// <summary>
        /// Gets the validated value. Throws if invalid.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the validation is invalid.</exception>
        public T Value => IsValid
            ? _value!
            : throw new InvalidOperationException(
                $"Cannot access Value on invalid validation. Errors: {string.Join(", ", _errors!)}");

        /// <summary>Gets the validation errors. Empty if valid.</summary>
        public IReadOnlyList<string> Errors => _errors ?? Array.Empty<string>();

        /// <summary>
        /// Creates a valid result.
        /// </summary>
        /// <param name="value">The validated value.</param>
        /// <returns>A valid validation result.</returns>
        public static Validation<T> Valid(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Validation<T>(value);
        }

        /// <summary>
        /// Creates an invalid result with errors.
        /// </summary>
        /// <param name="errors">The validation errors.</param>
        /// <returns>An invalid validation result.</returns>
        public static Validation<T> Invalid(params string[] errors)
        {
            if (errors.Length == 0)
                throw new ArgumentException("Must provide at least one error", nameof(errors));
            return new Validation<T>(errors);
        }

        /// <summary>
        /// Creates an invalid result with errors.
        /// </summary>
        /// <param name="errors">The validation errors.</param>
        /// <returns>An invalid validation result.</returns>
        public static Validation<T> Invalid(IEnumerable<string> errors)
        {
            var errorList = errors.ToList();
            if (errorList.Count == 0)
                throw new ArgumentException("Must provide at least one error", nameof(errors));
            return new Validation<T>(errorList);
        }

        /// <summary>
        /// Combines multiple validations, accumulating all errors.
        /// </summary>
        /// <param name="validations">The validations to combine.</param>
        /// <returns>A valid result if all are valid, otherwise an invalid result with all errors.</returns>
        public static Validation<T> Combine(params Validation<T>[] validations)
        {
            var allErrors = validations
                .Where(v => v.IsInvalid)
                .SelectMany(v => v.Errors)
                .ToList();

            if (allErrors.Count > 0)
                return Invalid(allErrors);

            // All valid - return first value (they should all be equivalent)
            return validations.First(v => v.IsValid);
        }

        /// <summary>
        /// Transforms the value if valid.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The function to transform the value.</param>
        /// <returns>A validation with the transformed value, or the original errors.</returns>
        public Validation<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsValid
                ? Validation<TResult>.Valid(mapper(_value!))
                : Validation<TResult>.Invalid(_errors!);
        }

        /// <summary>
        /// Chains validations, accumulating errors.
        /// </summary>
        /// <typeparam name="TResult">The type of the new validation.</typeparam>
        /// <param name="binder">The function that returns a new validation.</param>
        /// <returns>The result of the binder, or the accumulated errors.</returns>
        public Validation<TResult> Bind<TResult>(Func<T, Validation<TResult>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            if (IsInvalid)
                return Validation<TResult>.Invalid(_errors!);

            Validation<TResult> result = binder(_value!);
            return result;
        }

        /// <summary>
        /// Pattern matches on the validation result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="onValid">The function to execute if valid.</param>
        /// <param name="onInvalid">The function to execute if invalid.</param>
        /// <returns>The result of the matching function.</returns>
        public TResult Match<TResult>(
            Func<T, TResult> onValid,
            Func<IReadOnlyList<string>, TResult> onInvalid)
        {
            ArgumentNullException.ThrowIfNull(onValid);
            ArgumentNullException.ThrowIfNull(onInvalid);
            return IsValid ? onValid(_value!) : onInvalid(_errors!);
        }

        /// <summary>
        /// Converts to Result, combining errors into single message.
        /// </summary>
        /// <returns>A Result with the value or combined error message.</returns>
        public Result<T> ToResult()
        {
            return IsValid
                ? Result<T>.Success(_value!)
                : Result<T>.Failure(string.Join("; ", _errors!));
        }

        /// <summary>
        /// Converts this <see cref="Validation{T}"/> to a <see cref="Maybe{T}"/>.
        /// Valid becomes Some; Invalid becomes None (errors are discarded).
        /// </summary>
        /// <returns>A Maybe containing the value, or None.</returns>
        public Maybe<T> ToMaybe() =>
            IsValid ? Maybe<T>.Some(_value!) : Maybe<T>.None;

        /// <summary>
        /// Converts this <see cref="Validation{T}"/> to an <see cref="Either{TLeft, TRight}"/>.
        /// Valid becomes Right; Invalid becomes Left preserving all errors as a list.
        /// </summary>
        /// <returns>An Either with Right(value) if valid, or Left(errors) if invalid.</returns>
        public Either<IReadOnlyList<string>, T> ToEither() =>
            IsValid
                ? Either<IReadOnlyList<string>, T>.FromRight(_value!)
                : Either<IReadOnlyList<string>, T>.FromLeft(_errors!);

        /// <summary>
        /// Applicative apply: applies a <see cref="Validation{TFunc}"/> of a function to this
        /// <see cref="Validation{T}"/>. Unlike Bind, failures on both sides are accumulated.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="validatedFunc">A validation containing the function to apply.</param>
        /// <returns>A validation with the applied result, or accumulated errors from both sides.</returns>
        public Validation<TResult> Apply<TResult>(Validation<Func<T, TResult>> validatedFunc)
        {
            if (IsValid && validatedFunc.IsValid)
                return Validation<TResult>.Valid(validatedFunc._value!(_value!));

            var allErrors = new List<string>();
            if (validatedFunc.IsInvalid) allErrors.AddRange(validatedFunc._errors!);
            if (IsInvalid) allErrors.AddRange(_errors!);
            return Validation<TResult>.Invalid(allErrors);
        }

        /// <summary>
        /// Asynchronously transforms the value if valid.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The async function to transform the value.</param>
        /// <returns>A validation with the transformed value, or the original errors.</returns>
        public async Task<Validation<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsValid
                ? Validation<TResult>.Valid(await mapper(_value!).ConfigureAwait(false))
                : Validation<TResult>.Invalid(_errors!);
        }

        /// <summary>
        /// Asynchronously chains validations.
        /// </summary>
        /// <typeparam name="TResult">The type of the new validation.</typeparam>
        /// <param name="binder">The async function that returns a new validation.</param>
        /// <returns>The result of the binder, or the accumulated errors.</returns>
        public async Task<Validation<TResult>> BindAsync<TResult>(Func<T, Task<Validation<TResult>>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            if (IsInvalid)
                return Validation<TResult>.Invalid(_errors!);

            return await binder(_value!).ConfigureAwait(false);
        }

        // IEquatable implementation
        /// <summary>
        /// Determines whether the specified <see cref="Validation{T}"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The validation to compare with the current instance.</param>
        /// <returns>true if the specified validation is equal to the current instance; otherwise, false.</returns>
        public bool Equals(Validation<T> other)
        {
            if (IsValid != other.IsValid) return false;
            if (IsValid)
                return EqualityComparer<T>.Default.Equals(_value, other._value);

            return _errors!.SequenceEqual(other._errors!);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the specified object is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object? obj) =>
            obj is Validation<T> other && Equals(other);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            if (IsValid)
                return HashCode.Combine(IsValid, _value);

            var hash = new HashCode();
            hash.Add(IsValid);
            foreach (var error in _errors!)
                hash.Add(error);
            return hash.ToHashCode();
        }

        /// <summary>
        /// Determines whether two <see cref="Validation{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">The first validation to compare.</param>
        /// <param name="right">The second validation to compare.</param>
        /// <returns>true if the validations are equal; otherwise, false.</returns>
        public static bool operator ==(Validation<T> left, Validation<T> right) =>
            left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Validation{T}"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first validation to compare.</param>
        /// <param name="right">The second validation to compare.</param>
        /// <returns>true if the validations are not equal; otherwise, false.</returns>
        public static bool operator !=(Validation<T> left, Validation<T> right) =>
            !left.Equals(right);

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string in the form "Valid(value)" or "Invalid(errors)".</returns>
        public override string ToString() =>
            IsValid ? $"Valid({_value})" : $"Invalid({string.Join(", ", _errors!)})";
    }
}

