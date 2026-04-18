namespace Grondo
{
    /// <summary>
    /// Represents the outcome of an operation that can either succeed with a value of type
    /// <typeparamref name="T"/> or fail with an error of type <typeparamref name="TError"/>.
    /// This is the "typed error" counterpart to <see cref="Result{T}"/>, typically used with
    /// the <see cref="Error"/> record or a domain-specific error type/enum.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{IsSuccess ? \"Success(\" + _value + \")\" : \"Failure(\" + _error + \")\"}")]
    public readonly struct Result<T, TError> : IEquatable<Result<T, TError>>
    {
        private readonly T? _value;
        private readonly TError? _error;

        private Result(T value)
        {
            _value = value;
            _error = default;
            IsSuccess = true;
        }

        private Result(TError error)
        {
            _value = default;
            _error = error;
            IsSuccess = false;
        }

        /// <summary>Gets a value indicating whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>Gets a value indicating whether the operation failed.</summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>Gets the success value. Throws if the result is a failure.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a failure.</exception>
        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException($"Cannot access Value on a failed result. Error: {_error}");

        /// <summary>Gets the error value. Throws if the result is a success.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a success.</exception>
        public TError Error => IsFailure
            ? _error!
            : throw new InvalidOperationException("Cannot access Error on a successful result.");

        /// <summary>Creates a successful result with the specified value.</summary>
        public static Result<T, TError> Success(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Result<T, TError>(value);
        }

        /// <summary>Creates a failed result with the specified error.</summary>
        public static Result<T, TError> Failure(TError error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new Result<T, TError>(error);
        }

        /// <summary>Alias for <see cref="Success(T)"/>.</summary>
        public static Result<T, TError> Ok(T value) => Success(value);

        /// <summary>Implicitly converts a value to a successful result.</summary>
        public static implicit operator Result<T, TError>(T value) => Success(value);

        /// <summary>Returns the value if successful; otherwise the fallback.</summary>
        public T GetValueOrDefault(T fallback) => IsSuccess ? _value! : fallback;

        /// <summary>Transforms the success value using the specified function.</summary>
        public Result<TNew, TError> Map<TNew>(Func<T, TNew> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsSuccess
                ? Result<TNew, TError>.Success(mapper(_value!))
                : Result<TNew, TError>.Failure(_error!);
        }

        /// <summary>Transforms the error value using the specified function.</summary>
        public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsFailure
                ? Result<T, TNewError>.Failure(mapper(_error!))
                : Result<T, TNewError>.Success(_value!);
        }

        /// <summary>Chains with another operation that returns a typed-error result.</summary>
        public Result<TNew, TError> Bind<TNew>(Func<T, Result<TNew, TError>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return IsSuccess ? binder(_value!) : Result<TNew, TError>.Failure(_error!);
        }

        /// <summary>Pattern matches on the result.</summary>
        public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TError, TOut> onFailure)
        {
            ArgumentNullException.ThrowIfNull(onSuccess);
            ArgumentNullException.ThrowIfNull(onFailure);
            return IsSuccess ? onSuccess(_value!) : onFailure(_error!);
        }

        /// <summary>Executes a side-effect on the success value without changing the result.</summary>
        public Result<T, TError> Tap(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (IsSuccess) action(_value!);
            return this;
        }

        /// <summary>Executes a side-effect on the error value without changing the result.</summary>
        public Result<T, TError> TapError(Action<TError> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (IsFailure) action(_error!);
            return this;
        }

        /// <summary>Validates the success value against a predicate.</summary>
        public Result<T, TError> Ensure(Func<T, bool> predicate, TError error)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(error);
            return IsSuccess && !predicate(_value!) ? Failure(error) : this;
        }

        /// <summary>Recovers from failure by converting the error to a success value.</summary>
        public Result<T, TError> Recover(Func<TError, T> recovery)
        {
            ArgumentNullException.ThrowIfNull(recovery);
            return IsSuccess ? this : Success(recovery(_error!));
        }

        /// <summary>Converts to a <see cref="Maybe{T}"/>. Success becomes Some; Failure becomes None.</summary>
        public Maybe<T> ToMaybe() => IsSuccess ? Maybe<T>.Some(_value!) : Maybe<T>.None;

        /// <summary>Converts to an <see cref="Either{TError, T}"/>. Success becomes Right; Failure becomes Left.</summary>
        public Either<TError, T> ToEither() =>
            IsSuccess ? Either<TError, T>.FromRight(_value!) : Either<TError, T>.FromLeft(_error!);

        /// <summary>Asynchronously transforms the success value.</summary>
        public async Task<Result<TNew, TError>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsSuccess
                ? Result<TNew, TError>.Success(await mapper(_value!).ConfigureAwait(false))
                : Result<TNew, TError>.Failure(_error!);
        }

        /// <summary>Asynchronously chains with another typed-error result-returning operation.</summary>
        public async Task<Result<TNew, TError>> BindAsync<TNew>(Func<T, Task<Result<TNew, TError>>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return IsSuccess
                ? await binder(_value!).ConfigureAwait(false)
                : Result<TNew, TError>.Failure(_error!);
        }

        /// <summary>Enables LINQ query syntax support (alias for Map).</summary>
        public Result<TResult, TError> Select<TResult>(Func<T, TResult> selector) => Map(selector);

        /// <summary>Enables LINQ query syntax support for chaining operations.</summary>
        public Result<TResult, TError> SelectMany<TIntermediate, TResult>(
            Func<T, Result<TIntermediate, TError>> selector,
            Func<T, TIntermediate, TResult> projector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(projector);
            return Bind(t => selector(t).Map(i => projector(t, i)));
        }

        /// <summary>Determines whether the specified instance is equal to the current instance.</summary>
        public bool Equals(Result<T, TError> other)
        {
            if (IsSuccess != other.IsSuccess) return false;
            return IsSuccess
                ? EqualityComparer<T>.Default.Equals(_value, other._value)
                : EqualityComparer<TError>.Default.Equals(_error, other._error);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Result<T, TError> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => IsSuccess ? HashCode.Combine(IsSuccess, _value) : HashCode.Combine(IsSuccess, _error);

        /// <summary>Equality operator.</summary>
        public static bool operator ==(Result<T, TError> left, Result<T, TError> right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(Result<T, TError> left, Result<T, TError> right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString() => IsSuccess ? $"Success({_value})" : $"Failure({_error})";
    }
}
