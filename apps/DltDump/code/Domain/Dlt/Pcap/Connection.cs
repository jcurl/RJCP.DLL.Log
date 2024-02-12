namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Connection class which represents two IPv4 end points.
    /// </summary>
    public sealed class Connection : IDisposable
    {
        private readonly Dictionary<EndPointKey, IPcapTraceDecoder> m_Decoders = new Dictionary<EndPointKey, IPcapTraceDecoder>();
        private readonly Dictionary<int, IpFragments> m_Fragments = new Dictionary<int, IpFragments>();
        private readonly ITraceDecoderFactory<DltTraceLineBase> m_TraceDecoderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="srcAddr">The IPv4 source address.</param>
        /// <param name="dstAddr">The IPv4 destination address.</param>
        /// <param name="factory">The factory to return a <see cref="IPcapTraceDecoder"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
        public Connection(int srcAddr, int dstAddr, ITraceDecoderFactory<DltTraceLineBase> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);

            SourceAddress = srcAddr;
            DestinationAddress = dstAddr;
            m_TraceDecoderFactory = factory;
        }

        /// <summary>
        /// Gets the IPv4 32-bit source address.
        /// </summary>
        /// <value>The IPv4 32-bit source address.</value>
        public int SourceAddress { get; }

        /// <summary>
        /// Gets the IPv4 32-bit destination address.
        /// </summary>
        /// <value>The IPv4 32-bit destination address.</value>
        public int DestinationAddress { get; }

        /// <summary>
        /// Gets the output stream when instantiating the decoder.
        /// </summary>
        /// <value>
        /// The output stream when instantiating the decoder. May be <see langword="null"/> if it isn't provided.
        /// </value>
        public IOutputStream OutputStream { get; }

        /// <summary>
        /// Gets the DLT decoder for the virtual connection for this endpoint pair.
        /// </summary>
        /// <param name="srcPort">The IPv4 16-bit source port.</param>
        /// <param name="dstPort">The IPv4 16-bit destination port.</param>
        /// <returns>The decoder for this end point if it already exists, or a new decoder.</returns>
        public IPcapTraceDecoder GetDltDecoder(short srcPort, short dstPort)
        {
            EndPointKey key = new EndPointKey(srcPort, dstPort);
            if (!m_Decoders.TryGetValue(key, out IPcapTraceDecoder decoder)) {
                Log.Pcap.TraceEvent(TraceEventType.Information,
                    "New virtual connection {0:x8}:{1:x4} -> {2:x8}:{3:x4}", SourceAddress, srcPort, DestinationAddress, dstPort);

                decoder = (IPcapTraceDecoder)m_TraceDecoderFactory.Create();
                m_Decoders.Add(key, decoder);
            }
            return decoder;
        }

        /// <summary>
        /// Gets the IPv4 Fragments for the fragmentation identifier.
        /// </summary>
        /// <param name="fragmentId">The fragment identifier.</param>
        /// <returns>A collection of fragments, which can be retrieved or added to.</returns>
        public IpFragments GetIpFragments(int fragmentId)
        {
            if (!m_Fragments.TryGetValue(fragmentId, out IpFragments fragments)) {
                fragments = new IpFragments(fragmentId);
                m_Fragments.Add(fragmentId, fragments);
            }
            return fragments;
        }

        /// <summary>
        /// Discards the fragments associated with the fragmentation identifier.
        /// </summary>
        /// <param name="fragmentId">The fragment identifier to discard.</param>
        public void DiscardFragments(int fragmentId)
        {
            m_Fragments.Remove(fragmentId);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed) return;

            foreach (IPcapTraceDecoder decoder in m_Decoders.Values) {
                decoder.Dispose();
            }
            m_IsDisposed = true;
        }
    }
}
