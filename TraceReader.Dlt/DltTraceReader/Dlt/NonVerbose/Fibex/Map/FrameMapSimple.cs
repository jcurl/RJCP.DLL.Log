namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex.Map
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Maintains a map from Message Id to a frame.
    /// </summary>
    /// <remarks>
    /// This is the simplest map, assuming that all message identifiers are unique.
    /// </remarks>
    internal class FrameMapSimple : IFrameMapLoader
    {
        private readonly Dictionary<int, IFrame> m_Frames = new Dictionary<int, IFrame>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMapSimple"/> class.
        /// </summary>
        public FrameMapSimple() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMapSimple" /> class.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="frame">The frame that maps to the parameters given.</param>
        /// <exception cref="ArgumentNullException"><paramref name="frame"/> is <see langword="null"/>.</exception>
        internal FrameMapSimple(int id, IFrame frame)
        {
            if (frame is null) throw new ArgumentNullException(nameof(frame));

            m_Frames.Add(id, frame);
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
        /// <exception cref="ArgumentNullException"><paramref name="frame"/> is <see langword="null"/>.</exception>
        public bool TryAddFrame(int id, string appId, string ctxId, string ecuId, IFrame frame)
        {
            if (frame is null) throw new ArgumentNullException(nameof(frame));

            return m_Frames.TryAdd(id, frame);
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
            return m_Frames.TryGetValue(id, out frame);
        }
    }
}
