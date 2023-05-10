namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using Infrastructure.Tasks;
    using Resources;

    /// <summary>
    /// Manages writing data for DltDump to the output file.
    /// </summary>
    public sealed class OutputWriter : IDisposable
    {
        private Stream m_FileStream;
        private bool m_OwnsStream;

        /// <summary>
        /// Gets the length of data written.
        /// </summary>
        /// <value>The length of data written.</value>
        public long Length { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is open; otherwise, <see langword="false"/>.</value>
        public bool IsOpen { get { return m_FileStream is object; } }

        /// <summary>
        /// Opens a file for the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Underlying stream is still open. Call <see cref="Close"/> first.</exception>
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
        /// <exception cref="InvalidOperationException">Underlying stream is still open. Call <see cref="Close"/> first.</exception>
        /// <exception cref="OutputStreamException">The underlying stream threw an exception.</exception>
        public void Open(string fileName, FileMode mode)
        {
            if (m_Disposed) throw new ObjectDisposedException(nameof(OutputWriter));
            if (fileName is null) throw new ArgumentNullException(nameof(fileName));
            if (m_FileStream is object) throw new InvalidOperationException(AppResources.DomainOutputWriterOpen);

            try {
                m_FileStream = new FileStream(fileName, mode, FileAccess.Write, FileShare.Read);
                m_FileStream.Seek(0, SeekOrigin.End);
                Length = m_FileStream.Position;
                m_OwnsStream = true;
            } catch (Exception ex) {
                if (m_FileStream is object) m_FileStream.Dispose();
                m_FileStream = null;
                Length = 0;
                throw new OutputStreamException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Opens a file for the specified file name.
        /// </summary>
        /// <param name="stream">Name of the file.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputWriter"/> is disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not writable.
        /// <para>- or -</para>
        /// Underlying stream is still open. Call <see cref="Close"/> first.
        /// </exception>
        public void Open(Stream stream)
        {
            if (m_Disposed) throw new ObjectDisposedException(nameof(OutputWriter));
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (m_FileStream is object) throw new InvalidOperationException(AppResources.DomainOutputWriterOpen);
            if (!stream.CanWrite) throw new InvalidOperationException(AppResources.DomainOutputWriterCantWrite);

            try {
                m_FileStream = stream;
                Length = stream.Position;
                m_OwnsStream = false;
            } catch {
                m_FileStream = null;
                Length = 0;
                throw;
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

            Stream stream = m_FileStream ?? throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), AppResources.InfraArgOutOfRangeNegative);
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), AppResources.InfraArgOutOfRangeNegative);
            if (offset > buffer.Length - count) throw new ArgumentException(AppResources.InfraArgOutOfRangeIndex);

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

            Stream stream = m_FileStream ?? throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);
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

            Stream stream = m_FileStream ?? throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);
            try {
                stream.Flush();
            } catch (Exception ex) {
                throw new OutputStreamException(ex.Message, ex);
            }
        }

        private CancelTask m_AutoFlushTask;
        private readonly object m_CloseLock = new object();

        /// <summary>
        /// Automatically flushes the output at regular intervals in a Task.
        /// </summary>
        /// <param name="milliSeconds">The seconds interval to wait between flushing.</param>
        /// <exception cref="InvalidOperationException">This method was previously called.</exception>
        public void AutoFlush(int milliSeconds)
        {
            if (m_AutoFlushTask is object) return;
            if (m_FileStream is null) throw new InvalidOperationException(AppResources.DomainOutputWriterNotOpen);

            m_AutoFlushTask = new CancelTask((t) => {
                while (true) {
                    Stream stream = m_FileStream;
                    if (stream is null) return;

                    if (t.WaitHandle.WaitOne(milliSeconds)) {
                        // We're cancelled, so exit the thread.
                        return;
                    }
                    try {
                        lock (m_CloseLock) {
                            if (IsOpen) stream.Flush();
                        }
                    } catch (Exception) {
                        // Ignore background flush operations. Assume they're permanent.
                        return;
                    }
                }
            });
            _ = m_AutoFlushTask.Run();
        }

        /// <summary>
        /// Closes the file.
        /// </summary>
        public void Close()
        {
            if (m_Disposed) return;

            if (m_AutoFlushTask is object) {
                m_AutoFlushTask.Cancel().GetAwaiter().GetResult();
                m_AutoFlushTask = null;
            }

            lock (m_CloseLock) {
                // The AutoFlush method might be checking the file stream in parallel.
                if (m_FileStream is object) {
                    if (m_OwnsStream) m_FileStream.Dispose();
                    m_FileStream = null;
                }
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
