namespace RJCP.App.DltDump.Domain
{
    using Diagnostics.Log.Dlt;

    /// <summary>
    /// Information about a packet in context.
    /// </summary>
    public readonly struct ContextPacket
    {
        /// <summary>
        /// An empty context packet, that has no line and no data.
        /// </summary>
        public static readonly ContextPacket Empty = new ContextPacket(null);

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextPacket"/> struct.
        /// </summary>
        /// <param name="line">The line.</param>
        public ContextPacket(DltTraceLineBase line) : this(line, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextPacket"/> struct.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="packet">The packet that was used to generate the line.</param>
        public ContextPacket(DltTraceLineBase line, byte[] packet)
        {
            Line = line;
            Packet = packet;
        }

        /// <summary>
        /// Gets the line for this context packet.
        /// </summary>
        /// <value>The line for this context packet.</value>
        public DltTraceLineBase Line { get; }

        /// <summary>
        /// Gets the packet bytes that created line.
        /// </summary>
        /// <value>The packet bytes that created line.</value>
        public byte[] Packet { get; }
    }
}
