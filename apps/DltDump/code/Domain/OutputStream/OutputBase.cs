namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using System.Text;
    using Resources;

    /// <summary>
    /// The <see cref="OutputBase"/> manages writing binary files
    /// </summary>
    /// <remarks>
    /// This object handles the write operations when the file is first written.
    /// </remarks>
    public abstract class OutputBase : IDisposable
    {
        private readonly OutputWriter m_Writer = new OutputWriter();
        private readonly Template m_Template;
        private readonly string m_FileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        protected OutputBase(string fileName) : this(fileName, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName, bool force)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(AppResources.FileOpenError_EmptyName, nameof(fileName));

            m_Template = new Template(fileName);
            m_FileName = m_Template.ToString();
            Force = force;

            m_Encoding = Encoding.UTF8;
            m_Encoder = m_Encoding.GetEncoder();
            m_NewLine = m_Encoding.GetBytes(Environment.NewLine);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="OutputBase"/> is set to overwrite files.
        /// </summary>
        /// <value>Is <see langword="true"/> if force enabled; otherwise, <see langword="false"/>.</value>
        public bool Force { get; }

        private Encoding m_Encoding;
        private Encoder m_Encoder;
        private byte[] m_NewLine;

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        /// <exception cref="ArgumentNullException"><see cref="Encoding"/> is <see langword="null"/>.</exception>
        public Encoding Encoding
        {
            get { return m_Encoding; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Encoding));

                Encoding oldEncoding = m_Encoding;
                byte[] oldNewLine = m_NewLine;
                try {
                    m_Encoding = value;
                    m_Encoder = m_Encoding.GetEncoder();
                    m_NewLine = m_Encoding.GetBytes(Environment.NewLine);
                } catch {
                    m_Encoding = oldEncoding;
                    m_NewLine = oldNewLine;
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the input file name.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        protected void SetInput(string fileName)
        {
            /* The file name is not used */
        }

        // Defines the maximum line length.
        private readonly byte[] m_Buffer = new byte[65536];

        /// <summary>
        /// Writes the specified line of the given time stamp.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="line">The line to write..</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputBase"/> is disposed.</exception>
        protected void Write(DateTime timeStamp, string line)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(OutputBase));
            OpenWriter();

            m_Encoder.Convert(line, m_Buffer, true, out int _, out int bytes, out bool _);
            m_Writer.Write(m_Buffer, 0, bytes);
            m_Writer.Write(m_NewLine, 0, m_NewLine.Length);
        }

        /// <summary>
        /// Writes the specified line of the given time stamp.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="message">The line to write..</param>
        /// <param name="args">The arguments to format.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputBase"/> is disposed.</exception>
        protected void Write(DateTime timeStamp, string message, params object[] args)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(OutputBase));
            OpenWriter();

            string line = string.Format(message, args);
            m_Encoder.Convert(line, m_Buffer, true, out int _, out int bytes, out bool _);
            m_Writer.Write(m_Buffer, 0, bytes);
            m_Writer.Write(m_NewLine, 0, m_NewLine.Length);
        }

        /// <summary>
        /// Writes the specified buffer of the given time stamp.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="buffer">The buffer to write.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputBase"/> is disposed.</exception>
        protected void Write(DateTime timeStamp, ReadOnlySpan<byte> buffer)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(OutputBase));
            OpenWriter();

            m_Writer.Write(buffer);
        }

        /// <summary>
        /// Writes the specified buffer with a header of the given time stamp.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="header">The header that prepends the buffer.</param>
        /// <param name="buffer">The buffer to write.</param>
        /// <exception cref="ObjectDisposedException"><see cref="OutputBase"/> is disposed.</exception>
        protected void Write(DateTime timeStamp, ReadOnlySpan<byte> header, ReadOnlySpan<byte> buffer)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(OutputBase));
            OpenWriter();

            m_Writer.Write(header);
            m_Writer.Write(buffer);
        }

        private void OpenWriter()
        {
            if (m_Writer.IsOpen) return;

            FileMode mode = Force ? FileMode.Create : FileMode.CreateNew;
            m_Writer.Open(m_FileName, mode);
        }

        /// <summary>
        /// Flushes this contents of the stream to disk.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="OutputBase"/> is disposed.</exception>
        public void Flush()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(OutputBase));
            m_Writer.Flush();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                m_Writer.Dispose();
            }
            m_IsDisposed = true;
        }
    }
}
