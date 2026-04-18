using Microsoft.Extensions.Configuration;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IConfiguration"/>.
    /// </summary>
    public static class ConfigurationEx
    {
        extension(IConfiguration configuration)
        {
            /// <summary>
            /// Gets the value of the specified configuration key.
            /// Throws <see cref="InvalidOperationException"/> if the key is missing or the value is empty.
            /// </summary>
            /// <param name="key">The configuration key.</param>
            /// <returns>The non-empty configuration value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the configuration is <c>null</c>.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the key is missing or the value is null/empty.</exception>
            public string GetRequiredValue(string key)
            {
                ArgumentNullException.ThrowIfNull(configuration);
                ArgumentException.ThrowIfNullOrWhiteSpace(key);

                string? value = configuration[key];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"Configuration key '{key}' is missing or empty.");
                }

                return value;
            }

            /// <summary>
            /// Gets the value of the specified configuration key, or returns a default value if the key is missing.
            /// </summary>
            /// <param name="key">The configuration key.</param>
            /// <param name="defaultValue">The default value to return if the key is missing or empty.</param>
            /// <returns>The configuration value, or <paramref name="defaultValue"/> if not found.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the configuration is <c>null</c>.</exception>
            public string GetValueOrDefault(string key, string defaultValue = "")
            {
                ArgumentNullException.ThrowIfNull(configuration);
                ArgumentException.ThrowIfNullOrWhiteSpace(key);

                string? value = configuration[key];
                return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            }

            /// <summary>
            /// Determines whether the specified configuration key exists and has a non-empty value.
            /// </summary>
            /// <param name="key">The configuration key to check.</param>
            /// <returns><c>true</c> if the key exists and has a non-empty value; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the configuration is <c>null</c>.</exception>
            public bool HasKey(string key)
            {
                ArgumentNullException.ThrowIfNull(configuration);
                ArgumentException.ThrowIfNullOrWhiteSpace(key);

                return !string.IsNullOrWhiteSpace(configuration[key]);
            }

            /// <summary>
            /// Gets a typed configuration value with a default.
            /// </summary>
            /// <typeparam name="T">The type to convert the value to.</typeparam>
            /// <param name="key">The configuration key.</param>
            /// <param name="defaultValue">The default value if the key is missing or conversion fails.</param>
            /// <returns>The typed configuration value, or the default value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the configuration is <c>null</c>.</exception>
            public T GetValue<T>(string key, T defaultValue = default!)
            {
                ArgumentNullException.ThrowIfNull(configuration);
                ArgumentException.ThrowIfNullOrWhiteSpace(key);

                string? value = configuration[key];
                if (string.IsNullOrWhiteSpace(value))
                    return defaultValue;

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (InvalidCastException)
                {
                    return defaultValue;
                }
                catch (FormatException)
                {
                    return defaultValue;
                }
                catch (OverflowException)
                {
                    return defaultValue;
                }
            }

            /// <summary>
            /// Gets a required typed configuration value.
            /// </summary>
            /// <typeparam name="T">The type to convert the value to.</typeparam>
            /// <param name="key">The configuration key.</param>
            /// <returns>The typed configuration value.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the configuration is <c>null</c>.</exception>
            /// <exception cref="InvalidOperationException">Thrown if the key is missing or conversion fails.</exception>
            public T GetRequiredValue<T>(string key)
            {
                ArgumentNullException.ThrowIfNull(configuration);
                ArgumentException.ThrowIfNullOrWhiteSpace(key);

                string? value = configuration[key];
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException($"Configuration key '{key}' is missing or empty.");

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Cannot convert configuration key '{key}' to type {typeof(T).Name}",
                        ex);
                }
            }

            /// <summary>
            /// Binds a configuration section to a strongly-typed object.
            /// </summary>
            /// <typeparam name="T">The type to bind to.</typeparam>
            /// <param name="sectionName">The name of the configuration section.</param>
            /// <returns>An instance of T with values bound from the configuration section.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the configuration is <c>null</c>.</exception>
            public T GetSection<T>(string sectionName) where T : new()
            {
                ArgumentNullException.ThrowIfNull(configuration);
                ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

                IConfigurationSection section = configuration.GetSection(sectionName);
                var instance = new T();
                section.Bind(instance);
                return instance;
            }
        }
    }
}

