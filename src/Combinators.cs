namespace Grondo
{
    /// <summary>
    /// Static combinators for working with collections of functional types.
    /// Provides Sequence (turn a collection of containers inside-out) and Traverse
    /// (map-then-sequence) operations across <see cref="Result{T}"/>, <see cref="Maybe{T}"/>,
    /// <see cref="Validation{T}"/>, and <see cref="Result{T, TError}"/>.
    /// </summary>
    public static class Combinators
    {
        /// <summary>
        /// Transforms an enumerable of <see cref="Result{T}"/> into a single result containing the list of values.
        /// Short-circuits on the first failure.
        /// </summary>
        public static Result<IReadOnlyList<T>> Sequence<T>(IEnumerable<Result<T>> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            var values = new List<T>();
            foreach (Result<T> r in source)
            {
                if (r.IsFailure) return Result<IReadOnlyList<T>>.Failure(r.Error);
                values.Add(r.Value);
            }
            return Result<IReadOnlyList<T>>.Success(values);
        }

        /// <summary>
        /// Maps each element through a result-returning function, then sequences the results.
        /// Short-circuits on the first failure.
        /// </summary>
        public static Result<IReadOnlyList<TResult>> Traverse<T, TResult>(
            IEnumerable<T> source, Func<T, Result<TResult>> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            return Sequence(source.Select(selector));
        }

        /// <summary>
        /// Transforms an enumerable of <see cref="Maybe{T}"/> into a single <see cref="Maybe{T}"/>
        /// containing the list of values. Returns None if any element is None.
        /// </summary>
        public static Maybe<IReadOnlyList<T>> Sequence<T>(IEnumerable<Maybe<T>> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            var values = new List<T>();
            foreach (Maybe<T> m in source)
            {
                if (m.HasNoValue) return Maybe<IReadOnlyList<T>>.None;
                values.Add(m.Value);
            }
            return Maybe<IReadOnlyList<T>>.Some(values);
        }

        /// <summary>
        /// Maps each element through a Maybe-returning function, then sequences the results.
        /// Returns None if any mapped element is None.
        /// </summary>
        public static Maybe<IReadOnlyList<TResult>> Traverse<T, TResult>(
            IEnumerable<T> source, Func<T, Maybe<TResult>> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            return Sequence(source.Select(selector));
        }

        /// <summary>
        /// Transforms an enumerable of <see cref="Validation{T}"/> into a single validation containing
        /// the list of values, accumulating all errors across all elements.
        /// </summary>
        public static Validation<IReadOnlyList<T>> Sequence<T>(IEnumerable<Validation<T>> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            var values = new List<T>();
            var errors = new List<string>();
            foreach (Validation<T> v in source)
            {
                if (v.IsValid) values.Add(v.Value);
                else errors.AddRange(v.Errors);
            }
            return errors.Count > 0
                ? Validation<IReadOnlyList<T>>.Invalid(errors)
                : Validation<IReadOnlyList<T>>.Valid(values);
        }

        /// <summary>
        /// Maps each element through a Validation-returning function, then sequences the results,
        /// accumulating all errors.
        /// </summary>
        public static Validation<IReadOnlyList<TResult>> Traverse<T, TResult>(
            IEnumerable<T> source, Func<T, Validation<TResult>> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            return Sequence(source.Select(selector));
        }

        /// <summary>
        /// Transforms an enumerable of <see cref="Result{T, TError}"/> into a single typed-error result
        /// containing the list of values. Short-circuits on the first failure.
        /// </summary>
        public static Result<IReadOnlyList<T>, TError> Sequence<T, TError>(
            IEnumerable<Result<T, TError>> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            var values = new List<T>();
            foreach (Result<T, TError> r in source)
            {
                if (r.IsFailure) return Result<IReadOnlyList<T>, TError>.Failure(r.Error);
                values.Add(r.Value);
            }
            return Result<IReadOnlyList<T>, TError>.Success(values);
        }
    }
}
