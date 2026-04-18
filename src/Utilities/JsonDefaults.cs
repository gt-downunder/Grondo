using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grondo.Utilities
{
    /// <summary>
    /// Provides pre-configured, cached <see cref="JsonSerializerOptions"/> instances
    /// for consistent JSON serialization across applications.
    /// </summary>
    /// <remarks>
    /// All instances are marked read-only at construction. Do not mutate the returned options;
    /// create a new <see cref="JsonSerializerOptions"/> instance if a different configuration is required.
    /// </remarks>
    public static class JsonDefaults
    {
        /// <summary>Default options with no special configuration.</summary>
        public static readonly JsonSerializerOptions Default = new();

        /// <summary>Options with case-insensitive property name matching.</summary>
        public static readonly JsonSerializerOptions CaseInsensitive = new() { PropertyNameCaseInsensitive = true };

        /// <summary>Options with indented (pretty-printed) output.</summary>
        public static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

        /// <summary>Options using camelCase property naming (common Web API convention).</summary>
        public static readonly JsonSerializerOptions CamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>Options using snake_case (lowercase) property naming.</summary>
        public static readonly JsonSerializerOptions SnakeCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        /// <summary>Options that omit <c>null</c> values from the output.</summary>
        public static readonly JsonSerializerOptions NoNulls = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        /// <summary>Web-API-friendly options: camelCase, case-insensitive deserialization, omit nulls.</summary>
        public static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
