namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System;
    using System.Xml;

    /// <summary>
    /// Raised when a problem is observed loading or merging a Fibex file.
    /// </summary>
    public class FibexLoadErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FibexLoadErrorEventArgs"/> class.
        /// </summary>
        /// <param name="warning">The warning message that can be used to generate a localised string.</param>
        public FibexLoadErrorEventArgs(FibexWarnings warning)
            : this(warning, null, 0, null, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexLoadErrorEventArgs"/> class.
        /// </summary>
        /// <param name="warning">The warning message that can be used to generate a localised string.</param>
        /// <param name="position">The position in the XML file with the fault.</param>
        public FibexLoadErrorEventArgs(FibexWarnings warning, IXmlLineInfo position)
            : this(warning, position, 0, null, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexLoadErrorEventArgs"/> class.
        /// </summary>
        /// <param name="warning">The warning message that can be used to generate a localised string.</param>
        /// <param name="position">The position in the XML file with the fault.</param>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="ctxId">The context identifier.</param>
        /// <param name="ecuId">The ECU identifier.</param>
        public FibexLoadErrorEventArgs(FibexWarnings warning, IXmlLineInfo position, int id, string appId, string ctxId, string ecuId)
        {
            Warning = warning;
            Position = position;
            MessageId = id;
            ApplicationId = appId;
            ContextId = ctxId;
            EcuId = ecuId;
        }

        /// <summary>
        /// Gets the warning message that can be used to generate a localised string.
        /// </summary>
        /// <value>he warning message that can be used to generate a localised string.</value>
        public FibexWarnings Warning { get; private set; }

        /// <summary>
        /// Gets the position in the XML file with the fault.
        /// </summary>
        /// <value>The position in the XML file with the fault.</value>
        public IXmlLineInfo Position { get; private set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId { get; private set; }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Gets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        public string ContextId { get; private set; }

        /// <summary>
        /// Gets the ECU identifier.
        /// </summary>
        /// <value>The ECU identifier.</value>
        public string EcuId { get; private set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (MessageId == 0 && ApplicationId is null && ContextId is null && EcuId is null) {
                if (Position is null || !Position.HasLineInfo())
                    return $"{Warning}";

                return $"{Warning} : XML Line {Position.LineNumber} Pos {Position.LinePosition}";
            }

            if (Position is null || !Position.HasLineInfo())
                return $"{Warning} : {ApplicationId ?? string.Empty} {ContextId ?? string.Empty} {EcuId ?? string.Empty}";

            return $"{Warning} : XML Line {Position.LineNumber} Pos {Position.LinePosition} " +
                $"{ApplicationId ?? string.Empty} {ContextId ?? string.Empty} {EcuId ?? string.Empty}";
        }
    }
}
