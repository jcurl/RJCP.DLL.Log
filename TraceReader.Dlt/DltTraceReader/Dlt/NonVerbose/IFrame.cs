namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a frame, which is a single DLT message.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// Gets the ECU identifier.
        /// </summary>
        /// <value>The ECU identifier.</value>
        string EcuId { get; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        int Id { get; }

        /// <summary>
        /// Gets the application identifier for this message.
        /// </summary>
        /// <value>The application identifier for this message.</value>
        string ApplicationId { get; }

        /// <summary>
        /// Gets the context identifier for this message.
        /// </summary>
        /// <value>The context identifier for this message.</value>
        string ContextId { get; }

        /// <summary>
        /// Gets the type of the message for this frame.
        /// </summary>
        /// <value>The type of the message for this frame.</value>
        DltType MessageType { get; }

        /// <summary>
        /// Gets the arguments which make up this frame.
        /// </summary>
        /// <value>The arguments that make up this frame.</value>
        IReadOnlyList<IPdu> Arguments { get; }
    }
}
