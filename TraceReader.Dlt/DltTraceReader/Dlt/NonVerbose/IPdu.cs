namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    /// <summary>
    /// A Packet Data Unit, which describes a single argument in a frame.
    /// </summary>
    public interface IPdu
    {
        /// <summary>
        /// Gets the type of the PDU.
        /// </summary>
        /// <value>The type of the PDU.</value>
        public string PduType { get; }

        /// <summary>
        /// Gets the description of the PDU.
        /// </summary>
        /// <value>The description of the PDU.</value>
        public string Description { get; }

        /// <summary>
        /// Gets the length of the PDU.
        /// </summary>
        /// <value>The length of the PDU.</value>
        public int PduLength { get; }
    }
}
