namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;

    /// <summary>
    /// The exception that is thrown when an application error occurs.
    /// </summary>
    public class UnknownPcapFileFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownPcapFileFormatException"/> class.
        /// </summary>
        public UnknownPcapFileFormatException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownPcapFileFormatException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnknownPcapFileFormatException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownPcapFileFormatException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownPcapFileFormatException(string message, Exception innerException) : base(message, innerException) { }
    }
}
