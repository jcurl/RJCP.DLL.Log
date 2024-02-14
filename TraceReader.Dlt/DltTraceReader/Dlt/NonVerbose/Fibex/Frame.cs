namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Describes a frame loaded from a FIBEX file.
    /// </summary>
    internal class Frame : IFrame
    {
        /// <summary>
        /// Gets the ECU identifier.
        /// </summary>
        /// <value>The ECU identifier.</value>
        public string EcuId { get; set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets the application identifier for this message.
        /// </summary>
        /// <value>The application identifier for this message.</value>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets the context identifier for this message.
        /// </summary>
        /// <value>The context identifier for this message.</value>
        public string ContextId { get; set; }

        /// <summary>
        /// Gets or sets the message type for DLT (as a string read from the FIBEX file).
        /// </summary>
        /// <value>The message type for DLT.</value>
        internal string MessageDltType { get; set; }

        /// <summary>
        /// Gets or sets the message info subtype for DLT (as a string read from the FIBEX file).
        /// </summary>
        /// <value>The message info subtype for DLT.</value>
        internal string MessageDltInfo { get; set; }

        /// <summary>
        /// Gets the type of the message for this frame.
        /// </summary>
        /// <value>The type of the message for this frame.</value>
        public DltType MessageType { get; set; }

        private readonly List<Pdu> m_Arguments = new();

        /// <summary>
        /// Gets the arguments which make up this frame.
        /// </summary>
        /// <value>The arguments that make up this frame.</value>
        public IReadOnlyList<IPdu> Arguments { get { return m_Arguments; } }

        /// <summary>
        /// Adds the argument to the list of PDUs.
        /// </summary>
        /// <param name="pdu">The PDU argument to add.</param>
        internal void AddArgument(Pdu pdu)
        {
            m_Arguments.Add(pdu);
        }

        /// <summary>
        /// Indicates if this frame is valid or not.
        /// </summary>
        /// <value>Is <see langword="true"/> if this frame is valid; otherwise, <see langword="false"/>.</value>
        internal bool IsValid { get; set; } = true;

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder sb = new($"EcuID={EcuId} AppId={ApplicationId} CtxId={ContextId} {Id} ({MessageType})");

            if (m_Arguments.Count > 0) {
                foreach (IPdu pdu in m_Arguments) {
                    sb.Append(' ').Append(pdu.ToString());
                }
            }
            return sb.ToString();
        }
    }
}
