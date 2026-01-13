using System;
using System.Collections.Generic;
using Gamenator.Core.UnityBuilder.Core.Exceptions;

namespace Gamenator.Core.UnityBuilder.Core.Input
{
    /// <summary>
    /// Static utility class for parsing command-line arguments into typed values.
    /// </summary>
    public static class ArgumentsParser
    {
        /// <summary>
        /// Parses a boolean flag from command-line arguments.
        /// Returns true if the flag is present, false otherwise.
        /// </summary>
        /// <param name="options">Dictionary of parsed command-line options.</param>
        /// <param name="key">The flag key to check.</param>
        /// <returns>True if the flag is present, false otherwise.</returns>
        public static bool ParseBool(Dictionary<string, string> options, string key)
        {
            if (!options.ContainsKey(key))
            {
                Console.WriteLine(GetMissingArgumentErrorString(key));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses a string value from command-line arguments.
        /// </summary>
        /// <param name="options">Dictionary of parsed command-line options.</param>
        /// <param name="key">The option key to parse.</param>
        /// <param name="isRequired">Whether this option is required.</param>
        /// <param name="exitCode">Exit code to use if required option is missing.</param>
        /// <param name="defaultValue">Default value if option is not provided.</param>
        /// <returns>The parsed string value or default value.</returns>
        /// <exception cref="ArgumentMissingException">Thrown if required option is missing.</exception>
        public static string ParseString(Dictionary<string, string> options, string key, bool isRequired, int exitCode, string defaultValue)
        {
            if (!options.TryGetValue(key, out string value) || string.IsNullOrEmpty(value))
            {
                if (isRequired)
                    throw new ArgumentMissingException(GetMissingArgumentErrorString(key), exitCode);

                Console.WriteLine(GetMissingArgumentErrorString(key));
                return defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Parses an integer value from command-line arguments.
        /// </summary>
        /// <param name="options">Dictionary of parsed command-line options.</param>
        /// <param name="key">The option key to parse.</param>
        /// <param name="isRequired">Whether this option is required.</param>
        /// <param name="exitCode">Exit code to use if required option is missing.</param>
        /// <param name="defaultValue">Default value if option is not provided.</param>
        /// <returns>The parsed integer value or default value.</returns>
        /// <exception cref="ArgumentMissingException">Thrown if required option is missing.</exception>
        public static int ParseInt(Dictionary<string, string> options, string key, bool isRequired, int exitCode, int defaultValue)
        {
            if (!options.TryGetValue(key, out string value))
            {
                if (isRequired)
                    throw new ArgumentMissingException(GetMissingArgumentErrorString(key), exitCode);

                Console.WriteLine(GetMissingArgumentErrorString(key));
                return defaultValue;
            }
            return int.Parse(value);
        }

        /// <summary>
        /// Parses an enum value from command-line arguments.
        /// </summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="options">Dictionary of parsed command-line options.</param>
        /// <param name="key">The option key to parse.</param>
        /// <param name="isRequired">Whether this option is required.</param>
        /// <param name="exitCode">Exit code to use if required option is missing.</param>
        /// <param name="defaultValue">Default value if option is not provided.</param>
        /// <returns>The parsed enum value or default value.</returns>
        /// <exception cref="ArgumentMissingException">Thrown if required option is missing or value is invalid.</exception>
        public static T ParseEnum<T>(Dictionary<string, string> options, string key, bool isRequired, int exitCode, T defaultValue)
            where T : struct, IConvertible
        {
            if (!options.TryGetValue(key, out string stringValue))
            {
                if (isRequired)
                    throw new ArgumentMissingException(GetMissingArgumentErrorString(key), exitCode);

                Console.WriteLine(GetMissingArgumentErrorString(key));
                return defaultValue;
            }

            if (!Enum.TryParse(stringValue ?? string.Empty, out T value))
            {
                if (isRequired)
                    throw new ArgumentMissingException($"{stringValue} is not a defined {nameof(T)}", exitCode);

                Console.WriteLine($"{stringValue} is not a defined {nameof(T)}");
                return defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Gets the error message string for a missing argument.
        /// </summary>
        /// <param name="key">The missing argument key.</param>
        /// <returns>Error message string.</returns>
        private static string GetMissingArgumentErrorString(string key)
            => $"Missing argument -{key}";
    }
}
