namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IList{T}"/> and <see cref="List{T}"/>.
    /// </summary>
    public static class ListEx
    {
        extension<T>(IList<T> list)
        {
            /// <summary>
            /// Adds an item to the list if the item is not <c>null</c>.
            /// </summary>
            /// <param name="item">The item to add if not <c>null</c>.</param>
            /// <exception cref="ArgumentNullException">Thrown if the list is <c>null</c>.</exception>
            public void AddIfNotNull(T item)
            {
                ArgumentNullException.ThrowIfNull(list);
                if (item is not null) list.Add(item);
            }

            /// <summary>
            /// Adds one or more items to the list.
            /// </summary>
            /// <param name="items">The items to add.</param>
            /// <exception cref="ArgumentNullException">Thrown if the list or <paramref name="items"/> is <c>null</c>.</exception>
            public void AddRange(params T[] items)
            {
                ArgumentNullException.ThrowIfNull(list);
                ArgumentNullException.ThrowIfNull(items);

                foreach (T? item in items)
                {
                    list.Add(item);
                }
            }

            /// <summary>
            /// Adds a range of items to the list, skipping any items that are <c>null</c>.
            /// </summary>
            /// <param name="items">The source items to add.</param>
            /// <exception cref="ArgumentNullException">Thrown if the list or <paramref name="items"/> is <c>null</c>.</exception>
            public void AddRangeIfNotNull(params T[] items)
            {
                ArgumentNullException.ThrowIfNull(list);
                ArgumentNullException.ThrowIfNull(items);

                foreach (T? item in items)
                {
                    list.AddIfNotNull(item);
                }
            }
        }

        extension<T>(List<T> list)
        {
            /// <summary>
            /// Adds one or more items to the list and returns the list for fluent chaining.
            /// </summary>
            /// <param name="items">The items to add.</param>
            /// <returns>The same list instance with the new items added.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the list or <paramref name="items"/> is null.</exception>
            public List<T> Add(params T[] items)
            {
                ArgumentNullException.ThrowIfNull(list);
                ArgumentNullException.ThrowIfNull(items);

                list.AddRange(items);
                return list;
            }
        }

        extension(List<string> list)
        {
            /// <summary>
            /// Adds a string to the list if it does not already exist (case-insensitive).
            /// </summary>
            /// <param name="value">The string value to add if not already present.</param>
            /// <returns>The same list instance.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the list or <paramref name="value"/> is null.</exception>
            public List<string> AddIfNotExists(string value)
            {
                ArgumentNullException.ThrowIfNull(list);
                ArgumentNullException.ThrowIfNull(value);

                if (!list.ContainsIgnoreCase(value))
                {
                    list.Add(value);
                }

                return list;
            }

            /// <summary>
            /// Adds a range of strings to the list, ensuring no duplicates are added (case-insensitive).
            /// </summary>
            /// <param name="range">The range of strings to add.</param>
            /// <returns>The same list instance.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the list or <paramref name="range"/> is null.</exception>
            public List<string> AddRangeNoDuplicates(IEnumerable<string> range)
            {
                ArgumentNullException.ThrowIfNull(list);
                ArgumentNullException.ThrowIfNull(range);

                var existing = new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);

                foreach (string item in range.Where(item => existing.Add(item)))
                {
                    list.Add(item);
                }

                return list;
            }

            /// <summary>
            /// Determines whether the list contains the specified string using a case-insensitive comparison.
            /// </summary>
            /// <param name="toCompare">The string value to compare.</param>
            /// <returns><c>true</c> if the list contains the specified string (case-insensitive); otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the list or <paramref name="toCompare"/> is null.</exception>
            public bool ContainsIgnoreCase(string toCompare)
            {
                ArgumentNullException.ThrowIfNull(list);
                ArgumentNullException.ThrowIfNull(toCompare);

                return list.Any(x => x.Equals(toCompare, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}

