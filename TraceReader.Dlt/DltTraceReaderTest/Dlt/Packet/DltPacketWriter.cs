namespace RJCP.Diagnostics.Log.Dlt.Packet
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Support class that knows how to build DLT Packets.
    /// </summary>
    /// <remarks>
    /// This class is used to build packets in memory, useful for testing. It uses a fluent style for packet
    /// construction to make test case construction easier to read.
    /// <para>
    /// Set the properties for the packet with <see cref="EcuId"/>, <see cref="AppId"/>, <see cref="CtxId"/>,
    /// <see cref="SessionId"/>.
    /// </para>
    /// <para>
    /// Start building the packet with the <see cref="Verbose()"/> method, and there define the contents of the line.
    /// </para>
    /// <para>
    /// Each individual packet can be built, or it can be appended to an internal list, which can then be used as a
    /// stream with the <see cref="Stream()"/>, that represents a valid DLT stream.
    /// </para>
    /// </remarks>
    public sealed partial class DltPacketWriter : IDisposable
    {
        private const int PacketSize = 65536 + 16;
        private readonly List<byte[]> m_Packets = new List<byte[]>();

        /// <summary>
        /// Gets or sets the ecu identifier that should be generated.
        /// </summary>
        /// <value>The ECU identifier. Use <see langword="null"/> to have no ECU identifier.</value>
        public string EcuId { get; set; }

        /// <summary>
        /// Gets or sets the application identifier that should be generated.
        /// </summary>
        /// <value>The application identifier. This is always present.</value>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the context identifier that should be generated.
        /// </summary>
        /// <value>The context identifier. This is always present.</value>
        public string CtxId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier that should be generated.
        /// </summary>
        /// <value>The session identifier. This is always present.</value>
        /// <remarks>
        /// The DLT specification does maintain that this is an optional field.
        /// </remarks>
        public int SessionId { get; set; }

        /// <summary>
        /// Gets or sets the counter for the generated packet.
        /// </summary>
        /// <value>The counter. It increments automatically when packets are generated.</value>
        public byte Counter { get; set; }

        /// <summary>
        /// Constructs a new verbose DLT packet.
        /// </summary>
        /// <returns>The object to build a verbose DLT packet.</returns>
        public DltVerbosePacketBuilder Verbose()
        {
            return new DltVerbosePacketBuilder(this);
        }

        /// <summary>
        /// Returns a stream that contains all the appended packets.
        /// </summary>
        /// <returns>A <see cref="System.IO.Stream"/> that can be read</returns>
        public Stream Stream()
        {
            MemoryStream stream = new MemoryStream();
            foreach (byte[] packet in m_Packets) {
                stream.Write(packet, 0, packet.Length);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            m_Packets.Clear();
        }
    }
}
