namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;

    /// <summary>
    /// An exception when using an <see cref="IOutputStream"/>.
    /// </summary>
    public class OutputStreamException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputStreamException"/> class.
        /// </summary>
        public OutputStreamException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputStreamException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OutputStreamException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputStreamException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public OutputStreamException(string message, Exception inner) : base(message, inner) { }
    }
}
