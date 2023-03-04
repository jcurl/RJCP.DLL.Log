namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Collections.Generic;
    using RJCP.Diagnostics.Log.Dlt.Args;

    /// <summary>
    /// Represents a Non-Verbose DLT message
    /// </summary>
    /// <remarks>
    /// A non-verbose DLT message is functionally equivalent to a DLT verbose message, but it has a message identifier.
    /// To decode the dynamic data, external input is required that defines the format of the argument stream.
    /// </remarks>
    public class DltNonVerboseTraceLine : DltTraceLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltNonVerboseTraceLine"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier for the non-verbose message.</param>
        public DltNonVerboseTraceLine(int messageId)
        {
            MessageId = messageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltNonVerboseTraceLine"/> class with a read only argument list.
        /// </summary>
        /// <param name="messageId">The message identifier for the non-verbose message.</param>
        /// <param name="arguments">The arguments.</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// To make this method as fast as possible, the underlying array is only referenced, not copied. After using
        /// this constructor, the original array should be discarded (or never modified).
        /// </remarks>
        public DltNonVerboseTraceLine(int messageId, IDltArg[] arguments)
            : base(arguments)
        {
            MessageId = messageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltNonVerboseTraceLine"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier for the non-verbose message.</param>
        /// <param name="arguments">The payload argument collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/> may not be <see langword="null"/>.</exception>
        public DltNonVerboseTraceLine(int messageId, IEnumerable<IDltArg> arguments)
            : base(arguments)
        {
            MessageId = messageId;
        }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId { get; }

        private string m_Text;

        /// <summary>
        /// This is the decoded line of text with time stamp and other metadata stripped.
        /// </summary>
        /// <value>
        /// This is a string, that should normally present the text log that was emitted by the device being logged.
        /// That is, all metadata, such as timestamps, severity and other information is not present in this string. The
        /// DLT trace line only contains the concatenation of all the arguments. Feature fields and other properties are
        /// not represented here, but by <see cref="DltTraceLine.ToString()"/>.
        /// </value>
        /// <remarks>
        /// The text is generated lazily, at the time it is queried, and the result is then stored. Thus changes to this
        /// object will not automatically update the <see cref="Text"/> property. Writing to this property will override
        /// the value calculated. If the text should be automatically recalculated, set this property the value of
        /// <see langword="null"/>.
        /// </remarks>
        public override string Text
        {
            get
            {
                m_Text ??= string.Format("[{0}] {1}", unchecked((uint)MessageId), base.Text);
                return m_Text;
            }
            set { m_Text = value; }
        }
    }
}
