namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Maintains a map from AppId, Context Id, Message Id to a frame, with an optional ECU.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="FrameMapSimple"/>, if no ECU is added, then this becomes the same as
    /// <see cref="FrameMapSimple"/>. Adding to this map always adds the frame for all ECUs (and if there is another
    /// ECU with the same message, context and application identifier, then it isn't added). Adding with an ECU adds
    /// additionally for that ECU. When searching without an ECU, it looks from all ECUs. When searching with an ECU, it
    /// looks for the specific ECU, which if not found, then looks in all ECUs.
    /// </remarks>
    internal class FrameMapEcuSimple : IFrameMapLoader
    {
        private readonly FrameMapSimple m_Frames = new FrameMapSimple();
        private readonly Dictionary<string, FrameMapSimple> m_EcuFrames = new Dictionary<string, FrameMapSimple>();

        /// <summary>
        /// Tries to add the frame to the map.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">The application identifier. May not be <see langword="null"/>.</param>
        /// <param name="ctxId">The context identifier. May not be <see langword="null"/>.</param>
        /// <param name="ecuId">The ECU identifier. May be <see langword="null"/> if no ECU ID is known.</param>
        /// <param name="frame">The frame that maps to the parameters given.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the message identifier <paramref name="id"/> is unique for the given
        /// parameters, <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="frame"/> is <see langword="null"/>.</exception>
        public bool TryAddFrame(int id, string appId, string ctxId, string ecuId, IFrame frame)
        {
            bool success = m_Frames.TryAddFrame(id, appId, ctxId, ecuId, frame);
            if (ecuId is object) {
                if (m_EcuFrames.TryGetValue(ecuId, out FrameMapSimple map)) {
                    return map.TryAddFrame(id, appId, ctxId, ecuId, frame);
                }
                m_EcuFrames.Add(ecuId, new FrameMapSimple(id, frame));
                return true;
            }
            return success;
        }

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
        /// <remarks>
        /// Searches for the most appropriate frame given the ECU identifier, Application and Context identifier, and
        /// the Message identifier.
        /// </remarks>
        public IFrame GetFrame(int id, string appId, string ctxId, string ecuId)
        {
            if (TryGetFrame(id, appId, ctxId, ecuId, out IFrame frame))
                return frame;

            throw new KeyNotFoundException("Frame not found");
        }

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
        public bool TryGetFrame(int id, string appId, string ctxId, string ecuId, out IFrame frame)
        {
            if (ecuId is object) {
                if (!m_EcuFrames.TryGetValue(ecuId, out FrameMapSimple map)) {
                    frame = null;
                    return false;
                }
                return map.TryGetFrame(id, appId, ctxId, ecuId, out frame);
            }
            return m_Frames.TryGetFrame(id, appId, ctxId, ecuId, out frame);
        }
    }
}
