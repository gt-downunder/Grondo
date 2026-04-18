using System.Text.RegularExpressions;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="Regex"/> patterns over strings.
    /// </summary>
    public static class RegexEx
    {
        extension(string input)
        {
            /// <summary>
            /// Determines whether the input matches the specified regex pattern.
            /// </summary>
            /// <param name="pattern">The regex pattern to test against.</param>
            /// <param name="options">The regex options. Defaults to <see cref="RegexOptions.None"/>.</param>
            /// <returns><c>true</c> if the input matches; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the input or <paramref name="pattern"/> is null.</exception>
            public bool RegexIsMatch(string pattern, RegexOptions options = RegexOptions.None)
            {
                ArgumentNullException.ThrowIfNull(input);
                ArgumentNullException.ThrowIfNull(pattern);
                return Regex.IsMatch(input, pattern, options);
            }

            /// <summary>
            /// Returns the first match of the specified regex pattern in the input, or <c>null</c> if no match is found.
            /// </summary>
            /// <param name="pattern">The regex pattern to search for.</param>
            /// <param name="options">The regex options. Defaults to <see cref="RegexOptions.None"/>.</param>
            /// <returns>The matched string, or <c>null</c> if no match.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the input or <paramref name="pattern"/> is null.</exception>
            public string? RegexMatch(string pattern, RegexOptions options = RegexOptions.None)
            {
                ArgumentNullException.ThrowIfNull(input);
                ArgumentNullException.ThrowIfNull(pattern);

                Match match = Regex.Match(input, pattern, options);
                return match.Success ? match.Value : null;
            }

            /// <summary>
            /// Returns all matches of the specified regex pattern in the input.
            /// </summary>
            /// <param name="pattern">The regex pattern to search for.</param>
            /// <param name="options">The regex options. Defaults to <see cref="RegexOptions.None"/>.</param>
            /// <returns>An array of matched strings.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the input or <paramref name="pattern"/> is null.</exception>
            public string[] RegexMatches(string pattern, RegexOptions options = RegexOptions.None)
            {
                ArgumentNullException.ThrowIfNull(input);
                ArgumentNullException.ThrowIfNull(pattern);

                MatchCollection matches = Regex.Matches(input, pattern, options);
                string[] result = new string[matches.Count];
                for (int i = 0; i < matches.Count; i++)
                    result[i] = matches[i].Value;
                return result;
            }

            /// <summary>
            /// Replaces all occurrences of the specified regex pattern using the provided evaluator function.
            /// </summary>
            /// <param name="pattern">The regex pattern to search for.</param>
            /// <param name="evaluator">A function that produces the replacement string for each match.</param>
            /// <param name="options">The regex options. Defaults to <see cref="RegexOptions.None"/>.</param>
            /// <returns>The input string with all matches replaced.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the input, <paramref name="pattern"/>, or <paramref name="evaluator"/> is null.</exception>
            public string RegexReplace(string pattern, Func<Match, string> evaluator, RegexOptions options = RegexOptions.None)
            {
                ArgumentNullException.ThrowIfNull(input);
                ArgumentNullException.ThrowIfNull(pattern);
                ArgumentNullException.ThrowIfNull(evaluator);
                return Regex.Replace(input, pattern, new MatchEvaluator(evaluator), options);
            }

            /// <summary>
            /// Replaces all occurrences of the specified regex pattern with the replacement string.
            /// </summary>
            /// <param name="pattern">The regex pattern to search for.</param>
            /// <param name="replacement">The replacement string.</param>
            /// <param name="options">The regex options. Defaults to <see cref="RegexOptions.None"/>.</param>
            /// <returns>The input string with all matches replaced.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the input, <paramref name="pattern"/>, or <paramref name="replacement"/> is null.</exception>
            public string RegexReplace(string pattern, string replacement, RegexOptions options = RegexOptions.None)
            {
                ArgumentNullException.ThrowIfNull(input);
                ArgumentNullException.ThrowIfNull(pattern);
                ArgumentNullException.ThrowIfNull(replacement);
                return Regex.Replace(input, pattern, replacement, options);
            }
        }
    }
}
