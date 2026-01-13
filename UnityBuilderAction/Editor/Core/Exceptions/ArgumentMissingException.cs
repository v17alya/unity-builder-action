using System;

namespace Gamenator.Core.UnityBuilder.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a required command-line argument is missing.
    /// </summary>
    public class ArgumentMissingException : Exception
    {
        /// <summary>
        /// Gets the exit code to use when this exception is thrown.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentMissingException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">The exit code to use.</param>
        public ArgumentMissingException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentMissingException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">The exit code to use.</param>
        /// <param name="innerException">The inner exception.</param>
        public ArgumentMissingException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
