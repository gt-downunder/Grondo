namespace Grondo.Extensions
{
    /// <summary>
    /// Provides asynchronous extension methods for <see cref="IEnumerable{T}"/>.
    /// For synchronous extensions, see <see cref="EnumerableEx"/>.
    /// </summary>
    public static class EnumerableExAsync
    {
        extension<T>(IEnumerable<T> source)
        {
            /// <summary>
            /// Projects each element of a sequence to a new form asynchronously.
            /// </summary>
            /// <typeparam name="TResult">The type of the result elements.</typeparam>
            /// <param name="selector">The async function to transform each element.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A sequence of transformed elements.</returns>
            public async Task<IEnumerable<TResult>> SelectAsync<TResult>(
                Func<T, Task<TResult>> selector,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(selector);

                var results = new List<TResult>();
                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    results.Add(await selector(item).ConfigureAwait(false));
                }
                return results;
            }

            /// <summary>
            /// Projects each element asynchronously in parallel with max concurrency.
            /// </summary>
            /// <typeparam name="TResult">The type of the result elements.</typeparam>
            /// <param name="selector">The async function to transform each element.</param>
            /// <param name="maxConcurrency">The maximum number of concurrent operations.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A sequence of transformed elements.</returns>
            public async Task<IEnumerable<TResult>> SelectAsyncParallel<TResult>(
                Func<T, Task<TResult>> selector,
                int maxConcurrency = 10,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(selector);
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxConcurrency);

                using var semaphore = new SemaphoreSlim(maxConcurrency);
                var tasks = source.Select(async item =>
                {
                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        return await selector(item).ConfigureAwait(false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                return await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            /// <summary>
            /// Filters a sequence based on an asynchronous predicate.
            /// </summary>
            /// <param name="predicate">The async function to test each element.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A filtered sequence.</returns>
            public async Task<IEnumerable<T>> WhereAsync(
                Func<T, Task<bool>> predicate,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);

                var results = new List<T>();
                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (await predicate(item).ConfigureAwait(false))
                    {
                        results.Add(item);
                    }
                }
                return results;
            }

            /// <summary>
            /// Performs an async action on each element.
            /// </summary>
            /// <param name="action">The async action to perform on each element.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            public async Task ForEachAsync(
                Func<T, Task> action,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(action);

                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await action(item).ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Aggregates values asynchronously.
            /// </summary>
            /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
            /// <param name="seed">The initial accumulator value.</param>
            /// <param name="accumulator">The async function to accumulate values.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>The final accumulated value.</returns>
            public async Task<TAccumulate> AggregateAsync<TAccumulate>(
                TAccumulate seed,
                Func<TAccumulate, T, Task<TAccumulate>> accumulator,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(accumulator);

                var result = seed;
                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    result = await accumulator(result, item).ConfigureAwait(false);
                }
                return result;
            }

            /// <summary>
            /// Determines whether any element satisfies an asynchronous condition.
            /// </summary>
            /// <param name="predicate">The async function to test each element.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>true if any element satisfies the condition; otherwise, false.</returns>
            public async Task<bool> AnyAsync(
                Func<T, Task<bool>> predicate,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);

                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (await predicate(item).ConfigureAwait(false))
                        return true;
                }
                return false;
            }

            /// <summary>
            /// Determines whether all elements satisfy an asynchronous condition.
            /// </summary>
            /// <param name="predicate">The async function to test each element.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>true if all elements satisfy the condition; otherwise, false.</returns>
            public async Task<bool> AllAsync(
                Func<T, Task<bool>> predicate,
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);

                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!await predicate(item).ConfigureAwait(false))
                        return false;
                }
                return true;
            }
        }
    }
}

