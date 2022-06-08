namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A key used for sorting <see cref="DltPcapNetworkTraceFilterDecoder"/> objects in a dictionary within the
    /// <see cref="Connection"/> class.
    /// </summary>
    /// <remarks>It is implemented to enable a fast lookup in a dictionary.</remarks>
    public readonly struct EndPointKey : IEquatable<EndPointKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointKey"/> struct.
        /// </summary>
        /// <param name="srcPort">The IPv4 16-bit source port.</param>
        /// <param name="destPort">The IPv4 16-bit destination port.</param>
        public EndPointKey(short srcPort, short destPort)
        {
            SourcePort = srcPort;
            DestinationPort = destPort;
        }

        /// <summary>
        /// The IPv4 16-bit source port
        /// </summary>
        public readonly short SourcePort;

        /// <summary>
        /// The IPv4 16-bit destination port.
        /// </summary>
        public readonly short DestinationPort;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        [SuppressMessage("Style", "IDE0070:Use 'System.HashCode'", Justification = "This implementation is faster")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "False positive")]
        public override int GetHashCode()
        {
            var hashCode = 43270662;
            hashCode = unchecked(hashCode * -1521134295 + SourcePort.GetHashCode());
            hashCode = unchecked(hashCode * -1521134295 + DestinationPort.GetHashCode());
            return hashCode;
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EndPointKey left, EndPointKey right) => left.Equals(right);

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EndPointKey left, EndPointKey right) => !left.Equals(right);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <see langword="true"/> if the specified <see cref="object"/> is equal to this instance; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is EndPointKey other)
                return SourcePort == other.SourcePort && DestinationPort == other.DestinationPort;

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(EndPointKey other)
        {
            return SourcePort == other.SourcePort && DestinationPort == other.DestinationPort;
        }
    }
}