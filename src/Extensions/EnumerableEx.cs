namespace Grondo.Extensions
{
    /// <summary>
    /// Provides synchronous extension methods for <see cref="IEnumerable{T}"/>.
    /// For asynchronous extensions, see <see cref="EnumerableExAsync"/>.
    /// </summary>
    public static class EnumerableEx
    {
        extension<T>(IEnumerable<T>? source)
        {
            /// <summary>
            /// Determines whether the specified sequence is <c>null</c> or contains no elements.
            /// </summary>
            /// <returns><c>true</c> if the sequence is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
            public bool IsNullOrEmpty() =>
                source?.Any() != true;

            /// <summary>
            /// Determines whether the specified sequence is not <c>null</c> and contains at least one element.
            /// </summary>
            /// <returns><c>true</c> if the sequence is not <c>null</c> and contains elements; otherwise, <c>false</c>.</returns>
            public bool IsNotNullOrEmpty() =>
                source?.Any() == true;
        }

        extension<T>(IEnumerable<T> source)
        {
            /// <summary>
            /// Determines whether the specified sequence contains no elements.
            /// </summary>
            /// <returns><c>true</c> if the sequence is empty; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public bool IsEmpty()
            {
                ArgumentNullException.ThrowIfNull(source);
                return !source.Any();
            }

            /// <summary>
            /// Determines whether the specified sequence contains at least one element.
            /// </summary>
            /// <returns><c>true</c> if the sequence contains one or more elements; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public bool IsNotEmpty() => !source.IsEmpty();

            /// <summary>
            /// Determines whether two sequences contain the same elements in the same order.
            /// </summary>
            /// <param name="target">The target sequence to compare with.</param>
            /// <returns><c>true</c> if the sequences are equal; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if either the source or <paramref name="target"/> is null.</exception>
            public bool IsDeepEqualTo(IEnumerable<T> target)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(target);
                return source.SequenceEqual(target);
            }

            /// <summary>
            /// Iterates through a sequence and executes the specified action for each element,
            /// providing both the element and its index.
            /// </summary>
            /// <param name="action">The action to execute for each element and its index.</param>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="action"/> is null.</exception>
            public void ForEach(Action<T, int> action)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(action);

                int index = 0;
                foreach (T? item in source)
                {
                    action(item, index++);
                }
            }

            /// <summary>
            /// Iterates through a sequence and executes the specified function for each element,
            /// providing both the element and its index. Iteration continues until the function
            /// returns <c>false</c> or all elements have been processed.
            /// </summary>
            /// <param name="func">The function to execute for each element and its index.
            /// Return <c>false</c> to break the iteration early.</param>
            /// <returns><c>true</c> if all elements were processed without the function returning <c>false</c>; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="func"/> is null.</exception>
            public bool ForEach(Func<T, int, bool> func)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(func);

                return source.Select((item, index) => func(item, index)).All(result => result);
            }

            /// <summary>
            /// Splits the sequence into batches of the specified size.
            /// The last batch may contain fewer elements.
            /// </summary>
            /// <param name="batchSize">The maximum number of elements per batch.</param>
            /// <returns>A sequence of batches, each containing up to <paramref name="batchSize"/> elements.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="batchSize"/> is less than 1.</exception>
            public IEnumerable<IReadOnlyList<T>> Batch(int batchSize)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentOutOfRangeException.ThrowIfLessThan(batchSize, 1);

                var batch = new List<T>(batchSize);
                foreach (T item in source)
                {
                    batch.Add(item);
                    if (batch.Count == batchSize)
                    {
                        yield return batch.ToArray(); // Return array instead of list
                        batch.Clear(); // Reuse list instead of allocating new one
                    }
                }

                if (batch.Count > 0)
                    yield return batch.ToArray();
            }

            /// <summary>
            /// Splits the sequence into two groups based on a predicate.
            /// </summary>
            /// <param name="predicate">The predicate to evaluate for each element.</param>
            /// <returns>A tuple of (Matches, NonMatches) where Matches contains elements for which the predicate returned <c>true</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="predicate"/> is null.</exception>
            public (IReadOnlyList<T> Matches, IReadOnlyList<T> NonMatches) Partition(Func<T, bool> predicate)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);

                var matches = new List<T>();
                var nonMatches = new List<T>();

                foreach (T item in source)
                {
                    if (predicate(item))
                        matches.Add(item);
                    else
                        nonMatches.Add(item);
                }

                return (matches, nonMatches);
            }

            /// <summary>
            /// Interleaves the elements of this sequence with another sequence,
            /// alternating elements from each. If one sequence is longer, remaining elements are appended.
            /// </summary>
            /// <param name="other">The other sequence to interleave with.</param>
            /// <returns>A sequence with elements alternating from both sequences.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="other"/> is null.</exception>
            public IEnumerable<T> Interleave(IEnumerable<T> other)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(other);

                using IEnumerator<T> e1 = source.GetEnumerator();
                using IEnumerator<T> e2 = other.GetEnumerator();
                bool has1 = e1.MoveNext();
                bool has2 = e2.MoveNext();

                while (has1 || has2)
                {
                    if (has1)
                    {
                        yield return e1.Current;
                        has1 = e1.MoveNext();
                    }

                    if (has2)
                    {
                        yield return e2.Current;
                        has2 = e2.MoveNext();
                    }
                }
            }

            /// <summary>
            /// Applies an accumulator function over a sequence and yields each intermediate result.
            /// </summary>
            /// <typeparam name="TAccumulate">The type of the accumulated value.</typeparam>
            /// <param name="seed">The initial accumulator value.</param>
            /// <param name="accumulator">The accumulator function applied to each element.</param>
            /// <returns>A sequence of intermediate accumulated values.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="accumulator"/> is null.</exception>
            public IEnumerable<TAccumulate> Scan<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> accumulator)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(accumulator);

                TAccumulate state = seed;
                foreach (T item in source)
                {
                    state = accumulator(state, item);
                    yield return state;
                }
            }

            /// <summary>
            /// Applies an accumulator function over a sequence without a seed and yields each intermediate result.
            /// The first element is used as the initial accumulator value.
            /// </summary>
            /// <param name="accumulator">The accumulator function applied to each consecutive pair.</param>
            /// <returns>A sequence of intermediate accumulated values.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="accumulator"/> is null.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the source is empty.</exception>
            public IEnumerable<T> Scan(Func<T, T, T> accumulator)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(accumulator);

                using IEnumerator<T> enumerator = source.GetEnumerator();
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements.");

                T state = enumerator.Current;
                yield return state;

                while (enumerator.MoveNext())
                {
                    state = accumulator(state, enumerator.Current);
                    yield return state;
                }
            }

            /// <summary>
            /// Returns overlapping windows of the specified size from the sequence.
            /// </summary>
            /// <param name="size">The size of each window.</param>
            /// <returns>A sequence of overlapping windows, each as an <see cref="IReadOnlyList{T}"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="size"/> is less than 1.</exception>
            public IEnumerable<IReadOnlyList<T>> Window(int size)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

                var window = new Queue<T>(size);
                foreach (T item in source)
                {
                    window.Enqueue(item);
                    if (window.Count == size)
                    {
                        yield return window.ToArray();
                        window.Dequeue();
                    }
                }
            }

            /// <summary>
            /// Returns a randomly shuffled copy of the sequence using the Fisher-Yates algorithm.
            /// </summary>
            /// <param name="random">An optional <see cref="Random"/> instance. If null, a shared instance is used.</param>
            /// <returns>A shuffled copy of the sequence as an <see cref="IReadOnlyList{T}"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public IReadOnlyList<T> Shuffle(Random? random = null)
            {
                ArgumentNullException.ThrowIfNull(source);

                var list = source.ToList();
                random ??= Random.Shared;

                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }

                return list.AsReadOnly();
            }

            /// <summary>
            /// Returns the elements of the source sequence, or the fallback values if the source is empty.
            /// </summary>
            /// <param name="fallback">The fallback values to return if the source is empty.</param>
            /// <returns>The source sequence if non-empty; otherwise, the fallback values.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="fallback"/> is null.</exception>
            public IEnumerable<T> FallbackIfEmpty(params T[] fallback)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(fallback);

                return FallbackIfEmptyIterator(source, fallback);

                static IEnumerable<T> FallbackIfEmptyIterator(IEnumerable<T> source, IEnumerable<T> fallback)
                {
                    bool hasElements = false;
                    foreach (T item in source)
                    {
                        hasElements = true;
                        yield return item;
                    }

                    if (!hasElements)
                    {
                        foreach (T item in fallback)
                            yield return item;
                    }
                }
            }

            /// <summary>
            /// Returns the elements of the source sequence, or the fallback sequence if the source is empty.
            /// </summary>
            /// <param name="fallback">The fallback sequence to return if the source is empty.</param>
            /// <returns>The source sequence if non-empty; otherwise, the fallback sequence.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="fallback"/> is null.</exception>
            public IEnumerable<T> FallbackIfEmpty(IEnumerable<T> fallback)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(fallback);

                return FallbackIfEmptyIterator(source, fallback);

                static IEnumerable<T> FallbackIfEmptyIterator(IEnumerable<T> source, IEnumerable<T> fallback)
                {
                    bool hasElements = false;
                    foreach (T item in source)
                    {
                        hasElements = true;
                        yield return item;
                    }

                    if (!hasElements)
                    {
                        foreach (T item in fallback)
                            yield return item;
                    }
                }
            }

            /// <summary>
            /// Applies a function to each consecutive pair of elements in the sequence.
            /// </summary>
            /// <typeparam name="TResult">The type of the result elements.</typeparam>
            /// <param name="resultSelector">The function to apply to each consecutive pair.</param>
            /// <returns>A sequence of results from applying the function to each pair.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="resultSelector"/> is null.</exception>
            public IEnumerable<TResult> Pairwise<TResult>(Func<T, T, TResult> resultSelector)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(resultSelector);

                using IEnumerator<T> enumerator = source.GetEnumerator();
                if (!enumerator.MoveNext())
                    yield break;

                T previous = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return resultSelector(previous, enumerator.Current);
                    previous = enumerator.Current;
                }
            }

            /// <summary>
            /// Tags each element with whether it is the first and/or last element in the sequence.
            /// </summary>
            /// <typeparam name="TResult">The type of the result elements.</typeparam>
            /// <param name="resultSelector">A function that receives each element along with isFirst and isLast flags.</param>
            /// <returns>A sequence of tagged results.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="resultSelector"/> is null.</exception>
            public IEnumerable<TResult> TagFirstLast<TResult>(Func<T, bool, bool, TResult> resultSelector)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(resultSelector);

                using IEnumerator<T> enumerator = source.GetEnumerator();
                if (!enumerator.MoveNext())
                    yield break;

                bool isFirst = true;
                T current = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    yield return resultSelector(current, isFirst, false);
                    isFirst = false;
                    current = enumerator.Current;
                }

                yield return resultSelector(current, isFirst, true);
            }

            /// <summary>
            /// Determines whether the sequence contains any duplicate elements using the default equality comparer.
            /// </summary>
            /// <returns><c>true</c> if the sequence has at least one duplicate; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public bool HasDuplicates()
            {
                ArgumentNullException.ThrowIfNull(source);

                var seen = new HashSet<T>();
                foreach (T item in source)
                {
                    if (!seen.Add(item)) return true;
                }
                return false;
            }

            /// <summary>
            /// Determines whether no elements of the sequence satisfy the specified predicate.
            /// </summary>
            /// <param name="predicate">The predicate to evaluate for each element.</param>
            /// <returns><c>true</c> if no elements match; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="predicate"/> is null.</exception>
            public bool None(Func<T, bool> predicate)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);
                return !source.Any(predicate);
            }

            /// <summary>
            /// Returns an infinite sequence cycling through the elements of the source sequence.
            /// </summary>
            /// <returns>A lazily-evaluated infinite sequence. Callers must limit enumeration.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the source is empty when enumeration begins.</exception>
            public IEnumerable<T> Cycle()
            {
                ArgumentNullException.ThrowIfNull(source);
                return CycleIterator(source);

                static IEnumerable<T> CycleIterator(IEnumerable<T> src)
                {
                    var buffer = new List<T>();
                    foreach (T item in src)
                    {
                        buffer.Add(item);
                        yield return item;
                    }

                    if (buffer.Count == 0)
                        throw new InvalidOperationException("Cannot cycle an empty sequence.");

                    while (true)
                    {
                        foreach (T item in buffer)
                            yield return item;
                    }
                }
            }
        }

        extension<T>(IEnumerable<IEnumerable<T>> source)
        {
            /// <summary>
            /// Flattens a sequence of sequences into a single sequence by concatenation.
            /// </summary>
            /// <returns>A flat sequence containing all elements from all inner sequences.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public IEnumerable<T> Flatten()
            {
                ArgumentNullException.ThrowIfNull(source);
                return source.SelectMany(x => x);
            }
        }

        extension<T>(IEnumerable<T?> source)
        {
            /// <summary>
            /// Filters out <c>null</c> values from the sequence.
            /// </summary>
            /// <returns>A sequence containing only non-null elements.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public IEnumerable<T> WhereNotNull()
            {
                ArgumentNullException.ThrowIfNull(source);

                return source.Where(item => item is not null)!;
            }
        }
    }
}
