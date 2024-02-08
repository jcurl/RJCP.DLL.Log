namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    /// <summary>
    /// An exception when instantiating an <see cref="IInputStream"/> via a <see cref="IInputStreamFactory"/>.
    /// </summary>
    public class InputStreamException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputStreamException"/> class.
        /// </summary>
        public InputStreamException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputStreamException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InputStreamException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputStreamException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public InputStreamException(string message, Exception inner) : base(message, inner) { }
    }
}
