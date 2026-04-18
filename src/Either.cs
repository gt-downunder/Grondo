using System.Diagnostics;

namespace Grondo
{
    /// <summary>
    /// Represents a value that can be one of two types: Left or Right.
    /// By convention, Right represents success and Left represents failure/error.
    /// </summary>
    /// <typeparam name="TLeft">The type of the Left value (typically error).</typeparam>
    /// <typeparam name="TRight">The type of the Right value (typically success).</typeparam>
    [DebuggerDisplay("{IsLeft ? \"Left(\" + _left + \")\" : \"Right(\" + _right + \")\"}")]
    public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    {
        private readonly TLeft? _left;
        private readonly TRight? _right;

        private Either(TLeft left)
        {
            _left = left;
            _right = default;
            IsLeft = true;
        }

        private Either(TRight right)
        {
            _left = default;
            _right = right;
            IsLeft = false;
        }

        /// <summary>Gets a value indicating whether this is a Left value.</summary>
        public bool IsLeft { get; }

        /// <summary>Gets a value indicating whether this is a Right value.</summary>
        public bool IsRight => !IsLeft;

        /// <summary>
        /// Gets the Left value. Throws if this is a Right.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this is a Right value.</exception>
        public TLeft Left => IsLeft
            ? _left!
            : throw new InvalidOperationException("Cannot access Left on a Right value.");

        /// <summary>
        /// Gets the Right value. Throws if this is a Left.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this is a Left value.</exception>
        public TRight Right => IsRight
            ? _right!
            : throw new InvalidOperationException("Cannot access Right on a Left value.");

        /// <summary>
        /// Creates an Either with a Left value.
        /// </summary>
        /// <param name="value">The left value.</param>
        /// <returns>An Either containing the left value.</returns>
        public static Either<TLeft, TRight> FromLeft(TLeft value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Either<TLeft, TRight>(value);
        }

        /// <summary>
        /// Creates an Either with a Right value.
        /// </summary>
        /// <param name="value">The right value.</param>
        /// <returns>An Either containing the right value.</returns>
        public static Either<TLeft, TRight> FromRight(TRight value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Either<TLeft, TRight>(value);
        }

        /// <summary>
        /// Pattern matches on the Either value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="onLeft">The function to execute if this is a Left.</param>
        /// <param name="onRight">The function to execute if this is a Right.</param>
        /// <returns>The result of the matching function.</returns>
        public TResult Match<TResult>(
            Func<TLeft, TResult> onLeft,
            Func<TRight, TResult> onRight)
        {
            ArgumentNullException.ThrowIfNull(onLeft);
            ArgumentNullException.ThrowIfNull(onRight);
            return IsLeft ? onLeft(_left!) : onRight(_right!);
        }

        /// <summary>
        /// Transforms the Right value if present.
        /// </summary>
        /// <typeparam name="TNewRight">The type of the new right value.</typeparam>
        /// <param name="mapper">The function to transform the right value.</param>
        /// <returns>An Either with the transformed right value, or the original left value.</returns>
        public Either<TLeft, TNewRight> Map<TNewRight>(Func<TRight, TNewRight> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsRight
                ? Either<TLeft, TNewRight>.FromRight(mapper(_right!))
                : Either<TLeft, TNewRight>.FromLeft(_left!);
        }

        /// <summary>
        /// Chains operations on the Right value.
        /// </summary>
        /// <typeparam name="TNewRight">The type of the new right value.</typeparam>
        /// <param name="binder">The function that returns a new Either.</param>
        /// <returns>The result of the binder, or the original left value.</returns>
        public Either<TLeft, TNewRight> Bind<TNewRight>(
            Func<TRight, Either<TLeft, TNewRight>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return IsRight ? binder(_right!) : Either<TLeft, TNewRight>.FromLeft(_left!);
        }

        /// <summary>
        /// Transforms the Left value if present.
        /// </summary>
        /// <typeparam name="TNewLeft">The type of the new left value.</typeparam>
        /// <param name="mapper">The function to transform the left value.</param>
        /// <returns>An Either with the transformed left value, or the original right value.</returns>
        public Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsLeft
                ? Either<TNewLeft, TRight>.FromLeft(mapper(_left!))
                : Either<TNewLeft, TRight>.FromRight(_right!);
        }

        /// <summary>
        /// Executes a side-effect on the Right value.
        /// </summary>
        /// <param name="action">The action to execute on the right value.</param>
        /// <returns>This instance, unchanged.</returns>
        public Either<TLeft, TRight> Tap(Action<TRight> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (IsRight) action(_right!);
            return this;
        }

        /// <summary>
        /// Executes a side-effect on the Left value.
        /// </summary>
        /// <param name="action">The action to execute on the left value.</param>
        /// <returns>This instance, unchanged.</returns>
        public Either<TLeft, TRight> TapLeft(Action<TLeft> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (IsLeft) action(_left!);
            return this;
        }

        /// <summary>
        /// Asynchronously transforms the Right value if present.
        /// </summary>
        /// <typeparam name="TNewRight">The type of the new right value.</typeparam>
        /// <param name="mapper">The async function to transform the right value.</param>
        /// <returns>An Either with the transformed right value, or the original left value.</returns>
        public async Task<Either<TLeft, TNewRight>> MapAsync<TNewRight>(Func<TRight, Task<TNewRight>> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsRight
                ? Either<TLeft, TNewRight>.FromRight(await mapper(_right!).ConfigureAwait(false))
                : Either<TLeft, TNewRight>.FromLeft(_left!);
        }

        /// <summary>
        /// Asynchronously chains operations on the Right value.
        /// </summary>
        /// <typeparam name="TNewRight">The type of the new right value.</typeparam>
        /// <param name="binder">The async function that returns a new Either.</param>
        /// <returns>The result of the binder, or the original left value.</returns>
        public async Task<Either<TLeft, TNewRight>> BindAsync<TNewRight>(
            Func<TRight, Task<Either<TLeft, TNewRight>>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return IsRight
                ? await binder(_right!).ConfigureAwait(false)
                : Either<TLeft, TNewRight>.FromLeft(_left!);
        }

        // IEquatable implementation
        /// <summary>
        /// Determines whether the specified <see cref="Either{TLeft, TRight}"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The either to compare with the current instance.</param>
        /// <returns>true if the specified either is equal to the current instance; otherwise, false.</returns>
        public bool Equals(Either<TLeft, TRight> other)
        {
            if (IsLeft != other.IsLeft) return false;
            return IsLeft
                ? EqualityComparer<TLeft>.Default.Equals(_left, other._left)
                : EqualityComparer<TRight>.Default.Equals(_right, other._right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the specified object is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object? obj) =>
            obj is Either<TLeft, TRight> other && Equals(other);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode() =>
            IsLeft
                ? HashCode.Combine(IsLeft, _left)
                : HashCode.Combine(IsLeft, _right);

        /// <summary>
        /// Determines whether two <see cref="Either{TLeft, TRight}"/> instances are equal.
        /// </summary>
        /// <param name="left">The first either to compare.</param>
        /// <param name="right">The second either to compare.</param>
        /// <returns>true if the eithers are equal; otherwise, false.</returns>
        public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right) =>
            left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Either{TLeft, TRight}"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first either to compare.</param>
        /// <param name="right">The second either to compare.</param>
        /// <returns>true if the eithers are not equal; otherwise, false.</returns>
        public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) =>
            !left.Equals(right);

        /// <summary>
        /// Converts this <see cref="Either{TLeft, TRight}"/> to a <see cref="Maybe{TRight}"/>.
        /// Right becomes Some; Left becomes None (the Left value is discarded).
        /// </summary>
        /// <returns>A Maybe containing the Right value, or None.</returns>
        public Maybe<TRight> ToMaybe() =>
            IsRight ? Maybe<TRight>.Some(_right!) : Maybe<TRight>.None;

        /// <summary>
        /// Converts this <see cref="Either{TLeft, TRight}"/> to a <see cref="Result{TRight}"/>
        /// using the specified function to convert the Left value to an error message.
        /// </summary>
        /// <param name="errorMapper">The function to convert the Left value to an error string.</param>
        /// <returns>A successful Result with the Right value, or a failed Result with the mapped error.</returns>
        public Result<TRight> ToResult(Func<TLeft, string> errorMapper)
        {
            ArgumentNullException.ThrowIfNull(errorMapper);
            return IsRight
                ? Result<TRight>.Success(_right!)
                : Result<TRight>.Failure(errorMapper(_left!));
        }

        /// <summary>
        /// Converts this <see cref="Either{TLeft, TRight}"/> to a <see cref="Result{TRight, TLeft}"/>.
        /// Right becomes Success; Left becomes Failure preserving the typed error.
        /// </summary>
        /// <returns>A Result with the Right value as success, or the Left value as a typed error.</returns>
        public Result<TRight, TLeft> ToResult() =>
            IsRight
                ? Result<TRight, TLeft>.Success(_right!)
                : Result<TRight, TLeft>.Failure(_left!);

        /// <summary>
        /// Converts this <see cref="Either{TLeft, TRight}"/> to a <see cref="Validation{TRight}"/>
        /// using the specified function to convert the Left value to an error message.
        /// </summary>
        /// <param name="errorMapper">The function to convert the Left value to an error string.</param>
        /// <returns>A valid Validation with the Right value, or an invalid Validation with the mapped error.</returns>
        public Validation<TRight> ToValidation(Func<TLeft, string> errorMapper)
        {
            ArgumentNullException.ThrowIfNull(errorMapper);
            return IsRight
                ? Validation<TRight>.Valid(_right!)
                : Validation<TRight>.Invalid(errorMapper(_left!));
        }

        /// <summary>Swaps Left and Right. Useful for switching between failure-on-left and failure-on-right conventions.</summary>
        /// <returns>An Either with Left and Right swapped.</returns>
        public Either<TRight, TLeft> Swap() =>
            IsLeft
                ? Either<TRight, TLeft>.FromRight(_left!)
                : Either<TRight, TLeft>.FromLeft(_right!);

        /// <summary>Transforms both Left and Right values with their respective mapping functions.</summary>
        /// <typeparam name="TNewLeft">The type of the new Left value.</typeparam>
        /// <typeparam name="TNewRight">The type of the new Right value.</typeparam>
        /// <param name="leftMapper">The function to transform the Left value.</param>
        /// <param name="rightMapper">The function to transform the Right value.</param>
        /// <returns>An Either with both sides transformed.</returns>
        public Either<TNewLeft, TNewRight> BiMap<TNewLeft, TNewRight>(
            Func<TLeft, TNewLeft> leftMapper,
            Func<TRight, TNewRight> rightMapper)
        {
            ArgumentNullException.ThrowIfNull(leftMapper);
            ArgumentNullException.ThrowIfNull(rightMapper);
            return IsLeft
                ? Either<TNewLeft, TNewRight>.FromLeft(leftMapper(_left!))
                : Either<TNewLeft, TNewRight>.FromRight(rightMapper(_right!));
        }

        /// <summary>Executes one of two side-effects depending on which side is present.</summary>
        /// <param name="onLeft">The action to execute if this is a Left.</param>
        /// <param name="onRight">The action to execute if this is a Right.</param>
        /// <returns>This instance, unchanged.</returns>
        public Either<TLeft, TRight> TapBoth(Action<TLeft> onLeft, Action<TRight> onRight)
        {
            ArgumentNullException.ThrowIfNull(onLeft);
            ArgumentNullException.ThrowIfNull(onRight);
            if (IsLeft) onLeft(_left!); else onRight(_right!);
            return this;
        }

        /// <summary>Enables LINQ query syntax support (alias for Map over Right).</summary>
        public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector) => Map(selector);

        /// <summary>Enables LINQ query syntax support for chaining operations over Right.</summary>
        public Either<TLeft, TResult> SelectMany<TIntermediate, TResult>(
            Func<TRight, Either<TLeft, TIntermediate>> selector,
            Func<TRight, TIntermediate, TResult> projector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(projector);
            return Bind(r => selector(r).Map(i => projector(r, i)));
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string in the form "Left(value)" or "Right(value)".</returns>
        public override string ToString() =>
            IsLeft ? $"Left({_left})" : $"Right({_right})";
    }
}

