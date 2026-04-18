using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="string"/>.
    /// </summary>
    public static partial class StringEx
    {
        [GeneratedRegex("[^A-Za-z0-9]")]
        private static partial Regex NonAlphaNumericRegex();

        [GeneratedRegex("[^a-z0-9]+")]
        private static partial Regex SlugSeparatorRegex();

        [GeneratedRegex("(^-|-$)")]
        private static partial Regex SlugTrimRegex();

        [GeneratedRegex("(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|[-_]")]
        private static partial Regex WordBoundaryRegex();

        [GeneratedRegex("<[^>]+>")]
        private static partial Regex HtmlTagRegex();

        private static readonly EmailAddressAttribute _emailAddressValidator = new();

        /// <summary>
        /// Splits a string into words based on word boundaries (PascalCase, camelCase, snake_case, kebab-case).
        /// </summary>
        private static string[] SplitIntoWords(string value)
        {
            if (string.IsNullOrEmpty(value))
                return [];

            return [.. WordBoundaryRegex().Split(value).Where(w => !string.IsNullOrEmpty(w))];
        }

        extension(string value)
        {
            /// <summary>
            /// Converts the string to a UTF-8 encoded byte array.
            /// </summary>
            /// <returns>A UTF-8 encoded byte array representation of the string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            public byte[] ToByteArray()
            {
                ArgumentNullException.ThrowIfNull(value);
                return Encoding.UTF8.GetBytes(value);
            }

            /// <summary>
            /// Removes all non-alphanumeric characters from the string.
            /// </summary>
            /// <returns>The string with only letters and digits.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            public string RemoveSpecialCharacters()
            {
                ArgumentNullException.ThrowIfNull(value);
                return NonAlphaNumericRegex().Replace(value, string.Empty);
            }

            /// <summary>
            /// Splits the string into substrings based on a regex pattern.
            /// </summary>
            /// <param name="pattern">The regex pattern to split on.</param>
            /// <param name="ignoreCase">Whether to ignore case in the regex pattern.</param>
            /// <returns>An array of substrings.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="pattern"/> is null.</exception>
            public string[] RegexSplit(string pattern, bool ignoreCase = false)
            {
                ArgumentNullException.ThrowIfNull(value);
                ArgumentNullException.ThrowIfNull(pattern);
                return Regex.Split(value, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            }

            /// <summary>
            /// Determines whether two strings are equal using case-insensitive comparison.
            /// </summary>
            /// <param name="toCompare">The string to compare with.</param>
            /// <returns><c>true</c> if the strings are equal (case-insensitive); otherwise, <c>false</c>.</returns>
            public bool EqualsIgnoreCase(string toCompare) =>
                string.Equals(value, toCompare, StringComparison.OrdinalIgnoreCase);

            /// <summary>
            /// Determines whether the string ends with the specified substring using case-insensitive comparison.
            /// </summary>
            /// <param name="toCompare">The substring to compare.</param>
            /// <returns><c>true</c> if the string ends with <paramref name="toCompare"/> (case-insensitive); otherwise, <c>false</c>.</returns>
            public bool EndsWithIgnoreCase(string toCompare) =>
                value?.EndsWith(toCompare, StringComparison.OrdinalIgnoreCase) ?? false;

            /// <summary>
            /// Determines whether the string starts with the specified substring using case-insensitive comparison.
            /// </summary>
            /// <param name="toCompare">The substring to compare.</param>
            /// <returns><c>true</c> if the string starts with <paramref name="toCompare"/> (case-insensitive); otherwise, <c>false</c>.</returns>
            public bool StartsWithIgnoreCase(string toCompare) =>
                value?.StartsWith(toCompare, StringComparison.OrdinalIgnoreCase) ?? false;

            /// <summary>
            /// Determines whether the string contains the specified substring using case-insensitive comparison.
            /// </summary>
            /// <param name="toCompare">The substring to search for.</param>
            /// <returns><c>true</c> if the string contains <paramref name="toCompare"/> (case-insensitive); otherwise, <c>false</c>.</returns>
            public bool ContainsIgnoreCase(string toCompare) =>
                value is not null && toCompare is not null && value.Contains(toCompare, StringComparison.OrdinalIgnoreCase);

            /// <summary>
            /// Trims the string safely, returning an empty string if the input is null or whitespace.
            /// </summary>
            /// <returns>The trimmed string, or <see cref="string.Empty"/> if the input is null or whitespace.</returns>
            public string SafeTrim() =>
                string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

            /// <summary>
            /// Attempts to parse the string into an enum of type <typeparamref name="TEnum"/>.
            /// Returns the default value if parsing fails.
            /// </summary>
            /// <typeparam name="TEnum">The enum type to parse into.</typeparam>
            /// <param name="ignoreCase">Whether to ignore case when parsing. Defaults to <c>true</c>.</param>
            /// <returns>The parsed enum value, or <c>default</c> if parsing fails.</returns>
            public TEnum ToEnum<TEnum>(bool ignoreCase = true) where TEnum : struct =>
                Enum.TryParse(value, ignoreCase, out TEnum result) ? result : default;

            /// <summary>
            /// Determines whether the string is a well-formed email address.
            /// </summary>
            /// <returns><c>true</c> if the string is a valid email address; otherwise, <c>false</c>.</returns>
            public bool IsWellFormedEmailAddress() =>
                !string.IsNullOrWhiteSpace(value) && _emailAddressValidator.IsValid(value);

            /// <summary>
            /// Determines whether two strings are equal using case-insensitive comparison after trimming whitespace.
            /// </summary>
            /// <param name="toCompare">The string to compare with.</param>
            /// <returns><c>true</c> if the trimmed strings are equal (case-insensitive); otherwise, <c>false</c>.</returns>
            public bool EqualsIgnoreCaseWithTrim(string toCompare) =>
                value.SafeTrim().EqualsIgnoreCase(toCompare.SafeTrim());

            /// <summary>
            /// Determines whether two strings are equal after trimming whitespace.
            /// </summary>
            /// <param name="toCompare">The string to compare with.</param>
            /// <returns><c>true</c> if the trimmed strings are equal (ordinal); otherwise, <c>false</c>.</returns>
            public bool EqualsWithTrim(string toCompare) =>
                value.SafeTrim().Equals(toCompare.SafeTrim(), StringComparison.Ordinal);

            /// <summary>
            /// Determines whether the string starts with the specified substring using case-insensitive comparison after trimming whitespace.
            /// </summary>
            /// <param name="toCompare">The substring to compare.</param>
            /// <returns><c>true</c> if the trimmed string starts with <paramref name="toCompare"/> (case-insensitive); otherwise, <c>false</c>.</returns>
            public bool StartsWithIgnoreCaseWithTrim(string toCompare) =>
                value.SafeTrim().StartsWithIgnoreCase(toCompare.SafeTrim());

            /// <summary>
            /// Truncates the string to the specified maximum length, appending a suffix if truncation occurs.
            /// </summary>
            /// <param name="maxLength">The maximum length of the returned string, including the suffix.</param>
            /// <param name="suffix">The suffix to append when truncation occurs. Defaults to "...".</param>
            /// <returns>The original string if shorter than <paramref name="maxLength"/>, or the truncated string with suffix.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxLength"/> is negative.</exception>
            public string Truncate(int maxLength, string suffix = "...")
            {
                ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

                if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                    return value ?? string.Empty;

                int suffixLength = suffix?.Length ?? 0;
                if (maxLength <= suffixLength)
                    return (suffix ?? string.Empty)[..maxLength];

                return string.Concat(value.AsSpan(0, maxLength - suffixLength), suffix);
            }

            /// <summary>
            /// Encodes the string to a Base64 representation using UTF-8 encoding.
            /// </summary>
            /// <returns>The Base64-encoded string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            public string ToBase64()
            {
                ArgumentNullException.ThrowIfNull(value);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }

            /// <summary>
            /// Decodes a Base64-encoded string back to its original UTF-8 representation.
            /// </summary>
            /// <returns>The decoded string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            /// <exception cref="FormatException">Thrown if the string is not a valid Base64 string.</exception>
            public string FromBase64()
            {
                ArgumentNullException.ThrowIfNull(value);
                return Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }

            /// <summary>
            /// Masks the string, revealing only the last specified number of characters.
            /// The remaining characters are replaced with the mask character.
            /// </summary>
            /// <param name="visibleChars">The number of trailing characters to leave visible. Defaults to 4.</param>
            /// <param name="maskChar">The character to use for masking. Defaults to '*'.</param>
            /// <returns>The masked string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            public string Mask(int visibleChars = 4, char maskChar = '*')
            {
                ArgumentNullException.ThrowIfNull(value);

                if (value.Length <= visibleChars)
                    return value;

                int maskLength = value.Length - visibleChars;
                return string.Create(value.Length, (value, maskLength, maskChar), static (span, state) =>
                {
                    span[..state.maskLength].Fill(state.maskChar);
                    state.value.AsSpan(state.maskLength).CopyTo(span[state.maskLength..]);
                });
            }

            /// <summary>
            /// Converts the string to a URL-friendly slug by lowercasing, replacing non-alphanumeric
            /// characters with hyphens, and trimming leading/trailing hyphens.
            /// </summary>
            /// <returns>A URL-friendly slug representation of the string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            public string ToSlug()
            {
                ArgumentNullException.ThrowIfNull(value);

                string slug = value.ToLowerInvariant();
                slug = SlugSeparatorRegex().Replace(slug, "-");
                slug = SlugTrimRegex().Replace(slug, string.Empty);
                return slug;
            }

            /// <summary>
            /// Determines whether the string represents a valid numeric value.
            /// Supports integers, decimals, and scientific notation.
            /// </summary>
            /// <returns><c>true</c> if the string can be parsed as a number; otherwise, <c>false</c>.</returns>
            public bool IsNumeric() =>
                !string.IsNullOrWhiteSpace(value) && double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _);

            /// <summary>
            /// Determines whether the string represents a valid GUID.
            /// </summary>
            /// <returns><c>true</c> if the string can be parsed as a GUID; otherwise, <c>false</c>.</returns>
            public bool IsGuid() =>
                !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out _);

            /// <summary>
            /// Converts the string to snake_case (e.g., "MyPropertyName" → "my_property_name").
            /// Handles PascalCase, camelCase, snake_case, and kebab-case inputs.
            /// </summary>
            /// <returns>The string in snake_case.</returns>
            public string ToSnakeCase()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                string[] words = SplitIntoWords(value);
                return string.Join("_", words).ToLowerInvariant();
            }

            /// <summary>
            /// Converts the string to kebab-case (e.g., "MyPropertyName" → "my-property-name").
            /// Handles PascalCase, camelCase, snake_case, and kebab-case inputs.
            /// </summary>
            /// <returns>The string in kebab-case.</returns>
            public string ToKebabCase()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                string[] words = SplitIntoWords(value);
                return string.Join("-", words).ToLowerInvariant();
            }

            /// <summary>
            /// Converts the string to camelCase (e.g., "MyPropertyName" → "myPropertyName").
            /// Handles PascalCase, camelCase, snake_case, and kebab-case inputs.
            /// </summary>
            /// <returns>The string in camelCase.</returns>
            public string ToCamelCase()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                string[] words = SplitIntoWords(value);
                if (words.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                sb.Append(words[0].ToLowerInvariant());
                for (int i = 1; i < words.Length; i++)
                {
                    if (words[i].Length > 0)
                    {
                        sb.Append(char.ToUpperInvariant(words[i][0]));
                        sb.Append(words[i][1..].ToLowerInvariant());
                    }
                }
                return sb.ToString();
            }

            /// <summary>
            /// Converts the string to a human-readable form by splitting on word boundaries and capitalizing the first letter
            /// (e.g., "MyPropertyName" → "My Property Name", "some_variable" → "Some variable").
            /// </summary>
            /// <returns>The humanized string.</returns>
            public string Humanize()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                string[] words = SplitIntoWords(value);
                if (words.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                sb.Append(char.ToUpperInvariant(words[0][0]));
                sb.Append(words[0][1..].ToLowerInvariant());
                for (int i = 1; i < words.Length; i++)
                {
                    sb.Append(' ');
                    sb.Append(words[i].ToLowerInvariant());
                }
                return sb.ToString();
            }

            /// <summary>
            /// Returns the string with its characters in reverse order.
            /// </summary>
            /// <returns>The reversed string.</returns>
            public string Reverse()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                return string.Create(value.Length, value, static (span, str) =>
                {
                    str.AsSpan().CopyTo(span);
                    span.Reverse();
                });
            }

            /// <summary>
            /// Converts the string to PascalCase (e.g., "my_property_name" → "MyPropertyName").
            /// Handles PascalCase, camelCase, snake_case, and kebab-case inputs.
            /// </summary>
            /// <returns>The string in PascalCase.</returns>
            public string ToPascalCase()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                string[] words = SplitIntoWords(value);
                if (words.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                foreach (string word in words.Where(w => w.Length > 0))
                {
                    sb.Append(char.ToUpperInvariant(word[0]));
                    sb.Append(word[1..].ToLowerInvariant());
                }

                return sb.ToString();
            }

            /// <summary>
            /// Converts the string to Title Case (e.g., "hello world" → "Hello World").
            /// Uses invariant culture casing rules.
            /// </summary>
            /// <returns>The string in Title Case.</returns>
            public string ToTitleCase()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
            }

            /// <summary>
            /// Removes HTML tags from the string, leaving only the text content.
            /// </summary>
            /// <returns>The string with all HTML tags stripped.</returns>
            public string StripHtml()
            {
                if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
                return HtmlTagRegex().Replace(value, string.Empty);
            }

            /// <summary>
            /// Returns the number of whitespace-separated words in the string.
            /// </summary>
            /// <returns>The word count. Returns 0 for null or whitespace-only strings.</returns>
            public int WordCount()
            {
                if (string.IsNullOrWhiteSpace(value)) return 0;
                return value.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
            }

            /// <summary>
            /// Removes diacritical marks (accents) from the string.
            /// </summary>
            /// <returns>The string with accents removed (e.g., "café" → "cafe").</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            public string RemoveDiacritics()
            {
                ArgumentNullException.ThrowIfNull(value);

                string normalized = value.Normalize(System.Text.NormalizationForm.FormD);
                var sb = new StringBuilder(normalized.Length);
                foreach (char c in normalized.Where(c =>
                    System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                        != System.Globalization.UnicodeCategory.NonSpacingMark))
                {
                    sb.Append(c);
                }

                return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
            }

            /// <summary>
            /// Repeats the string the specified number of times.
            /// </summary>
            /// <param name="count">The number of times to repeat. Must be non-negative.</param>
            /// <returns>The repeated string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative.</exception>
            public string Repeat(int count)
            {
                ArgumentNullException.ThrowIfNull(value);
                ArgumentOutOfRangeException.ThrowIfNegative(count);

                if (count == 0 || value.Length == 0) return string.Empty;
                if (count == 1) return value;

                return string.Create(value.Length * count, (value, count), static (span, state) =>
                {
                    ReadOnlySpan<char> src = state.value.AsSpan();
                    for (int i = 0; i < state.count; i++)
                        src.CopyTo(span[(i * src.Length)..]);
                });
            }

            /// <summary>
            /// Ensures the string starts with the specified prefix, prepending it if not already present.
            /// </summary>
            /// <param name="prefix">The prefix to ensure.</param>
            /// <returns>The string guaranteed to start with <paramref name="prefix"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string or <paramref name="prefix"/> is null.</exception>
            public string EnsureStartsWith(string prefix)
            {
                ArgumentNullException.ThrowIfNull(value);
                ArgumentNullException.ThrowIfNull(prefix);
                return value.StartsWith(prefix, StringComparison.Ordinal) ? value : prefix + value;
            }

            /// <summary>
            /// Ensures the string ends with the specified suffix, appending it if not already present.
            /// </summary>
            /// <param name="suffix">The suffix to ensure.</param>
            /// <returns>The string guaranteed to end with <paramref name="suffix"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string or <paramref name="suffix"/> is null.</exception>
            public string EnsureEndsWith(string suffix)
            {
                ArgumentNullException.ThrowIfNull(value);
                ArgumentNullException.ThrowIfNull(suffix);
                return value.EndsWith(suffix, StringComparison.Ordinal) ? value : value + suffix;
            }

            /// <summary>
            /// Replaces multiple substrings in one pass using the provided mapping.
            /// </summary>
            /// <param name="replacements">A dictionary mapping each substring to replace to its replacement.</param>
            /// <returns>The string with all replacements applied.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the string or <paramref name="replacements"/> is null.</exception>
            public string ReplaceMultiple(IReadOnlyDictionary<string, string> replacements)
            {
                ArgumentNullException.ThrowIfNull(value);
                ArgumentNullException.ThrowIfNull(replacements);

                if (replacements.Count == 0 || value.Length == 0) return value;

                string result = value;
                foreach (KeyValuePair<string, string> kv in replacements)
                    result = result.Replace(kv.Key, kv.Value, StringComparison.Ordinal);

                return result;
            }
        }

        extension(string? value)
        {
            /// <summary>
            /// Determines whether the string is <c>null</c>, empty, or consists only of whitespace characters.
            /// Equivalent to <see cref="string.IsNullOrWhiteSpace(string?)"/> as an extension method.
            /// </summary>
            /// <returns><c>true</c> if the string is <c>null</c>, empty, or whitespace; otherwise, <c>false</c>.</returns>
            public bool IsNullOrWhiteSpace() =>
                string.IsNullOrWhiteSpace(value);
        }

        extension(byte[]? data)
        {
            /// <summary>
            /// Converts a UTF-8 encoded byte array back into a string.
            /// </summary>
            /// <returns>The decoded string, or an empty string if the byte array is null or empty.</returns>
            public string FromByteArray() =>
                data == null || data.Length == 0 ? string.Empty : Encoding.UTF8.GetString(data);
        }
    }
}
