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
    }
}
