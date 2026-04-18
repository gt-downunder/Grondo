namespace Grondo
{
    /// <summary>
    /// Represents an optional value that may or may not exist.
    /// Use <see cref="Maybe{T}.Some(T)"/> to wrap a value or <see cref="Maybe{T}.None"/> for absence.
    /// </summary>
    /// <typeparam name="T">The type of the contained value.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{HasValue ? \"Some(\" + _value + \")\" : \"None\"}")]
    public readonly struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly T? _value;

        private Maybe(T value)
        {
            _value = value;
            HasValue = true;
        }

        /// <summary>Gets a value indicating whether this instance contains a value.</summary>
        public bool HasValue { get; }

        /// <summary>Gets a value indicating whether this instance has no value.</summary>
        public bool HasNoValue => !HasValue;

        /// <summary>Gets the contained value.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the instance has no value.</exception>
        public T Value => HasValue
            ? _value!
            : throw new InvalidOperationException("Maybe has no value.");

        /// <summary>Creates a <see cref="Maybe{T}"/> with a value.</summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>A Maybe containing the value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public static Maybe<T> Some(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Maybe<T>(value);
        }

        /// <summary>Gets a <see cref="Maybe{T}"/> with no value.</summary>
        public static Maybe<T> None => default;

        /// <summary>
        /// Transforms the contained value using the specified mapping function.
        /// Returns <see cref="None"/> if this instance has no value.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The function to transform the contained value.</param>
        /// <returns>A <see cref="Maybe{TResult}"/> with the transformed value, or <see cref="Maybe{TResult}.None"/>.</returns>
        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return HasValue ? Maybe<TResult>.Some(mapper(_value!)) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects the contained value into a new <see cref="Maybe{TResult}"/> using the specified function.
        /// Returns <see cref="None"/> if this instance has no value.
        /// </summary>
        /// <typeparam name="TResult">The type of the new Maybe's value.</typeparam>
        /// <param name="binder">The function that returns a new Maybe.</param>
        /// <returns>The result of the binder, or <see cref="Maybe{TResult}.None"/>.</returns>
        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return HasValue ? binder(_value!) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Applies the appropriate function depending on whether a value is present.
        /// </summary>
        /// <typeparam name="TResult">The return type of the match.</typeparam>
        /// <param name="some">The function to apply if a value is present.</param>
        /// <param name="none">The function to apply if no value is present.</param>
        /// <returns>The result of applying the appropriate function.</returns>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            ArgumentNullException.ThrowIfNull(some);
            ArgumentNullException.ThrowIfNull(none);
            return HasValue ? some(_value!) : none();
        }

        /// <summary>
        /// Returns the contained value or the specified default if no value is present.
        /// </summary>
        /// <param name="defaultValue">The fallback value to return if no value is present.</param>
        /// <returns>The contained value, or <paramref name="defaultValue"/> if no value is present.</returns>
        public T GetValueOrDefault(T defaultValue) =>
            HasValue ? _value! : defaultValue;

        /// <summary>
        /// Returns the contained value if the predicate is satisfied; otherwise returns <see cref="None"/>.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate against the contained value.</param>
        /// <returns>This instance if the predicate passes, or <see cref="None"/>.</returns>
        public Maybe<T> Where(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return HasValue && predicate(_value!) ? this : None;
        }

        /// <summary>
        /// Executes an action on the contained value if present. Returns this instance unchanged.
        /// </summary>
        /// <param name="action">The action to execute on the contained value.</param>
        /// <returns>This instance, unchanged.</returns>
        public Maybe<T> Tap(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (HasValue) action(_value!);
            return this;
        }

        /// <summary>
        /// Asynchronously transforms the contained value using the specified function.
        /// Returns <see cref="None"/> if this instance has no value.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The async function to transform the contained value.</param>
        /// <returns>A <see cref="Maybe{TResult}"/> with the transformed value, or <see cref="Maybe{TResult}.None"/>.</returns>
        public async Task<Maybe<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return HasValue ? Maybe<TResult>.Some(await mapper(_value!).ConfigureAwait(false)) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Asynchronously projects the contained value into a new <see cref="Maybe{TResult}"/> using the specified function.
        /// Returns <see cref="None"/> if this instance has no value.
        /// </summary>
        /// <typeparam name="TResult">The type of the new Maybe's value.</typeparam>
        /// <param name="binder">The async function that returns a new Maybe.</param>
        /// <returns>The result of the binder, or <see cref="Maybe{TResult}.None"/>.</returns>
        public async Task<Maybe<TResult>> BindAsync<TResult>(Func<T, Task<Maybe<TResult>>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return HasValue ? await binder(_value!).ConfigureAwait(false) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Asynchronously executes an action on the contained value if present. Returns this instance unchanged.
        /// </summary>
        /// <param name="action">The async action to execute on the contained value.</param>
        /// <returns>This instance, unchanged.</returns>
        public async Task<Maybe<T>> TapAsync(Func<T, Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (HasValue) await action(_value!).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Converts this <see cref="Maybe{T}"/> to a <see cref="Result{T}"/>.
        /// Returns <see cref="Result{T}.Success(T)"/> if a value is present;
        /// otherwise returns <see cref="Result{T}.Failure(string)"/> with the specified error.
        /// </summary>
        /// <param name="errorMessage">The error message to use when no value is present.</param>
        /// <returns>A successful result with the value, or a failure with the error message.</returns>
        public Result<T> ToResult(string errorMessage) =>
            HasValue ? Result<T>.Success(_value!) : Result<T>.Failure(errorMessage);

        /// <summary>
        /// Converts this <see cref="Maybe{T}"/> to an <see cref="Either{TLeft, T}"/>.
        /// Some becomes Right; None becomes Left with the specified value.
        /// </summary>
        /// <typeparam name="TLeft">The type of the Left value.</typeparam>
        /// <param name="leftValue">The Left value to use when no value is present.</param>
        /// <returns>An Either with Right(value) if Some, or Left(leftValue) if None.</returns>
        public Either<TLeft, T> ToEither<TLeft>(TLeft leftValue)
        {
            ArgumentNullException.ThrowIfNull(leftValue);
            return HasValue
                ? Either<TLeft, T>.FromRight(_value!)
                : Either<TLeft, T>.FromLeft(leftValue);
        }

        /// <summary>
        /// Converts this <see cref="Maybe{T}"/> to a <see cref="Validation{T}"/>.
        /// Some becomes Valid; None becomes Invalid with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message to use when no value is present.</param>
        /// <returns>A valid Validation with the value, or an invalid Validation with the error.</returns>
        public Validation<T> ToValidation(string errorMessage) =>
            HasValue ? Validation<T>.Valid(_value!) : Validation<T>.Invalid(errorMessage);

        /// <summary>Returns a string representation of this instance.</summary>
        /// <returns>A string in the form "Some(value)" or "None".</returns>
        public override string ToString() =>
            HasValue ? $"Some({_value})" : "None";

        /// <summary>
        /// Determines whether the specified <see cref="Maybe{T}"/> is equal to the current <see cref="Maybe{T}"/>.
        /// </summary>
        /// <param name="other">The maybe to compare with the current maybe.</param>
        /// <returns>true if the specified maybe is equal to the current maybe; otherwise, false.</returns>
        public bool Equals(Maybe<T> other)
        {
            if (HasValue != other.HasValue)
                return false;

            return !HasValue || EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Maybe{T}"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current maybe.</param>
        /// <returns>true if the specified object is equal to the current maybe; otherwise, false.</returns>
        public override bool Equals(object? obj) =>
            obj is Maybe<T> other && Equals(other);

        /// <summary>
        /// Returns the hash code for this <see cref="Maybe{T}"/>.
        /// </summary>
        /// <returns>A hash code for the current maybe.</returns>
        public override int GetHashCode() =>
            HasValue ? HashCode.Combine(HasValue, _value) : HashCode.Combine(HasValue);

        /// <summary>
        /// Determines whether two <see cref="Maybe{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">The first maybe to compare.</param>
        /// <param name="right">The second maybe to compare.</param>
        /// <returns>true if the maybes are equal; otherwise, false.</returns>
        public static bool operator ==(Maybe<T> left, Maybe<T> right) =>
            left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Maybe{T}"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first maybe to compare.</param>
        /// <param name="right">The second maybe to compare.</param>
        /// <returns>true if the maybes are not equal; otherwise, false.</returns>
        public static bool operator !=(Maybe<T> left, Maybe<T> right) =>
            !left.Equals(right);

        /// <summary>
        /// Enables LINQ query syntax support (alias for Map).
        /// </summary>
        /// <typeparam name="TResult">The type of the result value.</typeparam>
        /// <param name="selector">The function to transform the contained value.</param>
        /// <returns>A new maybe with the transformed value, or None.</returns>
        public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            return Map(selector);
        }

        /// <summary>
        /// Enables LINQ query syntax support for chaining operations.
        /// </summary>
        /// <typeparam name="TIntermediate">The type of the intermediate maybe.</typeparam>
        /// <typeparam name="TResult">The type of the final result.</typeparam>
        /// <param name="selector">The function that returns an intermediate maybe.</param>
        /// <param name="projector">The function that combines the original and intermediate values.</param>
        /// <returns>A new maybe with the projected value, or None if any step is None.</returns>
        public Maybe<TResult> SelectMany<TIntermediate, TResult>(
            Func<T, Maybe<TIntermediate>> selector,
            Func<T, TIntermediate, TResult> projector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(projector);
            return Bind(t => selector(t).Map(i => projector(t, i)));
        }

        /// <summary>
        /// Returns an alternative Maybe if this is None.
        /// </summary>
        /// <param name="alternative">The alternative maybe to return if this is None.</param>
        /// <returns>This maybe if it has a value, otherwise the alternative.</returns>
        public Maybe<T> OrElse(Maybe<T> alternative)
        {
            return HasValue ? this : alternative;
        }

        /// <summary>
        /// Returns an alternative Maybe from a factory if this is None.
        /// </summary>
        /// <param name="alternativeFactory">The function to create an alternative maybe.</param>
        /// <returns>This maybe if it has a value, otherwise the result of the factory.</returns>
        public Maybe<T> OrElse(Func<Maybe<T>> alternativeFactory)
        {
            ArgumentNullException.ThrowIfNull(alternativeFactory);
            return HasValue ? this : alternativeFactory();
        }

        /// <summary>
        /// Alias for Where (common in functional programming).
        /// </summary>
        /// <param name="predicate">The predicate to test the value.</param>
        /// <returns>This maybe if it has a value and satisfies the predicate, otherwise None.</returns>
        public Maybe<T> Filter(Func<T, bool> predicate) => Where(predicate);

        /// <summary>
        /// Flattens a nested Maybe.
        /// </summary>
        /// <param name="nested">The nested maybe.</param>
        /// <returns>The inner maybe.</returns>
        public static Maybe<T> Flatten(Maybe<Maybe<T>> nested)
        {
            return nested.HasValue ? nested.Value : None;
        }

        /// <summary>
        /// Combines two Maybes into a tuple.
        /// </summary>
        /// <typeparam name="T1">The type of the first maybe.</typeparam>
        /// <typeparam name="T2">The type of the second maybe.</typeparam>
        /// <param name="first">The first maybe.</param>
        /// <param name="second">The second maybe.</param>
        /// <returns>A maybe containing a tuple of both values, or None if either is None.</returns>
        public static Maybe<(T1, T2)> Zip<T1, T2>(
            Maybe<T1> first,
            Maybe<T2> second)
        {
            return first.HasValue && second.HasValue
                ? Maybe<(T1, T2)>.Some((first.Value, second.Value))
                : Maybe<(T1, T2)>.None;
        }

        /// <summary>
        /// Converts None to Some with a default value.
        /// </summary>
        /// <param name="defaultValue">The default value to use if this is None.</param>
        /// <returns>This maybe if it has a value, otherwise a Some with the default value.</returns>
        public Maybe<T> OrValue(T defaultValue)
        {
            return HasValue ? this : Some(defaultValue);
        }

        /// <summary>
        /// Converts None to Some using a factory function.
        /// </summary>
        /// <param name="factory">The function to create a value.</param>
        /// <returns>This maybe if it has a value, otherwise a Some with the factory result.</returns>
        public Maybe<T> OrValue(Func<T> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            return HasValue ? this : Some(factory());
        }

        /// <summary>
        /// Implicitly converts a value to a <see cref="Maybe{T}"/> containing that value.
        /// Returns <see cref="None"/> if the value is null.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator Maybe<T>(T? value) =>
            value is null ? None : new Maybe<T>(value);

        /// <summary>
        /// Returns the contained value as a single-element sequence, or an empty sequence if None.
        /// Useful for integrating with LINQ pipelines over collections.
        /// </summary>
        /// <returns>An enumerable with one element if Some; otherwise an empty enumerable.</returns>
        public IEnumerable<T> AsEnumerable()
        {
            if (HasValue) yield return _value!;
        }

        /// <summary>Returns the contained value as a single-element array, or an empty array if None.</summary>
        public T[] ToArray() => HasValue ? [_value!] : [];

        /// <summary>Returns the contained value as a single-element list, or an empty list if None.</summary>
        public List<T> ToList() => HasValue ? [_value!] : [];
    }
}

