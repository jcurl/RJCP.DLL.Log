namespace RJCP.Diagnostics.Log.Dlt.Packet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

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
    /// Start building the packet with the <see cref="NewPacket()"/> method, and there define the contents of the line.
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
        public DltPacketBuilder NewPacket()
        {
            return new DltPacketBuilder(this);
        }

        /// <summary>
        /// Adds some random (corrupted) data to the stream.
        /// </summary>
        /// <param name="bytes">The number of random bytes to generate and add.</param>
        /// <returns>The number of bytes that were written.</returns>
        public int Random(int bytes)
        {
            byte[] data = new byte[bytes];
            new Random().NextBytes(data);
            m_Packets.Add(data);
            return bytes;
        }

        /// <summary>
        /// Copies the buffer data to the stream.
        /// </summary>
        /// <param name="buffer">The buffer with the data to copy.</param>
        /// <returns>The number of bytes that were written.</returns>
        public int Data(byte[] buffer)
        {
            byte[] data = new byte[buffer.Length];
            buffer.CopyTo(data, 0);
            m_Packets.Add(data);
            return buffer.Length;
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
        /// Writes the stream to the file name given.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <remarks>
        /// This allows a test case to write the results of a test case to a file for comparison with other decoders.
        /// </remarks>
        public void Write(string fileName)
        {
            using (Stream stream = Stream())
            using (FileStream file = new FileStream(fileName, FileMode.Create)) {
                stream.CopyTo(file);
            }
        }

        /// <summary>
        /// Writes the stream to the file name given.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This allows a test case to write the results of a test case to a file for comparison with other decoders.
        /// </remarks>
        public async Task WriteAsync(string fileName)
        {
            using (Stream stream = Stream())
            using (FileStream file = new FileStream(fileName, FileMode.Create)) {
                await stream.CopyToAsync(file);
            }
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
