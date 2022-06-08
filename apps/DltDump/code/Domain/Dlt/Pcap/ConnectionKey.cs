namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;

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
        public override int GetHashCode()
        {
            var hashCode = 43270662;
            hashCode = hashCode * -1521134295 + SourceAddress.GetHashCode();
            hashCode = hashCode * -1521134295 + DestinationAddress.GetHashCode();
            return hashCode;
        }

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