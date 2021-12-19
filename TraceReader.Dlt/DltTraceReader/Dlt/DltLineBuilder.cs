namespace RJCP.Diagnostics.Log.Dlt
{
    using System;

    /// <summary>
    /// DLT trace line builder.
    /// </summary>
    /// <remarks>
    /// Use this class for constructing DLT trace lines following the Diagnostic Log and Trace AutoSAR Release 4.2.2
    /// specification.
    /// <para>Allows for the creation of a single DLT trace line at a time.</para>
    /// <para>
    /// This class exposes methods for setting both the control payload and adding arguments for to a DLT verbose
    /// message payload,
    /// </para>
    /// <para>
    /// Each setter method can be used multiple times when a DLT trace line is being constructed. Only the last set
    /// value of a single field shall be used in the construction of the resulting DLT trace line.
    /// </para>
    /// </remarks>
    public class DltLineBuilder : IDltLineBuilder
    {
        private int m_Line = 0;
        private readonly bool m_Online;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltLineBuilder"/> class in offline mode.
        /// </summary>
        public DltLineBuilder() : this(false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltLineBuilder"/> class.
        /// </summary>
        /// <param name="online">
        /// if set to <see langword="true"/> then the <see cref="TimeStamp"/> field is automatically set to the current
        /// time when retrieving the line with <see cref="GetResult()"/>.
        /// </param>
        public DltLineBuilder(bool online)
        {
            m_Online = online;
            Reset();

            // Reset doesn't reconstruct the time stamp. This is done when we create the line, as it can be based on the
            // actual system clock. This allows data skipped to use the last set time stamp.
            ((DltLineFeatures)Features).Reset();
            Features.TimeStamp = m_Online;
            TimeStamp = m_Online ? DateTime.Now : DltConstants.DefaultTimeStamp;
        }

        /// <summary>
        /// Prepare the builder for the construction of a new DLT trace line.
        /// </summary>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Calling this method can result in losing the previously created DLT trace line, unless the DLT trace line is
        /// retrieved by calling the <see cref="GetResult()"/> method.
        /// <para>When calling reset, the following parameters are not reset:</para>
        /// <list type="bullet">
        /// <item><see cref="TimeStamp"/>: It remains the last set time stamp.</item>
        /// <item><see cref="BigEndian"/>: Is not reset, as it is generally constant.</item>
        /// <item>
        /// <see cref="DeviceTimeStamp"/>: Is not reset, but the feature flag is set to <see langword="false"/>. This
        /// means the last set time stamp is used if the <see cref="IDltLineFeatures.DeviceTimeStamp"/> is ignored. The
        /// property must be set to enable the feature flag.
        /// </item>
        /// </list>
        /// </remarks>
        public IDltLineBuilder Reset()
        {
            int features = ((DltLineFeatures)Features).Features & DltLineFeatures.BigEndianFeature;
            ((DltLineFeatures)Features).Set(features);
            Features.TimeStamp = m_Online;

            m_LastEcuId = EcuId;
            EcuId = null;
            ApplicationId = null;
            ContextId = null;
            Count = DltTraceLineBase.InvalidCounter;
            DltType = DltType.UNKNOWN;
            SessionId = 0;
            Position = 0;
            return this;
        }

        /// <summary>
        /// Creates and returns the DLT trace line instance.
        /// </summary>
        /// <returns>The DLT trace line instance.</returns>
        /// <remarks>
        /// This method should be used after all DLT trace line fields have been populated, otherwise the resulting
        /// trace line instance shall be incomplete.
        /// <para>
        /// If this builder is set in online mode (given by <see cref="DltLineBuilder(bool)"/>), the time stamp is set
        /// to the current time stamp when this method is called.
        /// </para>
        /// </remarks>
        public DltTraceLineBase GetResult()
        {
            // We don't support creating Control Messages as of yet.
            if (DltType != DltType.UNKNOWN && ((int)DltType & DltConstants.MessageType.DltTypeControl) != 0) {
                throw new InvalidOperationException("Cannot create standard line from a control type");
            }

            if (m_Online) TimeStamp = DateTime.Now;
            DltTraceLine line = new DltTraceLine() {
                Line = m_Line,
                Position = Position,
                TimeStamp = TimeStamp,
                EcuId = EcuId ?? string.Empty,
                ApplicationId = ApplicationId ?? string.Empty,
                ContextId = ContextId ?? string.Empty,
                SessionId = SessionId,
                Count = Count,
                DeviceTimeStamp = DeviceTimeStamp,
                Type = DltType,
                Text = string.Empty
            };
            ((DltLineFeatures)line.Features).Set((DltLineFeatures)Features);
            m_Line++;

            return line;
        }

        private string m_SkippedReason;
        private DateTime m_LastValidTimeStamp = DltConstants.DefaultTimeStamp;
        private TimeSpan m_LastValidDeviceTimeStamp = new TimeSpan(0);
        private string m_LastEcuId;

        /// <summary>
        /// Indicates that bytes were skipped. Take a snapshot of the time stamps.
        /// </summary>
        /// <param name="bytes">The number of bytes that were skipped.</param>
        /// <param name="reason">The reason why bytes were skipped.</param>
        public void AddSkippedBytes(int bytes, string reason)
        {
            if (bytes <= 0) return;

            if (SkippedBytes == 0) {
                m_SkippedReason = reason;
                m_LastValidTimeStamp = m_Online ? DateTime.Now : TimeStamp;
                m_LastValidDeviceTimeStamp = DeviceTimeStamp;
            }
            SkippedBytes += bytes;
        }

        /// <summary>
        /// Gets the current number of skipped bytes.
        /// </summary>
        /// <value>The number of skipped bytes.</value>
        public long SkippedBytes { get; private set; }

        /// <summary>
        /// Creates and returns the DLT trace line instance expressing skipped data.
        /// </summary>
        /// <returns>
        /// The DLT trace line for skipped bytes. The Skip counter is reset to zero. The line given uses the time stamps
        /// available when the first set of bytes skipped were identified.
        /// </returns>
        public DltTraceLineBase GetSkippedResult()
        {
            // TODO: When we can create lines with arguments, update this method to have constant strings and the
            // reason/bytes as separate arguments.

            if (m_Online) TimeStamp = DateTime.Now;
            DltTraceLine line = new DltTraceLine() {
                Line = m_Line,
                Position = Position,
                TimeStamp = m_LastValidTimeStamp,
                EcuId = m_LastEcuId ?? string.Empty,
                ApplicationId = string.Empty,
                ContextId = string.Empty,
                Count = DltTraceLineBase.InvalidCounter,
                DeviceTimeStamp = m_LastValidDeviceTimeStamp,
                Type = DltType.LOG_WARN,
                Text = m_SkippedReason == null ?
                    $"Skipped: {SkippedBytes}" :
                    $"Skipped: {SkippedBytes} bytes; {m_SkippedReason}"
            };
            line.Features.IsVerbose = true;
            line.Features.DeviceTimeStamp = m_LastValidDeviceTimeStamp.Ticks != 0;
            line.Features.TimeStamp = m_Online || m_LastValidTimeStamp.Ticks != DltConstants.DefaultTimeStamp.Ticks;
            SkippedBytes = 0;
            m_Line++;

            return line;
        }

        /// <summary>
        /// Gets the features which are set for this line.
        /// </summary>
        /// <value>The features set for this line.</value>
        public IDltLineFeatures Features { get; } = new DltLineFeatures();

        /// <summary>
        /// Gets or sets the ECU identifier.
        /// </summary>
        /// <value>The ECU identifier.</value>
        public string EcuId { get; private set; }

        /// <summary>
        /// Sets the ECU identifier.
        /// </summary>
        /// <param name="id">The ECU identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the ECU ID is expected to be no
        /// longer than 4 ASCII characters. Using this method will not set the WEID bit.
        /// </remarks>
        public IDltLineBuilder SetStorageHeaderEcuId(string id)
        {
            if (!Features.EcuId) EcuId = id;
            return this;
        }

        /// <summary>
        /// Sets the ECU identifier.
        /// </summary>
        /// <param name="id">The ECU identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the ECU ID is expected to be no
        /// longer than 4 ASCII characters.
        /// </remarks>
        public IDltLineBuilder SetEcuId(string id)
        {
            EcuId = id;
            Features.EcuId = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Sets the application identifier.
        /// </summary>
        /// <param name="id">The application identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the Application ID is expected
        /// to be no longer than 4 ASCII characters.
        /// </remarks>
        public IDltLineBuilder SetApplicationId(string id)
        {
            ApplicationId = id;
            Features.ApplicationId = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        public string ContextId { get; private set; }

        /// <summary>
        /// Sets the context identifier.
        /// </summary>
        /// <param name="id">The context identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the Context ID is expected to be
        /// no longer than 4 ASCII characters.
        /// </remarks>
        public IDltLineBuilder SetContextId(string id)
        {
            ContextId = id;
            Features.ContextId = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the message counter value.
        /// </summary>
        /// <value>
        /// The message counter value. A value of <see cref="DltTraceLineBase.InvalidCounter"/> indicates that the
        /// counter hasn't been set.
        /// </value>
        public int Count { get; private set; }

        /// <summary>
        /// Sets the message counter value.
        /// </summary>
        /// <param name="value">The message counter value.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetCount(byte value)
        {
            Count = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the device time stamp.
        /// </summary>
        /// <value>The device time stamp.</value>
        public TimeSpan DeviceTimeStamp { get; private set; }

        /// <summary>
        /// Sets the device time stamp.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// The measurement unit being used is the tick, which is equal to 100 nanoseconds or one ten-millionth of a
        /// second. There are 10,000 ticks in a millisecond.
        /// </remarks>
        public IDltLineBuilder SetDeviceTimeStamp(long ticks)
        {
            DeviceTimeStamp = new TimeSpan(ticks);
            Features.DeviceTimeStamp = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the type of the DLT message.
        /// </summary>
        /// <value>The type of the DLT message.</value>
        public DltType DltType { get; private set; }

        /// <summary>
        /// Sets the type and subtype of the DLT trace line.
        /// </summary>
        /// <param name="type">The type and subtype of the DLT trace line.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetDltType(DltType type)
        {
            DltType = type;
            Features.MessageType = true;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is verbose message.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if this instance is verbose; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsVerbose { get { return Features.IsVerbose; } }

        /// <summary>
        /// Set the trace line to verbose or non-verbose mode.
        /// </summary>
        /// <param name="value">Boolean value indicating if the DLT trace line is in verbose or non-verbose mode.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetIsVerbose(bool value)
        {
            Features.IsVerbose = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public int SessionId { get; private set; }

        /// <summary>
        /// Sets the session identifier.
        /// </summary>
        /// <param name="id">The session identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the Session ID field shall be a
        /// 32-bit unsigned integer.
        /// </remarks>
        public IDltLineBuilder SetSessionId(int id)
        {
            SessionId = id;
            Features.SessionId = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp of the device.</value>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Sets the time stamp.
        /// </summary>
        /// <param name="seconds">The number of seconds since 01.01.1970 (UNIX time).</param>
        /// <param name="microseconds">The number of microseconds of the second (between 0 and 1.000.000).</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetTimeStamp(int seconds, int microseconds)
        {
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(seconds)
                .AddTicks(microseconds * (TimeSpan.TicksPerMillisecond / 1000))
                .UtcDateTime;
            Features.TimeStamp = true;
            return this;
        }

        /// <summary>
        /// Sets the time stamp.
        /// </summary>
        /// <param name="dateTime">The logging time stamp.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetTimeStamp(DateTime dateTime)
        {
            TimeStamp = dateTime;
            Features.TimeStamp = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        /// <value>The position in the stream.</value>
        public long Position { get; private set; }

        /// <summary>
        /// Sets the position of the stream where the line is decoded.
        /// </summary>
        /// <param name="position">The position in the stream where the line is decoded.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetPosition(long position)
        {
            Position = position;
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether arguments are encoded as big endian.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if arguments are encoded big endian; otherwise, <see langword="false"/>.
        /// </value>
        public bool BigEndian { get { return Features.BigEndian; } }

        /// <summary>
        /// Sets the if the arguments of the trace line are encoded using big endian.
        /// </summary>
        /// <param name="bigEndian">
        /// if set to <see langword="true"/> then arguments are encoded as big endian, else if <see langword="false"/>,
        /// they're encoded as little endian.
        /// </param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        public IDltLineBuilder SetBigEndian(bool bigEndian)
        {
            Features.BigEndian = bigEndian;
            return this;
        }
    }
}
