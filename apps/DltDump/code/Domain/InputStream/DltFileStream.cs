namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;
    using Resources;

    /// <summary>
    /// Provides an input stream for DLT files.
    /// </summary>
    public sealed class DltFileStream : IInputStream
    {
        private const int FileSystemCaching = 262144;
        private readonly string m_FileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="fileName"/> is <see langword="null"/>.
        /// </exception>
        public DltFileStream(string fileName)
        {
            ThrowHelper.ThrowIfNullOrEmptyMsg(fileName, AppResources.FileOpenError_EmptyName);
            Connection = fileName;
            m_FileName = fileName;
            InputFileName = Path.GetFileName(fileName);

            string extension = Path.GetExtension(fileName);
            if (extension.Equals(".pcap", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".pcapng", StringComparison.OrdinalIgnoreCase)) {
                SuggestedFormat = InputFormat.Pcap;
            } else {
                SuggestedFormat = InputFormat.File;
            }
        }

        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        public string Scheme { get { return "file"; } }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string Connection { get; }

        /// <summary>
        /// Gets the name of the input file, associated with the connection string.
        /// </summary>
        /// <value>The name of the input file.</value>
        /// <remarks>
        /// The input file name can be used to calculate the output file name, given the input is loaded from a file
        /// system like URI.
        /// </remarks>
        public string InputFileName { get; }

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
        public InputFormat SuggestedFormat { get; }

        /// <summary>
        /// Gets a value indicating if this input stream requires a connection.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, this stream requires a connection; otherwise, <see langword="false"/>.
        /// </value>
        public bool RequiresConnection { get { return false; } }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        public Stream InputStream { get; private set; }

        /// <summary>
        /// Gets the input packet provider.
        /// </summary>
        /// <value>
        /// The input packet provider. This object doesn't support packets and always returns <see langword="null"/>.
        /// </value>
        public IPacket InputPacket { get { return null; } }

        /// <summary>
        /// Opens the input stream.
        /// </summary>
        /// <returns>The input stream.</returns>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <exception cref="InputStreamException">
        /// There was a problem opening the stream. Exceptions of this type are wrappers around the more specific
        /// exception, that indicate the stream should be skipped.
        /// <para>File name is invalid.</para>
        /// <para>- or -</para>
        /// <para>The file is not found.</para>
        /// <para>- or -</para>
        /// <para>The directory is not found.</para>
        /// <para>- or -</para>
        /// <para>The path is too long.</para>
        /// <para>- or -</para>
        /// <para>An I/O error occurred.</para>
        /// <para>- or -</para>
        /// <para>The caller doesn't have the required permissions.</para>
        /// <para>- or -</para>
        /// <para>The file is in use, or insufficient permissions.</para>
        /// </exception>
        public void Open()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            if (InputStream is not null) return;

            try {
                InputStream = new FileStream(m_FileName, FileMode.Open, FileAccess.Read, FileShare.Read,
                    FileSystemCaching, FileOptions.Asynchronous | FileOptions.SequentialScan);
            } catch (FileNotFoundException ex) {
                string message = string.Format(AppResources.FileOpenError_FileNotFound, Connection);
                throw new InputStreamException(message, ex);
            } catch (DirectoryNotFoundException ex) {
                string message = string.Format(AppResources.FileOpenError_DirectoryNotFound, Connection);
                throw new InputStreamException(message, ex);
            } catch (PathTooLongException ex) {
                string message = string.Format(AppResources.FileOpenError_PathTooLong, Connection);
                throw new InputStreamException(message, ex);
            } catch (IOException ex) {
                string message = string.Format(AppResources.FileOpenError_IOException, Connection, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (ArgumentException ex) {
                string message = string.Format(AppResources.FileOpenError_InvalidFile, Connection, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (NotSupportedException ex) {
                string message = string.Format(AppResources.FileOpenError_InvalidFile, Connection, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (System.Security.SecurityException ex) {
                string message = string.Format(AppResources.FileOpenError_Security, Connection, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (UnauthorizedAccessException ex) {
                string message = string.Format(AppResources.FileOpenError_Unauthorized, Connection);
                throw new InputStreamException(message, ex);
            }
        }

        /// <summary>
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>
        /// Returns if the input stream was connected. As a file is never connected, this always returns
        /// <see langword="true"/>.
        /// </returns>
        public Task<bool> ConnectAsync()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            if (InputStream is null)
                throw new InvalidOperationException(AppResources.DomainInputStreamNotOpen);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Closes this stream, but does not dispose, so it can be reopened.
        /// </summary>
        public void Close()
        {
            if (InputStream is not null) InputStream.Close();
            InputStream = null;
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and/or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed) {
                Close();
                m_IsDisposed = true;
            }
        }
    }
}
