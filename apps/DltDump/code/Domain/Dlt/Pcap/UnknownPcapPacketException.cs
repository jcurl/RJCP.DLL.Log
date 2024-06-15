namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;

    /// <summary>
    /// The exception that is thrown when trying to process an unknown PCAP packet.
    /// </summary>
    public class UnknownPcapPacketException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownPcapPacketException"/> class.
        /// </summary>
        public UnknownPcapPacketException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownPcapPacketException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnknownPcapPacketException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownPcapPacketException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownPcapPacketException(string message, Exception innerException) : base(message, innerException) { }
    }
}
