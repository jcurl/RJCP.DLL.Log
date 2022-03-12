namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;

    /// <summary>
    /// Provides an input stream for DLT files.
    /// </summary>
    public sealed class DltFileStream : IInputStream
    {
        private const int FileSystemCaching = 262144;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="fileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">The <paramref name="fileName"/> is invalid.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="fileName"/> is a non-file device.</exception>
        /// <exception cref="FileNotFoundException">The <paramref name="fileName"/> is not found.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller doesn't have the required permissions.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file is in use, or insufficient permissions.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// </exception>
        public DltFileStream(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            InputStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read,
                FileSystemCaching, FileOptions.Asynchronous | FileOptions.SequentialScan);
        }

        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        public string Scheme { get { return "file"; } }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        public Stream InputStream { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is live stream.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is live stream; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// A live stream generally indicates that time stamps should be based on the PC clock, and is not part of the
        /// input stream. This stream always returns <see langword="false"/>.
        /// </remarks>
        public bool IsLiveStream { get { return false; } }

        /// <summary>
        /// Gets the suggested format that should be used for instantiating a decoder.
        /// </summary>
        /// <value>The suggested format, which is <see cref="InputFormat.File"/>.</value>
        public InputFormat SuggestedFormat { get { return InputFormat.File; } }

        /// <summary>
        /// Gets a value indicating if this input stream requires a connection.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, this stream requires a connection; otherwise, <see langword="false"/>.
        /// </value>
        public bool RequiresConnection { get { return false; } }

        /// <summary>
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>
        /// Returns if the input stream was connected. As a file is never connected, this always returns
        /// <see langword="true"/>.
        /// </returns>
        public Task<bool> ConnectAsync()
        {
            return Task.FromResult(true);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and/or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed && InputStream != null) {
                InputStream.Dispose();
            }
            m_IsDisposed = true;
        }
    }
}
