namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Infrastructure.IO;
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
        private readonly HashSet<DetailedFileInfo> m_OutputFiles = new HashSet<DetailedFileInfo>();
        private readonly HashSet<DetailedFileInfo> m_ProtectedFiles = new HashSet<DetailedFileInfo>();
        private List<string> m_Segments;
        private readonly long m_Split;
        private long m_NextSplit;
        private int m_SplitCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName) : this(fileName, 0, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName, long split, bool force)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(AppResources.FileOpenError_EmptyName, nameof(fileName));

            m_Template = new Template(fileName);
            Force = force;
            m_Split = split;
            ResetSplit();

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
        /// Adds a file name that should be protected from writing.
        /// </summary>
        /// <param name="fileName">Name of the file that should be protected.</param>
        /// <remarks>
        /// Any files set here will prevent from being overwritten, even if forced. This could be, for example, the file
        /// is an input file.
        /// </remarks>
        public void AddProtectedFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return;

            DetailedFileInfo file = new DetailedFileInfo(fileName);
            m_ProtectedFiles.Add(file);
        }

        private void CheckIsProtectedFile(string fileName)
        {
            if (IsProtectedFile(fileName)) {
                // We don't overwrite protected files.
                string message = string.Format(AppResources.DomainOutputNoOverwriteInput, m_Template.ToString());
                throw new OutputStreamException(message);
            }
        }

        private bool IsProtectedFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return false;

            DetailedFileInfo file = new DetailedFileInfo(fileName);
            return m_ProtectedFiles.Contains(file);
        }

        private void AddOutputFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return;

            DetailedFileInfo file = new DetailedFileInfo(fileName);
            m_OutputFiles.Add(file);
        }

        private void CheckIsOutputFile(string fileName)
        {
            if (IsOutputFile(fileName)) {
                // We don't overwrite files that we've just created.
                string message = string.Format(AppResources.DomainOutputNoOverwrite, m_Template.ToString());
                throw new OutputStreamException(message);
            }
        }

        private bool IsOutputFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return false;

            DetailedFileInfo file = new DetailedFileInfo(fileName);
            return m_OutputFiles.Contains(file);
        }

        /// <summary>
        /// Sets the input file name.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        protected void SetInput(string fileName)
        {
            if (!m_Template.AllowConcatenation) {
                m_Segments = null;
                if (m_Writer.IsOpen) m_Writer.Close();
            }

            m_Template.Variables["FILE"] = Path.GetFileNameWithoutExtension(fileName);
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
            OpenWriter(timeStamp);

            m_Encoder.Convert(line, m_Buffer, true, out int _, out int bytes, out bool _);
            m_Writer.Write(m_Buffer, 0, bytes);
            m_Writer.Write(m_NewLine, 0, m_NewLine.Length);
            CheckSplit();
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
            OpenWriter(timeStamp);

            string line = string.Format(message, args);
            m_Encoder.Convert(line, m_Buffer, true, out int _, out int bytes, out bool _);
            m_Writer.Write(m_Buffer, 0, bytes);
            m_Writer.Write(m_NewLine, 0, m_NewLine.Length);
            CheckSplit();
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
            OpenWriter(timeStamp);

            m_Writer.Write(buffer);
            CheckSplit();
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
            OpenWriter(timeStamp);

            m_Writer.Write(header);
            m_Writer.Write(buffer);
            CheckSplit();
        }

        private void OpenWriter(DateTime timeStamp)
        {
            if (m_Writer.IsOpen) return;

            SetTimeStamp(timeStamp);
            if (m_Segments == null) {
                ResetSplit();

                string fileName = Path.GetFullPath(m_Template.ToString());
                CheckIsProtectedFile(fileName);
                CheckIsOutputFile(fileName);

                // Open the file first, then add to segments. If the file couldn't be opened, it doesn't exist and we
                // don't define a new segment list.
                FileMode mode = Force ? FileMode.Create : FileMode.CreateNew;
                m_Writer.Open(fileName, mode);
                m_Segments = new List<string>() { fileName };
                AddOutputFile(fileName);
            } else {
                string fileName = Path.GetFullPath(m_Template.ToString());
                CheckIsProtectedFile(fileName);

                FileMode mode;
                if (m_Segments[^1].Equals(fileName, StringComparison.Ordinal)) {
                    mode = FileMode.Append;
                    m_Writer.Open(fileName, mode);
                    if (m_Writer.Length > m_NextSplit) {
                        m_NextSplit += m_Split / 5;
                    }
                } else {
                    CheckIsOutputFile(fileName);
                    mode = Force ? FileMode.Create : FileMode.CreateNew;
                    m_Writer.Open(fileName, mode);
                    m_Segments.Add(fileName);
                    AddOutputFile(fileName);
                }
            }
        }

        private void CheckSplit()
        {
            if (!m_Template.SupportsSplit || m_NextSplit == 0) return;

            if (m_Writer.Length >= m_NextSplit) {
                // The next write will open and append to this file, or write to the next one.
                m_Writer.Close();
                SplitIncrement();
            }
        }

        private void ResetSplit()
        {
            m_SplitCounter = 1;
            m_NextSplit = m_Split;
            m_Template.Variables["CTR"] = m_SplitCounter.ToString("D3");
        }

        private void SplitIncrement()
        {
            m_SplitCounter++;
            m_Template.Variables["CTR"] = m_SplitCounter.ToString("D3");
        }

        private void SetTimeStamp(DateTime timeStamp)
        {
            DateTime local = timeStamp.ToLocalTime();
            m_Template.Variables["CDATE"] = local.ToString("yyyyMMdd");
            m_Template.Variables["CTIME"] = local.ToString("HHmmss");
            m_Template.Variables["CDATETIME"] = local.ToString("yyyyMMdd\\THHmmss");
        }

        /// <summary>
        /// Flushes this contents of the stream to disk.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="OutputBase"/> is disposed.</exception>
        public void Flush()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(OutputBase));
            if (m_Writer.IsOpen) m_Writer.Flush();
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
