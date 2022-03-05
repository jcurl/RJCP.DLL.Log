namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception when instantiating an <see cref="IInputStream"/> via a <see cref="IInputStreamFactory"/>.
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="InputStreamException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected InputStreamException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
    }
}
