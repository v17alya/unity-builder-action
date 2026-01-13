using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Gamenator.Core.UnityBuilder.Utils;

namespace Gamenator.Core.UnityBuilder.Core.Input
{
    /// <summary>
    /// Parses command-line arguments and populates option objects using reflection.
    /// Supports properties decorated with <see cref="OptionAttribute"/>.
    /// </summary>
    public class OptionsParser
    {
        private static readonly string[] s_secrets = {
            "androidKeystorePass", "androidKeyaliasName", "androidKeyaliasPass"
        };

        private readonly Dictionary<string, string> _rawOptions;

        public OptionsParser(string[] args)
        {
            _rawOptions = ExtractRawOptions(args);
        }

        /// <summary>
        /// Parse a single option of type T by its key.
        /// </summary>
        /// <typeparam name="T">The type of the option value.</typeparam>
        /// <param name="key">The option key.</param>
        /// <param name="isRequired">Whether this option is required.</param>
        /// <param name="exitCode">Exit code to use if required option is missing.</param>
        /// <param name="defaultValue">Default value if option is not provided.</param>
        /// <returns>The parsed option value.</returns>
        /// <exception cref="NotSupportedException">Thrown if type T is not supported.</exception>
        public T ParseOption<T>(string key, bool isRequired = false, int exitCode = 0, object defaultValue = null)
        {
            Type t = typeof(T);
            if (t == typeof(string))
                return (T)(object)ArgumentsParser.ParseString(_rawOptions, key, isRequired, exitCode, (string)defaultValue);
            if (t == typeof(int))
                return (T)(object)ArgumentsParser.ParseInt(_rawOptions, key, isRequired, exitCode, (int)defaultValue);
            if (t == typeof(bool))
                return (T)(object)ArgumentsParser.ParseBool(_rawOptions, key);
            if (t.IsEnum)
            {
                var method = typeof(ArgumentsParser)
                    .GetMethod(nameof(ArgumentsParser.ParseEnum))
                    .MakeGenericMethod(t);
                return (T)method.Invoke(null, new object[] { _rawOptions, key, isRequired, exitCode, defaultValue });
            }
            throw new NotSupportedException($"Type '{t.Name}' is not supported");
        }

        /// <summary>
        /// Parse a single option into a specific property of a target object,
        /// identified by a lambda like 'o => o.MyProperty'.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="target">The target object to set the property on.</param>
        /// <param name="propertySelector">Lambda expression selecting the property (e.g., o => o.BuildTarget).</param>
        /// <returns>The parsed property value.</returns>
        /// <exception cref="ArgumentException">Thrown if the expression is not a property access.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the property is missing [Option] attribute.</exception>
        public TProp ParseProperty<TTarget, TProp>(
            TTarget target,
            Expression<Func<TTarget, TProp>> propertySelector)
        {
            // Extract PropertyInfo from the lambda
            if (!(propertySelector.Body is MemberExpression memberExp)
                || !(memberExp.Member is PropertyInfo propInfo))
            {
                throw new ArgumentException("Expression must be a property access", nameof(propertySelector));
            }

            // Get our attribute metadata
            var attr = propInfo.GetCustomAttribute<OptionAttribute>();
            if (attr == null)
                throw new InvalidOperationException($"Property {propInfo.Name} is missing [Option]");

            // Invoke the generic parser to get a TProp value
            TProp parsedValue = ParseOption<TProp>(
                attr.Key,
                attr.IsRequired,
                attr.ExitCode,
                attr.DefaultValue
            );

            // Assign to the target object's property
            propInfo.SetValue(target, parsedValue);

            // Return it immediately
            return parsedValue;
        }

        /// <summary>
        /// Reflect over all [Option] properties on <paramref name="target"/>
        /// and fill them from command-line arguments.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="target">The target object to populate.</param>
        public void ParseInto<T>(T target)
        {
            var allProps = new List<PropertyInfo>();
            var type = target.GetType();

            while (type != null && type != typeof(object))
            {
                var declared = type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(p => p.CanWrite && p.GetCustomAttribute<OptionAttribute>() != null);

                allProps.AddRange(declared);
                type = type.BaseType;
            }

            foreach (var prop in allProps)
            {
                var attr = prop.GetCustomAttribute<OptionAttribute>();
                var method = typeof(OptionsParser)
                    .GetMethod(nameof(ParseOption))
                    .MakeGenericMethod(prop.PropertyType);

                var parsedValue = method.Invoke(this, new object[] { attr.Key, attr.IsRequired, attr.ExitCode, attr.DefaultValue });
                prop.SetValue(target, parsedValue);
            }
        }

        /// <summary>
        /// Extracts raw key-value pairs from command-line arguments.
        /// Supports flags with optional values (e.g., -buildTarget WebGL or -isDevelopmentBuild).
        /// </summary>
        /// <param name="args">Command-line arguments array.</param>
        /// <returns>Dictionary of option keys and values.</returns>
        private Dictionary<string, string> ExtractRawOptions(string[] args)
        {
            var dict = new Dictionary<string, string>();

            Console.WriteLine(
                $"{BuilderUtils.EOL}" +
                $"###########################{BuilderUtils.EOL}" +
                $"#    Parsing settings     #{BuilderUtils.EOL}" +
                $"###########################{BuilderUtils.EOL}" +
                $"{BuilderUtils.EOL}"
            );

            // Extract flags with optional values
            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse flag
                bool isFlag = args[current].StartsWith("-");
                if (!isFlag) continue;
                string flag = args[current].TrimStart('-');

                // Parse optional value
                bool flagHasValue = next < args.Length && !args[next].StartsWith("-");
                string value = flagHasValue ? args[next].TrimStart('-') : "";
                bool secret = s_secrets.Contains(flag);
                string displayValue = secret ? "*HIDDEN*" : "\"" + value + "\"";

                // Assign
                Console.WriteLine($"Found flag \"{flag}\" with value {displayValue}.");
                dict.Add(flag, value);
            }
            return dict;
        }
    }
}
