namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;

    /// <summary>
    /// Decodes a DLT service for a timezone event.
    /// </summary>
    /// <remarks>
    /// This is not specified in AutoSAR R20-11 or earlier, but is implemented by the Genivi DLT as of
    /// a3c77c3d9bd7523d8dc4f6401109d29f973b01ba.
    /// </remarks>
    public sealed class CustomTimeZoneResponse : ControlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUnregisterContextResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="timeZone">The time zone offset, in seconds.</param>
        /// <param name="isDst">
        /// If <see langword="true"/>, Daylight Savings Time is in effect, otherwise <see langword="false"/>.
        /// </param>
        public CustomTimeZoneResponse(int status, int timeZone, bool isDst)
            : base(status)
        {
            TimeZone = new TimeSpan(timeZone * TimeSpan.TicksPerSecond);
            IsDst = isDst;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUnregisterContextResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        /// <param name="timeZone">The time zone offset, in seconds.</param>
        /// <param name="isDst">
        /// If <see langword="true"/>, Daylight Savings Time is in effect, otherwise <see langword="false"/>.
        /// </param>
        public CustomTimeZoneResponse(int status, TimeSpan timeZone, bool isDst)
            : base(status)
        {
            TimeZone = timeZone;
            IsDst = isDst;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public override int ServiceId
        {
            get { return 0xF03; }
        }

        /// <summary>
        /// Gets the time zone offset from UTC.
        /// </summary>
        /// <value>The time zone offset from UTC.</value>
        public TimeSpan TimeZone { get; }

        /// <summary>
        /// Gets a value indicating whether the time zone offset is Daylight Savings Time.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is DST; otherwise, <see langword="false"/>.</value>
        public bool IsDst { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (!IsDst)
                return string.Format("[timezone {0}] {1}", ToString(Status), ToString(TimeZone));

            return string.Format("[timezone {0}] {1} DST", ToString(Status), ToString(TimeZone));
        }

        private static string ToString(TimeSpan timeSpan)
        {
            if (timeSpan.Ticks < 0)
                return string.Format("-{0:hh\\:mm}", timeSpan);

            return string.Format("+{0:hh\\:mm}", timeSpan);
        }
    }
}
