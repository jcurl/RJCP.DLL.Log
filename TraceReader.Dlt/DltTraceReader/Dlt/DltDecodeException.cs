﻿namespace RJCP.Diagnostics.Log.Dlt
{
    using System;

    /// <summary>
    /// Indicates an error in the data being parsed.
    /// </summary>
    public class DltDecodeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltDecodeException"/> class.
        /// </summary>
        public DltDecodeException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltDecodeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DltDecodeException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltDecodeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DltDecodeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
