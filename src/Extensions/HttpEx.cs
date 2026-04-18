using System.Text;
using System.Text.Json;
using Grondo.Utilities;
using Microsoft.AspNetCore.Http;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for HTTP-related types such as <see cref="HttpResponseMessage"/> and <see cref="HttpRequest"/>.
    /// </summary>
    public static class HttpEx
    {
        extension(HttpResponseMessage response)
        {
            /// <summary>
            ///     <para>Deserialize a <see cref="HttpResponseMessage"/> into the specified <typeparamref name="TObject"/> type.</para>
            ///     <para>Throws a <see cref="HttpRequestException"/> if the <paramref name="response"/> <see cref="HttpContent"/> is null, empty, or contains only whitespace.</para>
            /// </summary>
            /// <typeparam name="TObject">What to deserialize the response to</typeparam>
            /// <param name="propertyNameCaseInsensitive">Indicates whether a property's name uses a case-insensitive comparison during deserialization.</param>
            /// <returns>The deserialized object, or <c>null</c> if the JSON represents a null value.</returns>
            /// <exception cref="HttpRequestException">Thrown if the response content is null, empty, or whitespace, or if the response status code indicates failure.</exception>
            public async Task<TObject?> EnsureObjectAsync<TObject>(bool propertyNameCaseInsensitive = true)
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    throw new HttpRequestException($"{nameof(response.Content)} is null, empty, or whitespace");
                }

                JsonSerializerOptions options = propertyNameCaseInsensitive ? JsonDefaults.CaseInsensitive : JsonDefaults.Default;

                return JsonSerializer.Deserialize<TObject>(responseBody, options);
            }
        }

        extension(HttpRequest request)
        {
            /// <summary>
            /// Retrieve a raw string from the <see cref="HttpRequest.Body"/> stream.
            /// </summary>
            /// <param name="encoding">Optional character encoding, defaults to UTF8.</param>
            /// <returns>The raw body content as a string.</returns>
            public async Task<string> GetRawBodyAsync(Encoding? encoding = null)
            {
                encoding ??= Encoding.UTF8;

                using StreamReader reader = new(request.Body, encoding);
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// Retrieves a raw byte array from the <see cref="HttpRequest.Body"/> stream.
            /// </summary>
            /// <param name="token">An optional cancellation token.</param>
            /// <returns>The raw body content as a byte array.</returns>
            public async Task<byte[]> GetRawBodyAsync(CancellationToken token = default)
            {
                await using MemoryStream stream = new();
                await request.Body.CopyToAsync(stream, token).ConfigureAwait(false);
                return stream.ToArray();
            }

            /// <summary>
            /// Retrieves a raw byte array from the <see cref="HttpRequest.Body"/> stream with buffering enabled.
            /// </summary>
            /// <param name="cancellationToken">An optional cancellation token.</param>
            /// <returns>The raw body content as a byte array.</returns>
            public async Task<byte[]> GetRawBodyAsBytesAsync(CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(request);

                request.EnableBuffering();
                request.Body.Position = 0;

                await using var ms = new MemoryStream();
                await request.Body.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
                request.Body.Position = 0;

                return ms.ToArray();
            }

            /// <summary>
            /// Gets query parameters as a read-only dictionary.
            /// </summary>
            /// <returns>A dictionary of query parameters.</returns>
            public IReadOnlyDictionary<string, string> GetQueryParams()
            {
                ArgumentNullException.ThrowIfNull(request);

                return request.Query.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Gets form data as a read-only dictionary.
            /// </summary>
            /// <param name="cancellationToken">An optional cancellation token.</param>
            /// <returns>A dictionary of form data.</returns>
            public async Task<IReadOnlyDictionary<string, string>> GetFormDataAsync(
                CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(request);

                if (!request.HasFormContentType)
                    return new Dictionary<string, string>();

                IFormCollection form = await request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
                return form.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Determines whether the request is an AJAX request.
            /// </summary>
            /// <returns><c>true</c> if the request is an AJAX request; otherwise, <c>false</c>.</returns>
            public bool IsAjaxRequest()
            {
                ArgumentNullException.ThrowIfNull(request);

                return request.Headers.XRequestedWith == "XMLHttpRequest";
            }

            /// <summary>
            /// Gets the client IP address, checking X-Forwarded-For header first.
            /// </summary>
            /// <returns>The client IP address, or <c>null</c> if not available.</returns>
            public string? GetClientIpAddress()
            {
                ArgumentNullException.ThrowIfNull(request);

                // Check X-Forwarded-For header first (for proxies/load balancers)
                string? forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    string[] ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    return ips[0].Trim();
                }

                return request.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
        }
    }
}
