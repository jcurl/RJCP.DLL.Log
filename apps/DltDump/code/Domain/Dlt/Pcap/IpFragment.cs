namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;

    /// <summary>
    /// One IPv4 fragment of a complete packet.
    /// </summary>
    public readonly struct IpFragment
    {
        private readonly byte[] m_Buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpFragment"/> struct.
        /// </summary>
        /// <param name="fragOffset">The IPv4 fragment offset.</param>
        /// <param name="buffer">The fragment.</param>
        /// <param name="position">The position in the stream where the fragment starts.</param>
        public IpFragment(int fragOffset, int hcs, ReadOnlySpan<byte> buffer, long position)
        {
            FragmentOffset = fragOffset;
            CheckSum = hcs;
            m_Buffer = buffer.ToArray();
            Position = position;
        }

        /// <summary>
        /// The IPv4 fragment offset.
        /// </summary>
        public readonly int FragmentOffset;

        /// <summary>
        /// The Internet Header Checksum
        /// </summary>
        public readonly int CheckSum;

        /// <summary>
        /// The position where the fragment occurs in the input stream.
        /// </summary>
        public readonly long Position;

        /// <summary>
        /// The buffer of the IPv4 fragment (payload).
        /// </summary>
        public byte[] GetArray()
        {
            return m_Buffer;
        }
    }
}
