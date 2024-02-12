namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Resources;
    using FileSystemNodeInfo = RJCP.IO.FileSystemNodeInfo;

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
        private readonly HashSet<FileSystemNodeInfo> m_OutputFiles = new HashSet<FileSystemNodeInfo>();
        private readonly InputFiles m_InputFiles;
        private List<string> m_Segments;
        private readonly long m_Split;
        private long m_NextSplit;
        private int m_SplitCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to (may be a template).</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName) : this(fileName, null, 0, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to (may be a template).</param>
        /// <param name="inputs">A collection of input files that should be protected from overwriting.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName, InputFiles inputs) : this(fileName, inputs, 0, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to (may be a template).</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName, long split, bool force) : this(fileName, null, split, force) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to (may be a template).</param>
        /// <param name="inputs">A collection of input files that should be protected from overwriting.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        protected OutputBase(string fileName, InputFiles inputs, long split, bool force)
        {
            ArgumentNullException.ThrowIfNull(fileName);
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(AppResources.FileOpenError_EmptyName, nameof(fileName));
            m_InputFiles = inputs;

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
                ArgumentNullException.ThrowIfNull(value);

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
        /// Gets or sets the automatic flush period, in milliseconds.
        /// </summary>
        /// <value>The automatic flush period, in milliseconds.</value>
        /// <remarks>
        /// This value is used when the output is opened for the first time. Once it is opened, the value is no longer
        /// used.
        /// </remarks>
        public int AutoFlushPeriod { get; set; }

        private void AddOutputFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return;

            FileSystemNodeInfo file = new FileSystemNodeInfo(fileName);
            m_OutputFiles.Add(file);
        }

        private void CheckIsOutputFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (!File.Exists(fileName)) return;

            FileSystemNodeInfo file = new FileSystemNodeInfo(fileName);
            if (m_OutputFiles.Contains(file)) {
                // We don't overwrite files that we've just created.
                string message = string.Format(AppResources.DomainOutputNoOverwrite, m_Template.ToString());
                throw new OutputStreamException(message);
            }
        }

        private void CheckIsProtectedFile(string fileName)
        {
            if (m_InputFiles is null) return;
            if (m_InputFiles.IsProtectedFile(fileName)) {
                // We don't overwrite protected files.
                string message = string.Format(AppResources.DomainOutputNoOverwriteInput, fileName);
                throw new OutputStreamException(message);
            }
        }

        /// <summary>
        /// Sets the input file name.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <remarks>
        /// Setting a new input file name can cause the current file to be closed if the output file name depends on the
        /// input file name. Setting the <paramref name="fileName"/> to <see langword="null"/> or to
        /// <see cref="string.Empty"/> will cause the file name to be cleared and the old output file to be closed if it
        /// was previously not empty.
        /// <list type="bullet">
        /// <item>Previous=null/empty or new; FileName=null/empty; Result=Continue writing.</item>
        /// <item>
        /// Previous=null/empty or new; FileName=file name; Result=If template contains <c>%FILE%</c> then create new
        /// file.
        /// </item>
        /// <item>
        /// Previous=file name; FileName=null/empty; Result=If template contains <c>%FILE%</c> then create new file.
        /// </item>
        /// <item>
        /// Previous=file name; FileName=file name; Result=If template contains <c>%FILE%</c> then create new file.
        /// </item>
        /// </list>
        /// When creating a new file, if the new file name based on the template was already written using this session,
        /// an error will occur, so existing files created in this session are not overwritten. This can occur also if
        /// two distinct full paths have the same file name (but in different folders).
        /// </remarks>
        protected void SetInput(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) {
                if (!m_Template.AllowConcatenation && !string.IsNullOrEmpty(CurrentFile()))
                    ResetSegments();

                m_Template.Variables["FILE"] = string.Empty;
            } else {
                if (!m_Template.AllowConcatenation)
                    ResetSegments();

                m_Template.Variables["FILE"] = Path.GetFileNameWithoutExtension(fileName);
            }
        }

        private string CurrentFile()
        {
            if (m_Template.Variables.TryGetValue("FILE", out string fileName))
                return fileName ?? string.Empty;
            return string.Empty;
        }

        private void ResetSegments()
        {
            m_Segments = null;
            if (m_Writer.IsOpen) m_Writer.Close();
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
            if (m_Segments is null) {
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
            if (AutoFlushPeriod > 0) m_Writer.AutoFlush(AutoFlushPeriod);
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
