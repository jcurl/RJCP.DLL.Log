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
            /// The version identifier value to shift right after masking (VERS).
            /// </summary>
            public const int VersionBitShift = 5;

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
            public const int MessageTypeInfoMask = 0xFE;

            /// <summary>
            /// The message type mask (MSTP).
            /// </summary>
            public const int MessageTypeMaskMstp = 0x0E;

            /// <summary>
            /// The message type info mask (MTIN).
            /// </summary>
            public const int MessageTypeMaskMtin = 0xF0;

            /// <summary>
            /// The message type info mask bit shift.
            /// </summary>
            public const int MessageTypeMaskMtinShift = 4;

            /// <summary>
            /// The message DLT_TYPE_LOG.
            /// </summary>
            public const int MessageTypeLog = 0x00;

            /// <summary>
            /// The message type DLT_TYPE_APP_TRACE.
            /// </summary>
            public const int MessageTypeAppTrace = 0x02;

            /// <summary>
            /// The message type DLT_TYPE_NW_TRACE.
            /// </summary>
            public const int MessageTypeNwTrace = 0x04;

            /// <summary>
            /// The message type DLT_TYPE_CONTROL.
            /// </summary>
            public const int MessageTypeControl = 0x06;
        }

        /// <summary>
        /// Bit fields in the verbose argument Type Info.
        /// </summary>
        public static class TypeInfo
        {
            /// <summary>
            /// The number of bytes the Type Info field requires.
            /// </summary>
            public const int TypeInfoSize = 4;

            /// <summary>
            /// The type length mask to get the length of the verbose argument.
            /// </summary>
            public const int TypeLengthMask = 0xF;

            /// <summary>
            /// The type length is unknown.
            /// </summary>
            public const int TypeLengthUnknown = 0;

            /// <summary>
            /// The type length of 1 byte.
            /// </summary>
            public const int TypeLength8bit = 1;

            /// <summary>
            /// The type length of 2 bytes.
            /// </summary>
            public const int TypeLength16bit = 2;

            /// <summary>
            /// The type length of 4 bytes.
            /// </summary>
            public const int TypeLength32bit = 3;

            /// <summary>
            /// The type length of 8 bytes.
            /// </summary>
            public const int TypeLength64bit = 4;

            /// <summary>
            /// The type length of 16 bytes.
            /// </summary>
            public const int TypeLength128bit = 5;

            /// <summary>
            /// The boolean type.
            /// </summary>
            public const int TypeBool = 0x10;

            /// <summary>
            /// The signed integer type.
            /// </summary>
            public const int TypeSint = 0x20;

            /// <summary>
            /// The unsigned integer type.
            /// </summary>
            public const int TypeUint = 0x40;

            /// <summary>
            /// The float type.
            /// </summary>
            public const int TypeFloat = 0x80;

            /// <summary>
            /// The array type.
            /// </summary>
            public const int TypeArray = 0x100;

            /// <summary>
            /// The string type.
            /// </summary>
            public const int TypeString = 0x200;

            /// <summary>
            /// The raw type.
            /// </summary>
            public const int TypeRaw = 0x400;

            /// <summary>
            /// The variable information bit flag.
            /// </summary>
            public const int VariableInfo = 0x800;

            /// <summary>
            /// The fixed point flag with integer types.
            /// </summary>
            public const int FixedPoint = 0x1000;

            /// <summary>
            /// The trace information type.
            /// </summary>
            public const int TraceInfo = 0x2000;

            /// <summary>
            /// The structure type.
            /// </summary>
            public const int TypeStruct = 0x4000;

            /// <summary>
            /// The coding bit shift to the right where the coding mask starts.
            /// </summary>
            public const int CodingBitShift = 15;

            /// <summary>
            /// The string coding and integer mask before the shift.
            /// </summary>
            public const int CodingMask = 0x38000;
        }
    }
}
