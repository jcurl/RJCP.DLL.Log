namespace RJCP.Diagnostics.Log.Dlt
{
    using System;

    internal static class DltConstants
    {
        /// <summary>
        /// The default time stamp when creating a DLT line.
        /// </summary>
        public static readonly DateTime DefaultTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        /// <summary>
        /// The number of bytes (maximum) that the storage header can consume.
        /// </summary>
        public const int MaxStorageHeaderSize = 16;

        /// <summary>
        /// The number of bytes (maximum) for an individual DLT packet.
        /// </summary>
        public const int MaxPacketSize = 65536;

        /// <summary>
        /// The resolution of the time stamp value.
        /// </summary>
        /// <remarks>
        /// According to the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the time resolution
        /// equals 0.1 milliseconds.
        /// </remarks>
        public const long DeviceTimeResolution = TimeSpan.TicksPerMillisecond / 10;

        /// <summary>
        /// The message type (MSTP). This value is part of the Message Info byte (MSIN).
        /// </summary>
        public static class MessageType
        {
            /// <summary>
            /// Message Type mask.
            /// </summary>
            public const int DltTypeMask = 0x0E;

            /// <summary>
            /// The DLT type Log value as encoded in a DLT packet.
            /// </summary>
            public const int DltTypeLog = 0x00;

            /// <summary>
            /// The DLT type Trace as encoded in a DLT packet.
            /// </summary>
            public const int DltTypeAppTrace = 0x02;

            /// <summary>
            /// The DLT type Network as encoded in a DLT packet.
            /// </summary>
            public const int DltTypeNwTrace = 0x04;

            /// <summary>
            /// The DLT type Control as encoded in a DLT packet.
            /// </summary>
            public const int DltTypeControl = 0x06;
        }

        /// <summary>
        /// Bit fields of the Header Type (HTYP) which is part of the standard header.
        /// </summary>
        public static class HeaderType
        {
            /// <summary>
            /// If the extended header is present (UEH).
            /// </summary>
            public const int UseExtendedHeader = 0x01;

            /// <summary>
            /// If the format is most significant byte (MSBF).
            /// </summary>
            public const int MostSignificantByte = 0x02;

            /// <summary>
            /// If the ecu identifier is present (WEID).
            /// </summary>
            public const int WithEcuId = 0x04;

            /// <summary>
            /// If the session identifier is present (WSID).
            /// </summary>
            public const int WithSessionId = 0x08;

            /// <summary>
            /// If the device time stamp is present (WTMS)
            /// </summary>
            public const int WithTimeStamp = 0x10;

            /// <summary>
            /// The version identifier mask (VERS).
            /// </summary>
            public const int VersionIdentifierMask = 0xe0;

            /// <summary>
            /// Indicates the version 1 after the <see cref="VersionIdentifierMask"/>.
            /// </summary>
            public const int Version1 = 0x20;
        }

        /// <summary>
        /// Bit fields of the Header Type (MSIN) which is part of the extended header.
        /// </summary>
        public static class MessageInfo
        {
            /// <summary>
            /// Indicates if this is a verbose or non-verbose message.
            /// </summary>
            public const int Verbose = 0x01;

            /// <summary>
            /// A mask for the message type, mapping to <see cref="DltType"/>.
            /// </summary>
            public const int MessageTypeMask = 0xFE;
        }
    }
}
