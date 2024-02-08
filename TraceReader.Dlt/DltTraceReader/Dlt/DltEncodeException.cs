namespace RJCP.Diagnostics.Log.Dlt
{
    using System;

    /// <summary>
    /// Indicates an error in the data being parsed.
    /// </summary>
    public class DltEncodeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltEncodeException"/> class.
        /// </summary>
        public DltEncodeException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltEncodeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DltEncodeException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltEncodeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DltEncodeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
