namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// Describes fields on a DLT trace line that may be optional.
    /// </summary>
    public interface IDltLineFeatures
    {
        /// <summary>
        /// Indicates if this line has a valid ECU identifier.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.EcuId"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool EcuId { get; set; }

        /// <summary>
        /// Indicates if this line has a valid time stamp from the device.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.DeviceTimeStamp"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool DeviceTimeStamp { get; set; }

        /// <summary>
        /// Indicates if this line has a application identifier.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.ApplicationId"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool ApplicationId { get; set; }

        /// <summary>
        /// Indicates if this line has a valid context identifier.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.ContextId"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool ContextId { get; set; }

        /// <summary>
        /// Indicates if this line has a valid message type.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.Type"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool MessageType { get; set; }

        /// <summary>
        /// Indicates if this line has a valid session identifier.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.SessionId"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool SessionId { get; set; }

        /// <summary>
        /// Indicates if this line has a valid time stamp from the logger.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if the <see cref="DltTraceLineBase.TimeStamp"/> is valid, otherwise
        /// <see langword="false"/>.
        /// </value>
        bool TimeStamp { get; set; }

        /// <summary>
        /// Indicates if the packet format is in little endian ( <see langword="false"/>), or big endian (
        /// <see langword="true"/>).
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if big endian, otherwise, <see langword="false"/> for little endian.
        /// </value>
        bool BigEndian { get; set; }

        /// <summary>
        /// Indicates if this message is a verbose message, or a non-verbose message.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if this instance is a verbose message; otherwise, <see langword="false"/> if
        /// it is a non-verbose (or control) message.
        /// </value>
        bool IsVerbose { get; set; }
    }
}
