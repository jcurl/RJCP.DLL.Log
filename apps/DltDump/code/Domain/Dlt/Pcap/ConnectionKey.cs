namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A key used for sorting <see cref="Connection"/> objects in a dictionary.
    /// </summary>
    /// <remarks>
    /// It is implemented to enable a fast lookup in a dictionary.
    /// </remarks>
    public readonly struct ConnectionKey : IEquatable<ConnectionKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionKey"/> struct.
        /// </summary>
        /// <param name="srcAddr">The IPv4 32-bit source address.</param>
        /// <param name="destAddr">The IPv4 32-bit destination address.</param>
        public ConnectionKey(int srcAddr, int destAddr)
        {
            SourceAddress = srcAddr;
            DestinationAddress = destAddr;
        }

        /// <summary>
        /// The IPv4 32-bit source address.
        /// </summary>
        public readonly int SourceAddress;

        /// <summary>
        /// The IPv4 32-bit destination address
        /// </summary>
        public readonly int DestinationAddress;

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
            hashCode = unchecked(hashCode * -1521134295 + SourceAddress.GetHashCode());
            hashCode = unchecked(hashCode * -1521134295 + DestinationAddress.GetHashCode());
            return hashCode;
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ConnectionKey left, ConnectionKey right) => left.Equals(right);

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ConnectionKey left, ConnectionKey right) => !left.Equals(right);

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
            if (obj is ConnectionKey other)
                return SourceAddress == other.SourceAddress && DestinationAddress == other.DestinationAddress;

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
        public bool Equals(ConnectionKey other)
        {
            return SourceAddress == other.SourceAddress && DestinationAddress == other.DestinationAddress;
        }
    }
}