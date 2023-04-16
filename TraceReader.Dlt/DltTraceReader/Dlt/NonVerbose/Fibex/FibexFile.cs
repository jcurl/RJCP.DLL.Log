namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System.Collections.Generic;

    /// <summary>
    /// Loads a FIBEX file as described by the AutoSAR DLT standard 4.3.0
    /// </summary>
    public class FibexFile : IFrameMap
    {
        private readonly bool m_WithEcuId;
        private readonly bool m_WithExtHdr;

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexFile"/> class.
        /// </summary>
        /// <param name="options">The options which determines how to load the FIBEX files.</param>
        public FibexFile(FibexOptions options)
        {
            m_WithEcuId = options.HasFlag(FibexOptions.WithEcuId);
            m_WithExtHdr = !options.HasFlag(FibexOptions.WithoutExtHeader);
        }

        /// <summary>
        /// Loads the file and merges it into the current in-memory representation.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadFile(string fileName)
        {

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
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <remarks>
        /// Searches for the most appropriate frame given the ECU identifier, Application and Context identifier, and
        /// the Message identifier.
        /// </remarks>
        public IFrame GetFrame(int id, string appId, string ctxId, string ecuId)
        {
            throw new KeyNotFoundException();
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
            frame = null;
            return false;
        }
    }
}
