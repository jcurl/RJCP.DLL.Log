namespace RJCP.App.DltDump.Services
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Resources;
    using RJCP.Core.Collections;
    using RJCP.Core.Collections.Specialized;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;

    /// <summary>
    /// Represents a <see cref="IFrameMap"/> that can load a directory of files.
    /// </summary>
    public class FibexMap : IFrameMap
    {
        private readonly FibexFile m_FrameMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexMap"/> class.
        /// </summary>
        /// <param name="options">The options when merging multiple Fibex files.</param>
        public FibexMap(FibexOptions options)
        {
            m_FrameMap = new FibexFile(options);
            m_FrameMap.LoadErrorEvent += FrameMap_LoadErrorEvent;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFrame GetFrame(int id, string appId, string ctxId, string ecuId)
        {
            return m_FrameMap.GetFrame(id, appId, ctxId, ecuId);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetFrame(int id, string appId, string ctxId, string ecuId, out IFrame frame)
        {
            return m_FrameMap.TryGetFrame(id, appId, ctxId, ecuId, out frame);
        }

        /// <summary>
        /// Gets the event log, maintaining information when loading a Fibex file.
        /// </summary>
        /// <value>The event log.</value>
        public EventLog<FibexLogEntry> EventLog { get; } = new EventLog<FibexLogEntry>();

        private string m_ProcessedFile;

        private void FrameMap_LoadErrorEvent(object sender, FibexLoadErrorEventArgs e)
        {
            EventEntry<FibexLogEntry> entry = new(
                new FibexLogEntry(m_ProcessedFile, e.Warning, e.ToString())
            );
            EventLog.Add(entry);
        }

        /// <summary>
        /// Loads the Fibex file.
        /// </summary>
        /// <param name="path">The path to load from, either a file or a directory with *.xml files..</param>
        /// <returns>
        /// <see langword="true"/> if there are no event entries, <see langword="false"/> otherwise (see
        /// <see cref="EventLog"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is an empty string.</exception>
        /// <exception cref="FileNotFoundException">
        /// The <paramref name="path"/> cannot be found as a file or a directory.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="IOException">An I/O error, such as a disk error, occurred.</exception>
        /// <exception cref="UnauthorizedAccessException">Access is not permitted by the Operating System.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="fileName"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        /// non-NTFS environment.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// </exception>
        public bool LoadFibex(string path)
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(path);

            if (File.Exists(path)) {
                InternalLoadFibex(path);
            } else if (Directory.Exists(path)) {
                InternalLoadFibexDir(path);
            } else {
                string message = string.Format(AppResources.FibexOpenError_NotFound, path);
                throw new FileNotFoundException(message, path);
            }
            return EventLog.Count == 0;
        }

        private void InternalLoadFibex(string fileName)
        {
            try {
                // This is set, so the event knows what file we're setting in case of problems.
                m_ProcessedFile = fileName;
                m_FrameMap.LoadFile(fileName);
            } finally {
                m_ProcessedFile = null;
            }
        }

        private void InternalLoadFibexDir(string path)
        {
            foreach (string fileName in Directory.GetFiles(path, "*.xml")) {
                InternalLoadFibex(fileName);
            }
        }
    }
}
