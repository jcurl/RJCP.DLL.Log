namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using Resources;

    /// <summary>
    /// Manages writing data for DltDump to the output file.
    /// </summary>
    public sealed class OutputWriter : IDisposable
    {
        private Stream m_FileStream;

        /// <summary>
        /// Gets the length of data written.
        /// </summary>
        /// <value>The length of data written.</value>
        public long Length { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is open; otherwise, <see langword="false"/>.</value>
        public bool IsOpen { get { return m_FileStream != null; } }

        /// <summary>
        /// Opens a file for the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="OutputStreamException">The underlying stream threw an exception.</exception>
        public void Open(string fileName)
        {
            Open(fileName, FileMode.CreateNew);
        }

        /// <summary>
        /// Opens a file for the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mode">The mode to use when opening the file, to allow overwriting and appending.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="OutputStreamException">The underlying stream threw an exception.</exception>
        public void Open(string fileName, FileMode mode)
        {
            if (m_Disposed) throw new ObjectDisposedException(nameof(OutputWriter));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (m_FileStream != null) throw new InvalidOperationException(AppResources.DomainOutputWriterOpen);

            try {
                m_FileStream = new FileStream(fileName, mode, FileAccess.Write, FileShare.Read);
                m_FileStream.Seek(0, SeekOrigin.End);
                Length = m_FileStream.Position;
            } catch (Exception ex) {
                if (m_FileStream != null) m_FileStream.Dispose();
                m_FileStream = null;
                Length = 0;
                throw new OutputStreamException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified buffer to file.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="InvalidOperationException">Writer not opened.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="offset"/> and <paramref name="count"/> describe an invalid range in
        /// <paramref name="buffer"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="OutputStreamException">The underlying stream threw an exception.</exception>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (m_Disposed) throw new ObjectDisposedException(nameof(OutputWriter));

            Stream stream = m_FileStream;
            if (stream == null) throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The length and offset would exceed the boundaries of the array/buffer");

            try {
                stream.Write(buffer, offset, count);
                Length += count;
            } catch (Exception ex) {
                throw new OutputStreamException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified buffer to file.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="InvalidOperationException">Writer not opened.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        public void Write(ReadOnlySpan<byte> buffer)
        {
            if (m_Disposed) throw new ObjectDisposedException(nameof(OutputWriter));

            Stream stream = m_FileStream;
            if (stream == null) throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);
            try {
                stream.Write(buffer);
                Length += buffer.Length;
            } catch (Exception ex) {
                throw new OutputStreamException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Flushes data to the file.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="InvalidOperationException">Writer not opened.</exception>
        public void Flush()
        {
            if (m_Disposed) throw new ObjectDisposedException(nameof(OutputWriter));

            Stream stream = m_FileStream;
            if (stream == null) throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);
            try {
                stream.Flush();
            } catch (Exception ex) {
                throw new OutputStreamException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Closes the file.
        /// </summary>
        public void Close()
        {
            if (m_Disposed) return;

            if (m_FileStream != null) {
                m_FileStream.Dispose();
                m_FileStream = null;
            }
        }

        private bool m_Disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_Disposed) {
                try {
                    Close();
                } catch { /* Don't throw exceptions in Dispose */ }
                m_Disposed = true;
            }
        }
    }
}
