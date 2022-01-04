namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// A periodic DLT time marker for indicating an alive connection.
    /// </summary>
    public sealed class DltTimeMarker : IControlArg
    {
        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        /// <remarks>This type of message has no service identifier, and so is -1.</remarks>
        public int ServiceId { get { return -1; } }

        /// <summary>
        /// Gets the default type.
        /// </summary>
        /// <value>The default type/subtype of the DLT message that is expected to hold this payload.</value>
        public DltType DefaultType { get { return DltType.CONTROL_TIME; } }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "[]";
        }
    }
}
