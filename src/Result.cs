namespace Grondo
{
    /// <summary>
    /// Represents the outcome of an operation that can either succeed with a value or fail with an error message.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{IsSuccess ? \"Success(\" + _value + \")\" : \"Failure(\" + _error + \")\"}}")]
    public readonly struct Result<T> : IEquatable<Result<T>>
    {
        private readonly T? _value;
        private readonly string? _error;

        private Result(T value)
        {
            _value = value;
            _error = null;
            IsSuccess = true;
        }

        private Result(string error)
        {
            _value = default;
            _error = error;
            IsSuccess = false;
        }

        /// <summary>Gets a value indicating whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>Gets a value indicating whether the operation failed.</summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the success value. Throws if the result is a failure.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a failure.</exception>
        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException($"Cannot access Value on a failed result. Error: {_error}");

        /// <summary>
        /// Gets the error message. Throws if the result is a success.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a success.</exception>
        public string Error => IsFailure
            ? _error ?? "Result was created using default constructor"
            : throw new InvalidOperationException("Cannot access Error on a successful result.");

        /// <summary>Creates a successful result with the specified value.</summary>
        /// <param name="value">The success value.</param>
        /// <returns>A successful <see cref="Result{T}"/>.</returns>
        public static Result<T> Success(T value) => new(value);

        /// <summary>Creates a failed result with the specified error message.</summary>
        /// <param name="error">The error message.</param>
        /// <returns>A failed <see cref="Result{T}"/>.</returns>
        public static Result<T> Failure(string error) => new(error);

        /// <summary>Implicitly converts a value to a successful result.</summary>
        /// <param name="value">The value to wrap.</param>
        public static implicit operator Result<T>(T value) => Success(value);

        /// <summary>
        /// Gets the value if successful, or the specified fallback value if failed.
        /// </summary>
        /// <param name="fallback">The fallback value to return if the result is a failure.</param>
        /// <returns>The success value, or <paramref name="fallback"/> if the result is a failure.</returns>
        public T GetValueOrDefault(T fallback) => IsSuccess ? _value! : fallback;

        /// <summary>
        /// Transforms the success value using the specified function.
        /// </summary>
        /// <typeparam name="TNew">The type of the new success value.</typeparam>
        /// <param name="mapper">The function to transform the success value.</param>
        /// <returns>A new result with the transformed value, or the original failure.</returns>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper) =>
            IsSuccess ? Result<TNew>.Success(mapper(_value!)) : Result<TNew>.Failure(_error!);

        /// <summary>
        /// Chains this result with another operation that returns a Result.
        /// </summary>
        /// <typeparam name="TNew">The type of the new success value.</typeparam>
        /// <param name="binder">The function that returns a new Result.</param>
        /// <returns>The result of the binder, or the original failure.</returns>
        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder) =>
            IsSuccess ? binder(_value!) : Result<TNew>.Failure(_error!);

        /// <summary>
        /// Pattern-matches on the result, applying <paramref name="onSuccess"/> if the result is successful,
        /// or <paramref name="onFailure"/> if the result is a failure.
        /// </summary>
        /// <typeparam name="TOut">The return type of the match.</typeparam>
        /// <param name="onSuccess">The function to apply to the value if the result is successful.</param>
        /// <param name="onFailure">The function to apply to the error message if the result is a failure.</param>
        /// <returns>The result of applying the appropriate function.</returns>
        public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure) =>
            IsSuccess ? onSuccess(_value!) : onFailure(_error!);

        /// <summary>
        /// Executes a side-effect action on the success value without changing the result.
        /// If the result is a failure, the action is not executed and the failure propagates unchanged.
        /// </summary>
        /// <param name="action">The action to execute on the success value.</param>
        /// <returns>The original result, unchanged.</returns>
        public Result<T> Tap(Action<T> action)
        {
            if (IsSuccess) action(_value!);
            return this;
        }

        /// <summary>
        /// Transforms the error message using the specified function.
        /// If the result is a success, the mapper is not executed and the success propagates unchanged.
        /// </summary>
        /// <param name="mapper">The function to transform the error message.</param>
        /// <returns>A new result with the transformed error, or the original success.</returns>
        public Result<T> MapError(Func<string, string> mapper) =>
            IsFailure ? Result<T>.Failure(mapper(_error!)) : this;

        /// <summary>
        /// Validates the success value against a predicate. If the predicate returns <c>false</c>,
        /// the result is converted to a failure with the specified error message.
        /// If the result is already a failure, it propagates unchanged.
        /// </summary>
        /// <param name="predicate">The validation predicate to apply to the success value.</param>
        /// <param name="error">The error message to use if the predicate fails.</param>
        /// <returns>The original success if the predicate passes, or a failure result.</returns>
        public Result<T> Ensure(Func<T, bool> predicate, string error) =>
            IsSuccess && !predicate(_value!) ? Result<T>.Failure(error) : this;

        /// <summary>
        /// Asynchronously transforms the success value using the specified function.
        /// If the result is a failure, the mapper is not executed and the failure propagates unchanged.
        /// </summary>
        /// <typeparam name="TNew">The type of the new success value.</typeparam>
        /// <param name="mapper">The async function to transform the success value.</param>
        /// <returns>A new result with the transformed value, or the original failure.</returns>
        public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper) =>
            IsSuccess ? Result<TNew>.Success(await mapper(_value!).ConfigureAwait(false)) : Result<TNew>.Failure(_error!);

        /// <summary>
        /// Asynchronously chains this result with another operation that returns a Result.
        /// If the result is a failure, the binder is not executed and the failure propagates unchanged.
        /// </summary>
        /// <typeparam name="TNew">The type of the new success value.</typeparam>
        /// <param name="binder">The async function that returns a new Result.</param>
        /// <returns>The result of the binder, or the original failure.</returns>
        public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> binder) =>
            IsSuccess ? await binder(_value!).ConfigureAwait(false) : Result<TNew>.Failure(_error!);

        /// <summary>
        /// Asynchronously executes a side-effect action on the success value without changing the result.
        /// If the result is a failure, the action is not executed and the failure propagates unchanged.
        /// </summary>
        /// <param name="action">The async action to execute on the success value.</param>
        /// <returns>The original result, unchanged.</returns>
        public async Task<Result<T>> TapAsync(Func<T, Task> action)
        {
            if (IsSuccess) await action(_value!).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Executes a side-effect action on the error message without changing the result.
        /// If the result is a success, the action is not executed and the success propagates unchanged.
        /// </summary>
        /// <param name="action">The action to execute on the error message.</param>
        /// <returns>The original result, unchanged.</returns>
        public Result<T> TapError(Action<string> action)
        {
            if (IsFailure) action(_error!);
            return this;
        }

        /// <summary>
        /// Asynchronously executes a side-effect action on the error message without changing the result.
        /// If the result is a success, the action is not executed and the success propagates unchanged.
        /// </summary>
        /// <param name="action">The async action to execute on the error message.</param>
        /// <returns>The original result, unchanged.</returns>
        public async Task<Result<T>> TapErrorAsync(Func<string, Task> action)
        {
            if (IsFailure) await action(_error!).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Converts this <see cref="Result{T}"/> to a <see cref="Maybe{T}"/>.
        /// Returns <see cref="Maybe{T}.Some(T)"/> if successful; otherwise <see cref="Maybe{T}.None"/>.
        /// </summary>
        /// <returns>A Maybe containing the value if successful, or None.</returns>
        public Maybe<T> ToMaybe() =>
            IsSuccess ? Maybe<T>.Some(_value!) : Maybe<T>.None;

        /// <summary>
        /// Converts this <see cref="Result{T}"/> to an <see cref="Either{TLeft, TRight}"/>.
        /// Success becomes Right; Failure becomes Left with the error message.
        /// </summary>
        /// <returns>An Either with Right(value) if successful, or Left(error).</returns>
        public Either<string, T> ToEither() =>
            IsSuccess ? Either<string, T>.FromRight(_value!) : Either<string, T>.FromLeft(_error!);

        /// <summary>
        /// Converts this <see cref="Result{T}"/> to a <see cref="Validation{T}"/>.
        /// Success becomes Valid; Failure becomes Invalid with a single error.
        /// </summary>
        /// <returns>A Validation with the value if successful, or a single validation error.</returns>
        public Validation<T> ToValidation() =>
            IsSuccess ? Validation<T>.Valid(_value!) : Validation<T>.Invalid(_error!);

        /// <summary>
        /// Returns a string representation of this result.
        /// </summary>
        /// <returns>A string in the form "Success(value)" or "Failure(error)".</returns>
        public override string ToString() =>
            IsSuccess ? $"Success({_value})" : $"Failure({_error})";

        /// <summary>
        /// Determines whether the specified <see cref="Result{T}"/> is equal to the current <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="other">The result to compare with the current result.</param>
        /// <returns>true if the specified result is equal to the current result; otherwise, false.</returns>
        public bool Equals(Result<T> other)
        {
            if (IsSuccess != other.IsSuccess)
                return false;

            return IsSuccess
                ? EqualityComparer<T>.Default.Equals(_value, other._value)
                : _error == other._error;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current result.</param>
        /// <returns>true if the specified object is equal to the current result; otherwise, false.</returns>
        public override bool Equals(object? obj) =>
            obj is Result<T> other && Equals(other);

        /// <summary>
        /// Returns the hash code for this <see cref="Result{T}"/>.
        /// </summary>
        /// <returns>A hash code for the current result.</returns>
        public override int GetHashCode()
        {
            if (!IsSuccess)
                return HashCode.Combine(IsSuccess, _error);

            return HashCode.Combine(IsSuccess, _value);
        }

        /// <summary>
        /// Determines whether two <see cref="Result{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns>true if the results are equal; otherwise, false.</returns>
        public static bool operator ==(Result<T> left, Result<T> right) =>
            left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Result{T}"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns>true if the results are not equal; otherwise, false.</returns>
        public static bool operator !=(Result<T> left, Result<T> right) =>
            !left.Equals(right);

        /// <summary>
        /// Enables LINQ query syntax support (alias for Map).
        /// </summary>
        /// <typeparam name="TResult">The type of the result value.</typeparam>
        /// <param name="selector">The function to transform the success value.</param>
        /// <returns>A new result with the transformed value, or the original failure.</returns>
        public Result<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            return Map(selector);
        }

        /// <summary>
        /// Enables LINQ query syntax support for chaining operations.
        /// </summary>
        /// <typeparam name="TIntermediate">The type of the intermediate result.</typeparam>
        /// <typeparam name="TResult">The type of the final result.</typeparam>
        /// <param name="selector">The function that returns an intermediate result.</param>
        /// <param name="projector">The function that combines the original and intermediate values.</param>
        /// <returns>A new result with the projected value, or the first failure encountered.</returns>
        public Result<TResult> SelectMany<TIntermediate, TResult>(
            Func<T, Result<TIntermediate>> selector,
            Func<T, TIntermediate, TResult> projector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(projector);
            return Bind(t => selector(t).Map(i => projector(t, i)));
        }

        /// <summary>
        /// Alias for Success (common convention in functional programming).
        /// </summary>
        /// <param name="value">The success value.</param>
        /// <returns>A successful result containing the value.</returns>
        public static Result<T> Ok(T value) => Success(value);

        /// <summary>
        /// Recovers from failure by converting the error to a success value.
        /// </summary>
        /// <param name="recovery">The function to convert the error to a value.</param>
        /// <returns>This result if successful, or a new success result from the recovery function.</returns>
        public Result<T> Recover(Func<string, T> recovery)
        {
            ArgumentNullException.ThrowIfNull(recovery);
            return IsSuccess ? this : Success(recovery(_error!));
        }

        /// <summary>
        /// Asynchronously recovers from failure.
        /// </summary>
        /// <param name="recovery">The async function to convert the error to a value.</param>
        /// <returns>This result if successful, or a new success result from the recovery function.</returns>
        public async Task<Result<T>> RecoverAsync(Func<string, Task<T>> recovery)
        {
            ArgumentNullException.ThrowIfNull(recovery);
            return IsSuccess ? this : Success(await recovery(_error!).ConfigureAwait(false));
        }

        /// <summary>
        /// Flattens a nested Result.
        /// </summary>
        /// <param name="nested">The nested result.</param>
        /// <returns>The inner result.</returns>
        public static Result<T> Flatten(Result<Result<T>> nested)
        {
            return nested.IsSuccess ? nested.Value : Failure(nested.Error);
        }

        /// <summary>
        /// Combines two results into a tuple.
        /// </summary>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <param name="first">The first result.</param>
        /// <param name="second">The second result.</param>
        /// <returns>A result containing a tuple of both values, or the first failure.</returns>
        public static Result<(T1, T2)> Zip<T1, T2>(
            Result<T1> first,
            Result<T2> second)
        {
            if (first.IsFailure) return Result<(T1, T2)>.Failure(first.Error);
            if (second.IsFailure) return Result<(T1, T2)>.Failure(second.Error);
            return Result<(T1, T2)>.Success((first.Value, second.Value));
        }

        /// <summary>
        /// Combines three results into a tuple.
        /// </summary>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <param name="first">The first result.</param>
        /// <param name="second">The second result.</param>
        /// <param name="third">The third result.</param>
        /// <returns>A result containing a tuple of all values, or the first failure.</returns>
        public static Result<(T1, T2, T3)> Zip<T1, T2, T3>(
            Result<T1> first,
            Result<T2> second,
            Result<T3> third)
        {
            if (first.IsFailure) return Result<(T1, T2, T3)>.Failure(first.Error);
            if (second.IsFailure) return Result<(T1, T2, T3)>.Failure(second.Error);
            if (third.IsFailure) return Result<(T1, T2, T3)>.Failure(third.Error);
            return Result<(T1, T2, T3)>.Success((first.Value, second.Value, third.Value));
        }

        /// <summary>
        /// Converts failure to success with a default value.
        /// </summary>
        /// <param name="defaultValue">The default value to use if this is a failure.</param>
        /// <returns>This result if successful, or a new success result with the default value.</returns>
        public Result<T> OrElse(T defaultValue)
        {
            return IsSuccess ? this : Success(defaultValue);
        }

        /// <summary>
        /// Converts failure to success using a factory function.
        /// </summary>
        /// <param name="factory">The function to create a value from the error.</param>
        /// <returns>This result if successful, or a new success result from the factory.</returns>
        public Result<T> OrElse(Func<string, T> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            return IsSuccess ? this : Success(factory(_error!));
        }
    }

    /// <summary>
    /// Provides extension methods for creating <see cref="Result{T}"/> instances.
    /// </summary>
    public static class ResultEx
    {
        /// <summary>
        /// Executes the function and wraps the result, catching any exceptions as failures.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <param name="func">The function to execute.</param>
        /// <returns>A successful result with the function's return value, or a failure with the exception message.</returns>
        public static Result<T> TryExecute<T>(Func<T> func)
        {
            try
            {
                return Result<T>.Success(func());
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Executes the async function and wraps the result, catching any exceptions as failures.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <param name="func">The async function to execute.</param>
        /// <returns>A successful result with the function's return value, or a failure with the exception message.</returns>
        public static async Task<Result<T>> TryExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return Result<T>.Success(await func().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Executes the action and wraps the outcome, catching any exceptions as failures.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>A successful result if the action completes, or a failure with the exception message.</returns>
        public static Result TryExecute(Action action)
        {
            try
            {
                action();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Executes the async action and wraps the outcome, catching any exceptions as failures.
        /// </summary>
        /// <param name="func">The async action to execute.</param>
        /// <returns>A successful result if the action completes, or a failure with the exception message.</returns>
        public static async Task<Result> TryExecuteAsync(Func<Task> func)
        {
            try
            {
                await func().ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Combines two results into a tuple. If either result is a failure, the first error is returned.
        /// </summary>
        /// <typeparam name="T1">The type of the first result's value.</typeparam>
        /// <typeparam name="T2">The type of the second result's value.</typeparam>
        /// <param name="first">The first result.</param>
        /// <param name="second">The second result.</param>
        /// <returns>A successful result containing a tuple of both values, or the first failure encountered.</returns>
        public static Result<(T1, T2)> Combine<T1, T2>(Result<T1> first, Result<T2> second)
        {
            if (first.IsFailure) return Result<(T1, T2)>.Failure(first.Error);
            if (second.IsFailure) return Result<(T1, T2)>.Failure(second.Error);
            return Result<(T1, T2)>.Success((first.Value, second.Value));
        }

        /// <summary>
        /// Combines a collection of results into a single result containing all values.
        /// If any result is a failure, the first error is returned.
        /// </summary>
        /// <typeparam name="T">The type of the success values.</typeparam>
        /// <param name="results">The collection of results to combine.</param>
        /// <returns>A successful result with all values, or the first failure encountered.</returns>
        public static Result<IReadOnlyList<T>> Combine<T>(IEnumerable<Result<T>> results)
        {
            var values = new List<T>();

            foreach (Result<T> result in results)
            {
                if (result.IsFailure)
                    return Result<IReadOnlyList<T>>.Failure(result.Error);

                values.Add(result.Value);
            }

            return Result<IReadOnlyList<T>>.Success(values);
        }

        extension<T>(T value)
        {
            /// <summary>
            /// Wraps the value in a successful <see cref="Result{T}"/>.
            /// </summary>
            /// <returns>A successful <see cref="Result{T}"/> containing the value.</returns>
            public Result<T> ToResult() => Result<T>.Success(value);
        }

        extension<T>(Task<Result<T>> task)
        {
            /// <summary>
            /// Transforms the success value of an async result using the specified function.
            /// </summary>
            /// <typeparam name="TNew">The type of the new success value.</typeparam>
            /// <param name="mapper">The function to transform the success value.</param>
            /// <returns>A new result with the transformed value, or the original failure.</returns>
            public async Task<Result<TNew>> MapAsync<TNew>(Func<T, TNew> mapper) =>
                (await task.ConfigureAwait(false)).Map(mapper);

            /// <summary>
            /// Asynchronously transforms the success value of an async result.
            /// </summary>
            /// <typeparam name="TNew">The type of the new success value.</typeparam>
            /// <param name="mapper">The async function to transform the success value.</param>
            /// <returns>A new result with the transformed value, or the original failure.</returns>
            public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper) =>
                await (await task.ConfigureAwait(false)).MapAsync(mapper).ConfigureAwait(false);

            /// <summary>
            /// Chains the async result with another operation that returns a Result.
            /// </summary>
            /// <typeparam name="TNew">The type of the new success value.</typeparam>
            /// <param name="binder">The function that returns a new Result.</param>
            /// <returns>The result of the binder, or the original failure.</returns>
            public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Result<TNew>> binder) =>
                (await task.ConfigureAwait(false)).Bind(binder);

            /// <summary>
            /// Asynchronously chains the async result with another async operation that returns a Result.
            /// </summary>
            /// <typeparam name="TNew">The type of the new success value.</typeparam>
            /// <param name="binder">The async function that returns a new Result.</param>
            /// <returns>The result of the binder, or the original failure.</returns>
            public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> binder) =>
                await (await task.ConfigureAwait(false)).BindAsync(binder).ConfigureAwait(false);

            /// <summary>
            /// Executes a side-effect action on the success value of an async result without changing the result.
            /// </summary>
            /// <param name="action">The action to execute on the success value.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result<T>> TapAsync(Action<T> action) =>
                (await task.ConfigureAwait(false)).Tap(action);

            /// <summary>
            /// Asynchronously executes a side-effect action on the success value of an async result.
            /// </summary>
            /// <param name="action">The async action to execute on the success value.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result<T>> TapAsync(Func<T, Task> action) =>
                await (await task.ConfigureAwait(false)).TapAsync(action).ConfigureAwait(false);

            /// <summary>
            /// Validates the success value of an async result against a predicate.
            /// </summary>
            /// <param name="predicate">The validation predicate.</param>
            /// <param name="error">The error message if the predicate fails.</param>
            /// <returns>The original success if the predicate passes, or a failure result.</returns>
            public async Task<Result<T>> EnsureAsync(Func<T, bool> predicate, string error) =>
                (await task.ConfigureAwait(false)).Ensure(predicate, error);

            /// <summary>
            /// Transforms the error message of an async result.
            /// </summary>
            /// <param name="mapper">The function to transform the error message.</param>
            /// <returns>A new result with the transformed error, or the original success.</returns>
            public async Task<Result<T>> MapErrorAsync(Func<string, string> mapper) =>
                (await task.ConfigureAwait(false)).MapError(mapper);

            /// <summary>
            /// Pattern-matches on the async result.
            /// </summary>
            /// <typeparam name="TOut">The return type of the match.</typeparam>
            /// <param name="onSuccess">The function to apply if the result is successful.</param>
            /// <param name="onFailure">The function to apply if the result is a failure.</param>
            /// <returns>The result of applying the appropriate function.</returns>
            public async Task<TOut> MatchAsync<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure) =>
                (await task.ConfigureAwait(false)).Match(onSuccess, onFailure);

            /// <summary>
            /// Executes a side-effect action on the error of an async result without changing the result.
            /// </summary>
            /// <param name="action">The action to execute on the error message.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result<T>> TapErrorAsync(Action<string> action) =>
                (await task.ConfigureAwait(false)).TapError(action);

            /// <summary>
            /// Asynchronously executes a side-effect action on the error of an async result.
            /// </summary>
            /// <param name="action">The async action to execute on the error message.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result<T>> TapErrorAsync(Func<string, Task> action) =>
                await (await task.ConfigureAwait(false)).TapErrorAsync(action).ConfigureAwait(false);
        }

        extension(Task<Result> task)
        {
            /// <summary>
            /// Executes a side-effect action on the success of an async result without changing the result.
            /// </summary>
            /// <param name="action">The action to execute on success.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result> TapAsync(Action action) =>
                (await task.ConfigureAwait(false)).Tap(action);

            /// <summary>
            /// Asynchronously executes a side-effect action on the success of an async result.
            /// </summary>
            /// <param name="action">The async action to execute on success.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result> TapAsync(Func<Task> action) =>
                await (await task.ConfigureAwait(false)).TapAsync(action).ConfigureAwait(false);

            /// <summary>
            /// Executes a side-effect action on the error of an async result without changing the result.
            /// </summary>
            /// <param name="action">The action to execute on the error message.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result> TapErrorAsync(Action<string> action) =>
                (await task.ConfigureAwait(false)).TapError(action);

            /// <summary>
            /// Asynchronously executes a side-effect action on the error of an async result.
            /// </summary>
            /// <param name="action">The async action to execute on the error message.</param>
            /// <returns>The original result, unchanged.</returns>
            public async Task<Result> TapErrorAsync(Func<string, Task> action) =>
                await (await task.ConfigureAwait(false)).TapErrorAsync(action).ConfigureAwait(false);

            /// <summary>
            /// Validates a condition on an async result.
            /// </summary>
            /// <param name="predicate">The validation predicate.</param>
            /// <param name="error">The error message if the predicate fails.</param>
            /// <returns>The original success if the predicate passes, or a failure result.</returns>
            public async Task<Result> EnsureAsync(Func<bool> predicate, string error) =>
                (await task.ConfigureAwait(false)).Ensure(predicate, error);

            /// <summary>
            /// Transforms the error message of an async result.
            /// </summary>
            /// <param name="mapper">The function to transform the error message.</param>
            /// <returns>A new result with the transformed error, or the original success.</returns>
            public async Task<Result> MapErrorAsync(Func<string, string> mapper) =>
                (await task.ConfigureAwait(false)).MapError(mapper);

            /// <summary>
            /// Pattern-matches on the async result.
            /// </summary>
            /// <typeparam name="TOut">The return type of the match.</typeparam>
            /// <param name="onSuccess">The function to apply if the result is successful.</param>
            /// <param name="onFailure">The function to apply if the result is a failure.</param>
            /// <returns>The result of applying the appropriate function.</returns>
            public async Task<TOut> MatchAsync<TOut>(Func<TOut> onSuccess, Func<string, TOut> onFailure) =>
                (await task.ConfigureAwait(false)).Match(onSuccess, onFailure);

            /// <summary>
            /// Chains the async result with an operation that returns a Result&lt;T&gt;.
            /// </summary>
            /// <typeparam name="T">The type of the new success value.</typeparam>
            /// <param name="binder">The function that returns a new Result.</param>
            /// <returns>The result of the binder, or the original failure.</returns>
            public async Task<Result<T>> BindAsync<T>(Func<Result<T>> binder) =>
                (await task.ConfigureAwait(false)).Bind(binder);

            /// <summary>
            /// Asynchronously chains the async result with an async operation that returns a Result&lt;T&gt;.
            /// </summary>
            /// <typeparam name="T">The type of the new success value.</typeparam>
            /// <param name="binder">The async function that returns a new Result.</param>
            /// <returns>The result of the binder, or the original failure.</returns>
            public async Task<Result<T>> BindAsync<T>(Func<Task<Result<T>>> binder) =>
                await (await task.ConfigureAwait(false)).BindAsync(binder).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Represents the outcome of a void operation that can either succeed or fail with an error message.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{IsSuccess ? \"Success\" : \"Failure(\" + _error + \")\"}}")]
    public readonly struct Result : IEquatable<Result>
    {
        private readonly string? _error;

        private Result(bool _)
        {
            _error = null;
            IsSuccess = true;
        }

        private Result(string error)
        {
            _error = error;
            IsSuccess = false;
        }

        /// <summary>Gets a value indicating whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>Gets a value indicating whether the operation failed.</summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error message. Throws if the result is a success.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result is a success.</exception>
        public string Error => IsFailure
            ? _error ?? "Result was created using default constructor"
            : throw new InvalidOperationException("Cannot access Error on a successful result.");

        /// <summary>Creates a successful result.</summary>
        /// <returns>A successful <see cref="Result"/>.</returns>
        public static Result Success() => new(true);

        /// <summary>Creates a failed result with the specified error message.</summary>
        /// <param name="error">The error message.</param>
        /// <returns>A failed <see cref="Result"/>.</returns>
        public static Result Failure(string error) => new(error);

        /// <summary>
        /// Executes a side-effect action on success without changing the result.
        /// </summary>
        /// <param name="action">The action to execute on success.</param>
        /// <returns>The original result, unchanged.</returns>
        public Result Tap(Action action)
        {
            if (IsSuccess) action();
            return this;
        }

        /// <summary>
        /// Executes a side-effect action on the error message without changing the result.
        /// </summary>
        /// <param name="action">The action to execute on the error message.</param>
        /// <returns>The original result, unchanged.</returns>
        public Result TapError(Action<string> action)
        {
            if (IsFailure) action(_error!);
            return this;
        }

        /// <summary>
        /// Transforms the error message using the specified function.
        /// </summary>
        /// <param name="mapper">The function to transform the error message.</param>
        /// <returns>A new result with the transformed error, or the original success.</returns>
        public Result MapError(Func<string, string> mapper) =>
            IsFailure ? Failure(mapper(_error!)) : this;

        /// <summary>
        /// Validates a condition. If the predicate returns <c>false</c>,
        /// the result is converted to a failure with the specified error message.
        /// </summary>
        /// <param name="predicate">The validation predicate.</param>
        /// <param name="error">The error message if the predicate fails.</param>
        /// <returns>The original success if the predicate passes, or a failure result.</returns>
        public Result Ensure(Func<bool> predicate, string error) =>
            IsSuccess && !predicate() ? Failure(error) : this;

        /// <summary>
        /// Pattern-matches on the result, applying <paramref name="onSuccess"/> if the result is successful,
        /// or <paramref name="onFailure"/> if the result is a failure.
        /// </summary>
        /// <typeparam name="TOut">The return type of the match.</typeparam>
        /// <param name="onSuccess">The function to apply if the result is successful.</param>
        /// <param name="onFailure">The function to apply if the result is a failure.</param>
        /// <returns>The result of applying the appropriate function.</returns>
        public TOut Match<TOut>(Func<TOut> onSuccess, Func<string, TOut> onFailure) =>
            IsSuccess ? onSuccess() : onFailure(_error!);

        /// <summary>
        /// Chains this result with an operation that returns another <see cref="Result"/>.
        /// If this result is a failure, the binder is not executed and the failure propagates.
        /// </summary>
        /// <param name="binder">The function that returns a new Result.</param>
        /// <returns>The result of the binder, or the original failure.</returns>
        public Result Bind(Func<Result> binder) =>
            IsSuccess ? binder() : this;

        /// <summary>
        /// Chains this result with an operation that returns a <see cref="Result{T}"/>.
        /// If this result is a failure, the binder is not executed and the failure propagates.
        /// </summary>
        /// <typeparam name="T">The type of the new success value.</typeparam>
        /// <param name="binder">The function that returns a new Result.</param>
        /// <returns>The result of the binder, or the original failure.</returns>
        public Result<T> Bind<T>(Func<Result<T>> binder) =>
            IsSuccess ? binder() : Result<T>.Failure(_error!);

        /// <summary>
        /// Asynchronously chains this result with an operation that returns another <see cref="Result"/>.
        /// </summary>
        /// <param name="binder">The async function that returns a new Result.</param>
        /// <returns>The result of the binder, or the original failure.</returns>
        public async Task<Result> BindAsync(Func<Task<Result>> binder) =>
            IsSuccess ? await binder().ConfigureAwait(false) : this;

        /// <summary>
        /// Asynchronously executes a side-effect action on success without changing the result.
        /// </summary>
        /// <param name="action">The async action to execute on success.</param>
        /// <returns>The original result, unchanged.</returns>
        public async Task<Result> TapAsync(Func<Task> action)
        {
            if (IsSuccess) await action().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Asynchronously executes a side-effect action on the error message without changing the result.
        /// </summary>
        /// <param name="action">The async action to execute on the error message.</param>
        /// <returns>The original result, unchanged.</returns>
        public async Task<Result> TapErrorAsync(Func<string, Task> action)
        {
            if (IsFailure) await action(_error!).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Asynchronously chains this result with an operation that returns a <see cref="Result{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the new success value.</typeparam>
        /// <param name="binder">The async function that returns a new Result.</param>
        /// <returns>The result of the binder, or the original failure.</returns>
        public async Task<Result<T>> BindAsync<T>(Func<Task<Result<T>>> binder) =>
            IsSuccess ? await binder().ConfigureAwait(false) : Result<T>.Failure(_error!);

        /// <summary>
        /// Returns a string representation of this result.
        /// </summary>
        /// <returns>A string in the form "Success" or "Failure(error)".</returns>
        public override string ToString() =>
            IsSuccess ? "Success" : $"Failure({_error})";

        /// <summary>
        /// Determines whether the specified <see cref="Result"/> is equal to the current <see cref="Result"/>.
        /// </summary>
        /// <param name="other">The result to compare with the current result.</param>
        /// <returns>true if the specified result is equal to the current result; otherwise, false.</returns>
        public bool Equals(Result other)
        {
            if (IsSuccess != other.IsSuccess)
                return false;

            return IsSuccess || _error == other._error;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Result"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current result.</param>
        /// <returns>true if the specified object is equal to the current result; otherwise, false.</returns>
        public override bool Equals(object? obj) =>
            obj is Result other && Equals(other);

        /// <summary>
        /// Returns the hash code for this <see cref="Result"/>.
        /// </summary>
        /// <returns>A hash code for the current result.</returns>
        public override int GetHashCode() =>
            IsSuccess ? HashCode.Combine(IsSuccess) : HashCode.Combine(IsSuccess, _error);

        /// <summary>
        /// Determines whether two <see cref="Result"/> instances are equal.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns>true if the results are equal; otherwise, false.</returns>
        public static bool operator ==(Result left, Result right) =>
            left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Result"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns>true if the results are not equal; otherwise, false.</returns>
        public static bool operator !=(Result left, Result right) =>
            !left.Equals(right);
    }
}
