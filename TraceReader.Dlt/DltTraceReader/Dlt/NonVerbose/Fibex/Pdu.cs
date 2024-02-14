namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    /// <summary>
    /// Represents a PDU.
    /// </summary>
    internal class Pdu : IPdu
    {
        /// <summary>
        /// Gets the type of the PDU.
        /// </summary>
        /// <value>The type of the PDU.</value>
        public string PduType { get; set; }

        /// <summary>
        /// Gets the description of the PDU.
        /// </summary>
        /// <value>The description of the PDU.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the length of the PDU.
        /// </summary>
        /// <value>The length of the PDU.</value>
        public int PduLength { get; set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (Description is not null) {
                return $"[{Description}]";
            }
            return $"{PduType} ({PduLength})";
        }
    }
}
