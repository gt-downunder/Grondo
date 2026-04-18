using System.Text;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Uri"/> inspection, manipulation, and diagnostics.
    /// </summary>
    public static class UriEx
    {
        extension(Uri uri)
        {
            /// <summary>
            /// Returns a list of string representations of all key Uri properties.
            /// Intended for debugging/logging.
            /// </summary>
            /// <returns>A read-only list of formatted URI property strings.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the URI is <c>null</c>.</exception>
            public IReadOnlyList<string> DumpProperties()
            {
                ArgumentNullException.ThrowIfNull(uri);

                var properties = new List<string>
                {
                    $"AbsolutePath: {uri.AbsolutePath}",
                    $"AbsoluteUri: {uri.AbsoluteUri}",
                    $"Authority: {uri.Authority}",
                    $"DnsSafeHost: {uri.DnsSafeHost}",
                    $"Fragment: {uri.Fragment}",
                    $"Host: {uri.Host}",
                    $"HostNameType: {uri.HostNameType}",
                    $"IsAbsoluteUri: {uri.IsAbsoluteUri}",
                    $"IsDefaultPort: {uri.IsDefaultPort}",
                    $"IsFile: {uri.IsFile}",
                    $"IsLoopback: {uri.IsLoopback}",
                    $"IsUnc: {uri.IsUnc}",
                    $"LocalPath: {uri.LocalPath}",
                    $"OriginalString: {uri.OriginalString}",
                    $"PathAndQuery: {uri.PathAndQuery}",
                    $"Port: {uri.Port}",
                    $"Query: {uri.Query}",
                    $"Scheme: {uri.Scheme}"
                };

                properties.AddRange(uri.Segments.Select((seg, i) => $"Segment{i}: {seg}"));

                properties.Add($"UserEscaped: {uri.UserEscaped}");
                properties.Add($"UserInfo: {uri.UserInfo}");

                return properties;
            }

            /// <summary>
            /// Returns a new <see cref="Uri"/> with the specified path segment appended, preserving the query and fragment.
            /// </summary>
            /// <param name="segment">The path segment to append. Leading and trailing slashes are handled.</param>
            /// <returns>A new <see cref="Uri"/> with the segment appended.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the URI or <paramref name="segment"/> is null.</exception>
            public Uri AppendPath(string segment)
            {
                ArgumentNullException.ThrowIfNull(uri);
                ArgumentNullException.ThrowIfNull(segment);

                var builder = new UriBuilder(uri);
                string path = builder.Path;
                bool pathEndsWithSlash = path.EndsWith('/');
                bool segmentStartsWithSlash = segment.StartsWith('/');
                builder.Path = (pathEndsWithSlash, segmentStartsWithSlash) switch
                {
                    (true, true) => path + segment[1..],
                    (false, false) => path + "/" + segment,
                    _ => path + segment,
                };
                return builder.Uri;
            }

            /// <summary>
            /// Parses the URI's query string into a dictionary. Duplicate keys are overwritten by the last occurrence.
            /// </summary>
            /// <returns>A case-sensitive dictionary of query parameters. Empty if the URI has no query.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the URI is null.</exception>
            public IReadOnlyDictionary<string, string> GetQueryParameters()
            {
                ArgumentNullException.ThrowIfNull(uri);

                var result = new Dictionary<string, string>(StringComparer.Ordinal);
                string query = uri.IsAbsoluteUri ? uri.Query : new Uri("http://x" + uri.OriginalString, UriKind.Absolute).Query;
                if (string.IsNullOrEmpty(query)) return result;

                ReadOnlySpan<char> span = query.AsSpan(query[0] == '?' ? 1 : 0);
                while (!span.IsEmpty)
                {
                    int amp = span.IndexOf('&');
                    ReadOnlySpan<char> pair = amp < 0 ? span : span[..amp];
                    span = amp < 0 ? [] : span[(amp + 1)..];

                    int eq = pair.IndexOf('=');
                    string key = Uri.UnescapeDataString((eq < 0 ? pair : pair[..eq]).ToString());
                    string value = eq < 0 ? string.Empty : Uri.UnescapeDataString(pair[(eq + 1)..].ToString());
                    if (key.Length > 0) result[key] = value;
                }

                return result;
            }

            /// <summary>
            /// Returns a new <see cref="Uri"/> with the specified query parameter added or replaced.
            /// </summary>
            /// <param name="name">The query parameter name.</param>
            /// <param name="value">The query parameter value.</param>
            /// <returns>A new <see cref="Uri"/> with the updated query string.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the URI, <paramref name="name"/>, or <paramref name="value"/> is null.</exception>
            public Uri WithQueryParameter(string name, string value)
            {
                ArgumentNullException.ThrowIfNull(uri);
                ArgumentNullException.ThrowIfNull(name);
                ArgumentNullException.ThrowIfNull(value);

                var parameters = new Dictionary<string, string>(uri.GetQueryParameters(), StringComparer.Ordinal)
                {
                    [name] = value
                };

                var sb = new StringBuilder();
                bool first = true;
                foreach (KeyValuePair<string, string> kv in parameters)
                {
                    if (!first) sb.Append('&');
                    first = false;
                    sb.Append(Uri.EscapeDataString(kv.Key));
                    sb.Append('=');
                    sb.Append(Uri.EscapeDataString(kv.Value));
                }

                var builder = new UriBuilder(uri) { Query = sb.ToString() };
                return builder.Uri;
            }
        }
    }
}
