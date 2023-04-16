namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System.Collections.Generic;

    /// <summary>
    /// Maps a Message Identifier to a frame, which can be used to decode a non-verbose payload.
    /// </summary>
    public interface IFrameMap
    {
        /// <summary>
        /// Gets the frame from the message, app and context identifier.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">
        /// The application identifier. May be <see langword="null"/> if the payload doesn't contain an application
        /// identifier.
        /// </param>
        /// <param name="ctxId">
        /// The context identifier. May be <see langword="null"/> if the payload doesn't contain a context identifier.
        /// </param>
        /// <param name="ecuId">
        /// The ECU identifier. May be <see langword="null"/> if the payload doesn't contain an ECU identifier.
        /// </param>
        /// <returns>
        /// Returns a frame implementing <see cref="IFrame"/>. If no suitable frame can be found, an exception is
        /// raised.
        /// </returns>
        /// <exception cref="KeyNotFoundException">The message cannot be found.</exception>
        /// <remarks>
        /// Searches for the most appropriate frame given the ECU identifier, Application and Context identifier, and
        /// the Message identifier.
        /// </remarks>
        IFrame GetFrame(int id, string appId, string ctxId, string ecuId);

        /// <summary>
        /// Tries to get the frame from the message, app and context identifier.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">
        /// The application identifier. May be <see langword="null"/> if the payload doesn't contain an application
        /// identifier.
        /// </param>
        /// <param name="ctxId">
        /// The context identifier. May be <see langword="null"/> if the payload doesn't contain a context identifier.
        /// </param>
        /// <param name="ecuId">
        /// The ECU identifier. May be <see langword="null"/> if the payload doesn't contain an ECU identifier.
        /// </param>
        /// <param name="frame">The frame that is returned if the message identifier exists.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the message identifier <paramref name="id"/> exists,
        /// <see langword="false"/> otherwise.
        /// </returns>
        /// <remarks>
        /// If the decoded message contains an extended header with the application and context identifier, then the
        /// message identifier may be reused.
        /// </remarks>
        bool TryGetFrame(int id, string appId, string ctxId, string ecuId, out IFrame frame);
    }
}
