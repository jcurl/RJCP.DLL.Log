namespace RJCP.Diagnostics.Log.Dlt
{
    using System;
    using System.Globalization;

    /// <summary>
    /// DLT trace line base class.
    /// </summary>
    /// <remarks>
    /// The members in this class have been defined following the Diagnostic Log and Trace AUTOSAR Release 4.2.2
    /// specification.
    /// </remarks>
    public abstract class DltTraceLineBase : ITraceLine
    {
        /// <summary>
        /// Invalid counter value used as the default for the <see cref="Count"/> property.
        /// </summary>
        public const int InvalidCounter = -1;

        /// <summary>
        /// This is the decoded line of text with time stamp and other metadata stripped.
        /// </summary>
        /// <value>
        /// This is a string, that should normally present the text log that was emitted by the device being logged.
        /// That is, all metadata, such as timestamps, severity and other information is not present in this string.
        /// </value>
        public virtual string Text { get; set; }

        /// <summary>
        /// The current line in the log file.
        /// </summary>
        /// <value>This is the line number that this line of text was found on.</value>
        public int Line { get; set; }

        /// <summary>
        /// The position in the file.
        /// </summary>
        /// <value>This is the offset in the file where the line begins.</value>
        public long Position { get; set; }

        /// <summary>
        /// Describes the features which are available on this trace line.
        /// </summary>
        /// <value>The features available on this trace line.</value>
        /// <remarks>
        /// A DLT line has many fields of which some may not be present. This property allows to determine which of
        /// those fields are valid.
        /// </remarks>
        public DltLineFeatures Features { get; set; }

        /// <summary>
        /// Gets or sets the ECU identifier.
        /// </summary>
        /// <value>The ECU identifier.</value>
        public string EcuId { get; set; }

        /// <summary>
        /// Gets or sets the DLT message counter.
        /// </summary>
        /// <value>The DLT message counter.</value>
        /// <remarks>
        /// The user of this class is expected to set the value of this field if it is not set explicitly through any of
        /// the available constructors. If this is the case, it shall be initialized with the default value of
        /// <see cref="InvalidCounter"/>.
        /// </remarks>
        public int Count { get; set; } = InvalidCounter;

        /// <summary>
        /// Gets or sets the device time stamp.
        /// </summary>
        /// <value>The device time stamp.</value>
        public TimeSpan DeviceTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        public string ContextId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks>
        /// The value of this field is a combination of the type and subtype, as defined in the Diagnostic Log and Trace
        /// AUTOSAR Release 4.2.2 specification. The value shall always be a two digit number, the first digit
        /// representing the type of the DLT message, while the second digit represents the subtype of the DLT message.
        /// </remarks>
        public DltType Type { get; set; } = DltType.UNKNOWN;

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public int SessionId { get; set; }

        /// <summary>
        /// Gets or sets the time stamp of the log message, typically that from the PC.
        /// </summary>
        /// <remarks>
        /// Shall have a default value equal to the Unix time start date 00:00:00 UTC on 1st January 1970.
        /// <para>The value of this property is expected to be a Unix time date.</para>
        /// </remarks>
        public DateTime TimeStamp { get; set; } = DltConstants.DefaultTimeStamp;

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>
        /// Shall return a string containing the values of the DLT line fields, similar to a DLT trace line, exported to
        /// a text file using DLT Viewer 2.18.0. The date of the log message is converted to local time.
        /// </remarks>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                TimeStamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture),
                DeviceTimeStamp.TotalSeconds.ToString("F4", CultureInfo.InvariantCulture),
                Count,
                EcuId,
                ApplicationId,
                ContextId,
                SessionId,
                Type.GetDescription());
        }
    }
}
