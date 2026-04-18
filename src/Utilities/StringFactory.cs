namespace Grondo.Utilities
{
    /// <summary>
    /// Provides factory methods for generating string values.
    /// </summary>
    public static class StringFactory
    {
        private const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";
        private const string AlphabeticChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string NumericChars = "0123456789";
        private const string HexChars = "0123456789abcdef";

        /// <summary>
        /// Generates a unique string identifier using a GUID in "N" format (32 hex digits, no hyphens).
        /// </summary>
        public static string UniqueString => Guid.NewGuid().ToString("N");

        /// <summary>
        /// Creates a random string of the specified length.
        /// This method is thread-safe.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <param name="includeSpecialChars">Whether to include special characters.</param>
        /// <returns>A random string of the specified length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if length is less than or equal to zero.</exception>
        public static string CreateRandomString(int length, bool includeSpecialChars = false)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            var chars = includeSpecialChars ? AllChars : AlphanumericChars;

            return string.Create(length, chars, static (span, chars) =>
            {
                for (int i = 0; i < span.Length; i++)
                {
                    span[i] = chars[Random.Shared.Next(chars.Length)];
                }
            });
        }

        /// <summary>
        /// Creates a GUID string.
        /// </summary>
        /// <returns>A GUID string in standard format.</returns>
        public static string CreateGuid() => Guid.NewGuid().ToString();

        /// <summary>
        /// Creates a random alphabetic string (A-Z, a-z) of the specified length. Thread-safe.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <returns>A random alphabetic string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is less than or equal to zero.</exception>
        public static string CreateAlphabeticString(int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
            return CreateFromPool(length, AlphabeticChars);
        }

        /// <summary>
        /// Creates a random numeric string (digits 0-9) of the specified length. Thread-safe.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <returns>A random numeric string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is less than or equal to zero.</exception>
        public static string CreateNumericString(int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
            return CreateFromPool(length, NumericChars);
        }

        /// <summary>
        /// Creates a random lowercase hexadecimal string of the specified length. Thread-safe.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <returns>A random hex string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is less than or equal to zero.</exception>
        public static string CreateHexString(int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
            return CreateFromPool(length, HexChars);
        }

        private static string CreateFromPool(int length, string pool) =>
            string.Create(length, pool, static (span, chars) =>
            {
                for (int i = 0; i < span.Length; i++)
                    span[i] = chars[Random.Shared.Next(chars.Length)];
            });
    }
}

