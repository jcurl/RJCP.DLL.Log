namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Legacy
{
    using System;
    using Resources;
    using RJCP.Core;

    /// <summary>
    /// Decodes the PCAP legacy header.
    /// </summary>
    public class PcapFormat
    {
        public const int HeaderLength = 24;

        /// <summary>
        /// The units for time resolution in the PCAP header.
        /// </summary>
        public enum TimeResolution
        {
            Microseconds,
            Nanoseconds
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapFormat"/> class.
        /// </summary>
        /// <param name="buffer">The PCAP header, which must be 24 bytes.</param>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> is not 24 bytes in length.</exception>
        /// <exception cref="UnknownPcapFileFormatException">
        /// Unknown buffer header (magic bytes are unknown).
        /// <para>- or -</para>
        /// Unsupported PCAP version
        /// <para>- or -</para>
        /// PCAP contains invalid data (the Snap Length is zero).
        /// <para>- or -</para>
        /// Invalid link format (reserved bits are set, which are an error, or the link type is not supported).
        /// </exception>
        public PcapFormat(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < HeaderLength) throw new ArgumentException(AppResources.ArgErrorBufferTooSmall, nameof(buffer));

            int magic = BitOperations.To32ShiftBigEndian(buffer);
            if (magic == unchecked((int)0xD4C3B2A1)) {
                IsLittleEndian = true;
                Resolution = TimeResolution.Microseconds;
            } else if (magic == 0x4D3CB2A1) {
                IsLittleEndian = true;
                Resolution = TimeResolution.Nanoseconds;
            } else if (magic == unchecked((int)0xA1B2C3D4)) {
                IsLittleEndian = false;
                Resolution = TimeResolution.Microseconds;
            } else if (magic == unchecked((int)0xA1B23C4D)) {
                IsLittleEndian = false;
                Resolution = TimeResolution.Nanoseconds;
            } else {
                string message = string.Format(AppResources.DomainPcapUnknownBlockType, magic);
                throw new UnknownPcapFileFormatException(message);
            }

            MajorVersion = BitOperations.To16Shift(buffer[4..], IsLittleEndian);
            MinorVersion = BitOperations.To16Shift(buffer[6..], IsLittleEndian);
            if (MajorVersion != 2 || MinorVersion != 4) {
                string message = string.Format(AppResources.DomainPcapUnsupportedVersion, MajorVersion, MinorVersion);
                throw new UnknownPcapFileFormatException(message);
            }

            SnapLen = BitOperations.To32Shift(buffer[16..], IsLittleEndian);
            if (SnapLen == 0) {
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapInvalidData);
            }

            int linkFcs = BitOperations.To32Shift(buffer[20..], IsLittleEndian);
            bool r = (linkFcs & 0x0BFF0000) != 0;
            if (r) {
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }

            bool p = (linkFcs & 0x04000000) != 0;
            if (p) {
                FcsLen = (linkFcs >> 28) * 2;
            }

            LinkType = linkFcs & 0xFFFF;
            switch (LinkType) {
            case LinkTypes.LINKTYPE_ETHERNET:
            case LinkTypes.LINKTYPE_LINUX_SLL:
                break;
            default:
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }
        }

        /// <summary>
        /// Gets a value indicating the endianness of the packet data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if data is encoded as little endian; otherwise, <see langword="false"/> if data is
        /// encoded as big endian.
        /// </value>
        public bool IsLittleEndian { get; }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>The major version.</value>
        public int MajorVersion { get; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>The minor version.</value>
        public int MinorVersion { get; }

        /// <summary>
        /// Gets the snap length, which is the maximum length of each packet.
        /// </summary>
        /// <value>The snap length of each packet.</value>
        public int SnapLen { get; }

        /// <summary>
        /// Gets the length of the FCS (frame checksum).
        /// </summary>
        /// <value>The length of the FCS (frame checksum).</value>
        public int FcsLen { get; }

        /// <summary>
        /// Gets the type of the link.
        /// </summary>
        /// <value>The type of the link (see <see cref="LinkTypes"/>.</value>
        public int LinkType { get; }

        /// <summary>
        /// Gets the time resolution for each packet.
        /// </summary>
        /// <value>The time resolution for each packet.</value>
        public TimeResolution Resolution { get; }

        /// <summary>
        /// Gets the time stamp from a PCAP header.
        /// </summary>
        /// <param name="seconds">The number of seconds since 1st January 1970.</param>
        /// <param name="subsec">The number of partial seconds, in units of <see cref="Resolution"/>.</param>
        /// <returns>A <see cref="DateTime"/> converted.</returns>
        public DateTime GetTimeStamp(uint seconds, uint subsec)
        {
            DateTimeOffset timeStamp;
            if (Resolution == TimeResolution.Microseconds) {
                timeStamp = DateTimeOffset.FromUnixTimeSeconds(seconds)
                    .AddTicks(subsec * TimeSpan.TicksPerSecond / 1000000);
            } else {
                timeStamp = DateTimeOffset.FromUnixTimeSeconds(seconds)
                    .AddTicks(subsec * TimeSpan.TicksPerSecond / 1000000000);
            }

            return timeStamp.UtcDateTime;
        }

        private string m_Format;

        /// <summary>
        /// Provides a string value that represents this structure.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            m_Format ??= string.Format("PCAP file ({0}), version {1}.{2}. Link Type = {3}. SnapLen = {4}. FCS Length = {5}. Resolution = {6}",
                IsLittleEndian ? "little endian" : "big endian",
                MajorVersion, MinorVersion,
                LinkType, SnapLen, FcsLen,
                Resolution);
            return m_Format;
        }
    }
}
