using System;

namespace Gamenator.Core.UnityBuilder.Core.Input
{
    /// <summary>
    /// Attribute to declare how a property should be parsed from command-line arguments.
    /// Apply this attribute to properties in option classes to enable automatic parsing.
    /// </summary>
    /// <example>
    /// <code>
    /// public class OptionsWebGL : OptionsBase
    /// {
    ///     [Option("codeOptimization", true, 140, WasmCodeOptimization.RuntimeSpeed)]
    ///     public WasmCodeOptimization WasmCodeOptimization { get; private set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the command-line argument key (e.g., "buildTarget" for -buildTarget WebGL).
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets whether this option is required.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Gets the exit code to use if a required option is missing.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Gets the default value to use if the option is not provided.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="key">The command-line argument key.</param>
        /// <param name="isRequired">Whether this option is required.</param>
        /// <param name="exitCode">Exit code to use if required option is missing.</param>
        /// <param name="defaultValue">Default value if option is not provided.</param>
        public OptionAttribute(string key, bool isRequired = false, int exitCode = 0, object defaultValue = null)
        {
            Key = key;
            IsRequired = isRequired;
            ExitCode = exitCode;
            DefaultValue = defaultValue;
        }
    }
}
