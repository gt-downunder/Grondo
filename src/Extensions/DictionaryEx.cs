using System.Text;
using System.Text.Json;
using Grondo.Utilities;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class DictionaryEx
    {
        extension<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            /// <summary>
            /// Serializes the dictionary into a JSON string.
            /// </summary>
            /// <returns>A JSON string representation of the dictionary.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
            public string ToJson()
            {
                ArgumentNullException.ThrowIfNull(source);
                return JsonSerializer.Serialize(source, JsonDefaults.Default);
            }

            /// <summary>
            /// Determines whether the dictionary contains the specified key and its value is not null.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><c>true</c> if the dictionary contains the key and its value is not null; otherwise, <c>false</c>.</returns>
            public bool HasKeyAndValue(TKey key) =>
                source.TryGetValue(key, out TValue? value) && value is not null;

            /// <summary>
            /// Determines whether the dictionary contains any of the specified keys.
            /// </summary>
            /// <param name="keyList">An array of keys to search for.</param>
            /// <returns><c>true</c> if any of the keys are present in the dictionary; otherwise, <c>false</c>.</returns>
            public bool HasAnyKey(TKey[]? keyList)
            {
                if (keyList is null) return false;

                return keyList.Where(key => key is not null).Any(key => source.ContainsKey(key!));
            }

            /// <summary>
            /// Converts the dictionary into a URL query string.
            /// </summary>
            /// <returns>A query string representation of the dictionary, or an empty string if the dictionary is empty.</returns>
            public string ToQueryString()
            {
                if (!source.Any())
                    return string.Empty;

                var sb = new StringBuilder("?");
                bool first = true;
                foreach (KeyValuePair<TKey, TValue> kv in source)
                {
                    if (!first) sb.Append('&');
                    first = false;
                    sb.Append(Uri.EscapeDataString(kv.Key?.ToString() ?? string.Empty));
                    sb.Append('=');
                    sb.Append(Uri.EscapeDataString(kv.Value?.ToString() ?? string.Empty));
                }

                return sb.ToString();
            }

            /// <summary>
            /// Returns a string representation of the dictionary in a debug-friendly format.
            /// </summary>
            /// <returns>A string representation of the dictionary in the form "{key=value, ...}".</returns>
            public string ToDebugString() =>
                "{" + string.Join(", ", source.Select(kv => $"{kv.Key}={kv.Value}")) + "}";

            /// <summary>
            /// Determines whether two dictionaries contain the same key-value pairs.
            /// </summary>
            /// <param name="other">The dictionary to compare with.</param>
            /// <returns><c>true</c> if both dictionaries contain the same key-value pairs; otherwise, <c>false</c>.</returns>
            public bool IsDeepEqualTo(IDictionary<TKey, TValue> other)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(other);
                return source.Count == other.Count && source.All(kv =>
                    other.TryGetValue(kv.Key, out TValue? val) && EqualityComparer<TValue>.Default.Equals(kv.Value, val));
            }

            /// <summary>
            /// Merges another dictionary into this dictionary. If a key already exists, its value
            /// is overwritten when <paramref name="overwrite"/> is <c>true</c>; otherwise, existing values are preserved.
            /// </summary>
            /// <param name="other">The dictionary to merge from.</param>
            /// <param name="overwrite">Whether to overwrite existing keys. Defaults to <c>true</c>.</param>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="other"/> is null.</exception>
            public void Merge(IDictionary<TKey, TValue> other, bool overwrite = true)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(other);

                foreach (KeyValuePair<TKey, TValue> kvp in other.Where(kvp => overwrite || !source.ContainsKey(kvp.Key)))
                {
                    source[kvp.Key] = kvp.Value;
                }
            }

            /// <summary>
            /// Gets the value associated with the specified key. If the key does not exist,
            /// the value is created using the factory, added to the dictionary, and returned.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <param name="factory">The function to create a value if the key does not exist.</param>
            /// <returns>The existing or newly created value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source, <paramref name="key"/>, or <paramref name="factory"/> is null.</exception>
            public TValue GetOrAdd(TKey key, Func<TValue> factory)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(key);
                ArgumentNullException.ThrowIfNull(factory);

                if (source.TryGetValue(key, out TValue? existing))
                    return existing;

                TValue newValue = factory();
                source[key] = newValue;
                return newValue;
            }

            /// <summary>
            /// Adds a value to the dictionary for the specified key, or updates the existing value using the provided update function.
            /// </summary>
            /// <param name="key">The key to add or update.</param>
            /// <param name="addValue">The value to add if the key does not exist.</param>
            /// <param name="updateValueFactory">A function that produces the new value given the key and existing value.</param>
            /// <returns>The value that is now stored for the key.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source, <paramref name="key"/>, or <paramref name="updateValueFactory"/> is null.</exception>
            public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(key);
                ArgumentNullException.ThrowIfNull(updateValueFactory);

                if (source.TryGetValue(key, out TValue? existing))
                {
                    TValue updated = updateValueFactory(key, existing);
                    source[key] = updated;
                    return updated;
                }

                source[key] = addValue;
                return addValue;
            }

            /// <summary>
            /// Removes all entries from the dictionary that match the specified predicate.
            /// </summary>
            /// <param name="predicate">The condition evaluated for each key-value pair.</param>
            /// <returns>The number of entries removed.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the source or <paramref name="predicate"/> is null.</exception>
            public int RemoveWhere(Func<KeyValuePair<TKey, TValue>, bool> predicate)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);

                List<TKey> keysToRemove = [.. source.Where(predicate).Select(kv => kv.Key)];
                foreach (TKey key in keysToRemove)
                    source.Remove(key);
                return keysToRemove.Count;
            }
        }

        extension<TKey, TValue>(IDictionary<TKey, TValue?> source)
        {
            /// <summary>
            /// Retrieves the value associated with the specified key, or the default value if the key is not found.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The value associated with the key if found; otherwise, the default value of <typeparamref name="TValue"/>.</returns>
            public TValue? GetValueOrDefault(TKey key) =>
                source.TryGetValue(key, out TValue? value) ? value : default;
        }

        extension<TKey, TValue>(IDictionary<TKey, TValue> dict) where TKey : notnull
        {
            /// <summary>
            /// Determines whether the dictionary contains the same key-value pairs as a serialized dictionary string.
            /// </summary>
            /// <param name="serializedDictionary">A JSON string representing the target dictionary.</param>
            /// <returns><c>true</c> if both dictionaries contain the same key-value pairs; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="serializedDictionary"/> is null or whitespace.</exception>
            public bool IsDeepEqualTo(string serializedDictionary)
            {
                if (string.IsNullOrWhiteSpace(serializedDictionary))
                    throw new ArgumentNullException(nameof(serializedDictionary));

                Dictionary<TKey, TValue>? deserialized = JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(serializedDictionary, JsonDefaults.CaseInsensitive);
                return deserialized != null && dict.IsDeepEqualTo(deserialized);
            }
        }

        extension(IDictionary<string, string> dict)
        {
            /// <summary>
            /// Adds a key-value pair to a nested dictionary stored as a JSON string within the parent dictionary.
            /// </summary>
            /// <param name="fieldName">The key in the parent dictionary that stores the nested dictionary.</param>
            /// <param name="key">The key to add to the nested dictionary.</param>
            /// <param name="value">The value to add to the nested dictionary.</param>
            public void AddEntryToNestedDictionary(string fieldName, string key, string value)
            {
                Dictionary<string, string> links = dict.TryGetValue(fieldName, out string? existing) && !string.IsNullOrWhiteSpace(existing)
                    ? JsonSerializer.Deserialize<Dictionary<string, string>>(existing, JsonDefaults.CaseInsensitive) ?? []
                    : [];

                links[key] = value;
                dict[fieldName] = JsonSerializer.Serialize(links, JsonDefaults.Default);
            }

            /// <summary>
            /// Removes a key-value pair from a nested dictionary stored as a JSON string within the parent dictionary.
            /// </summary>
            /// <param name="fieldName">The key in the parent dictionary that stores the nested dictionary.</param>
            /// <param name="key">The key to remove from the nested dictionary.</param>
            public void RemoveEntryFromNestedDictionary(string fieldName, string key)
            {
                if (!dict.TryGetValue(fieldName, out string? existing) || string.IsNullOrWhiteSpace(existing)) return;

                Dictionary<string, string>? links = JsonSerializer.Deserialize<Dictionary<string, string>>(existing, JsonDefaults.CaseInsensitive);
                if (links == null) return;

                links.Remove(key);
                dict[fieldName] = JsonSerializer.Serialize(links, JsonDefaults.Default);
            }
        }
    }
}
