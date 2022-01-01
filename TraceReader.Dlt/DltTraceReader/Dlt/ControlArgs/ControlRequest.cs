namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// The base class for control request messages.
    /// </summary>
    public abstract class ControlRequest : IControlArg
    {
        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public abstract int ServiceId { get; }

        /// <summary>
        /// Gets the default type.
        /// </summary>
        /// <value>The default type/subtype of the DLT message that is expected to hold this payload.</value>
        /// <remarks>Control requests should have the type of <see cref="DltType.CONTROL_REQUEST"/>.</remarks>
        public DltType DefaultType
        {
            get { return DltType.CONTROL_REQUEST; }
        }
    }
}
