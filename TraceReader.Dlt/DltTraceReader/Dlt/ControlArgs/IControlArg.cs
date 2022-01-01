namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// <see cref="IControlArg"/> represents the payload of a DLT control message.
    /// </summary>
    public interface IControlArg
    {
        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        int ServiceId { get; }

        /// <summary>
        /// Gets the default type.
        /// </summary>
        /// <value>
        /// The default type/subtype of the DLT message that is expected to hold this payload.
        /// </value>
        DltType DefaultType { get; }
    }
}
