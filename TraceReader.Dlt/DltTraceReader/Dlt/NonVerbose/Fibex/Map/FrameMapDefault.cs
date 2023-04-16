namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Maintains a map from AppId, Context Id, Message Id to a frame.
    /// </summary>
    /// <remarks>
    /// When adding frames, we keep two maps. One mapping the AppId/CtxId/MessageId to a frame, the second only mapping
    /// the MessageId to a frame (if it doesn't already exist). When retrieving the frames, if the AppId/CtxId is
    /// provided, then the MessageId must exist for that combination, else if no AppId/CtxId is provided, then use only
    /// the MessageId to get the frame.
    /// <para>Behaviour of this class follows that of COVESA DLT-Viewer.</para>
    /// </remarks>
    internal class FrameMapDefault : IFrameMapLoader
    {
        private readonly FrameMapSimple m_Frames;
        private readonly Dictionary<(string, string), FrameMapSimple> m_StdHdrFrames = new Dictionary<(string, string), FrameMapSimple>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMapDefault"/> class.
        /// </summary>
        public FrameMapDefault()
        {
            m_Frames = new FrameMapSimple();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMapDefault"/> class.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="appId">The application identifier. May not be <see langword="null"/>.</param>
        /// <param name="ctxId">The context identifier. May not be <see langword="null"/>.</param>
        /// <param name="frame">The frame that maps to the parameters given.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appId"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="ctxId"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="frame"/> is <see langword="null"/>
        /// </exception>
        public FrameMapDefault(int id, string appId, string ctxId, IFrame frame)
        {
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            if (ctxId == null) throw new ArgumentNullException(nameof(ctxId));
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            m_Frames = new FrameMapSimple(id, frame);
            m_StdHdrFrames.Add((appId, ctxId), new FrameMapSimple(id, frame));
        }

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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appId"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="ctxId"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="frame"/> is <see langword="null"/>
        /// </exception>
        public bool TryAddFrame(int id, string appId, string ctxId, string ecuId, IFrame frame)
        {
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            if (ctxId == null) throw new ArgumentNullException(nameof(ctxId));
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            if (m_StdHdrFrames.TryGetValue((appId, ctxId), out FrameMapSimple map)) {
                if (!map.TryAddFrame(id, appId, ctxId, ecuId, frame)) return false;
            } else {
                m_StdHdrFrames.Add((appId, ctxId), new FrameMapSimple(id, frame));
            }

            m_Frames.TryAddFrame(id, appId, ctxId, ecuId, frame);
            return true;
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
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Frame not found</exception>
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
            if (appId != null && ctxId != null) {
                if (!m_StdHdrFrames.TryGetValue((appId, ctxId), out FrameMapSimple map)) {
                    frame = null;
                    return false;
                }
                return map.TryGetFrame(id, appId, ctxId, ecuId, out frame);
            }

            return m_Frames.TryGetFrame(id, appId, ctxId, ecuId, out frame);
        }
    }
}
