namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Domain.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Writes the output to a text file in UTF8 format.
    /// </summary>
    public sealed class TextOutput : OutputBase, IOutputStream
    {
        private readonly object m_WriteLock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public TextOutput(string fileName) : base(fileName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutput" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="force">Force overwrite the file if <see langword="true" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public TextOutput(string fileName, bool force) : base(fileName, 0, force) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutput" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public TextOutput(string fileName, long split, bool force) : base(fileName, split, force) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutput" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="inputs">A collection of input files that should be protected from overwriting.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public TextOutput(string fileName, InputFiles inputs, long split, bool force) : base(fileName, inputs, split, force) { }

        /// <summary>
        /// Gets or sets a value indicating whether the output should contain the position.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if the console output should show the position; otherwise <see langword="false"/>.
        /// </value>
        public bool ShowPosition { get; set; }

        /// <summary>
        /// Indicates if this output stream can write binary data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this object can write binary data; otherwise, <see langword="false"/>.
        /// </value>
        public bool SupportsBinary { get { return false; } }

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputFormat"/> is not defined, or is <see cref="InputFormat.Automatic"/>.
        /// </exception>
        /// <remarks>
        /// Setting the input file name and the format can assist with knowing how to write binary data, and optionally
        /// set the name of the file that should be written.
        /// </remarks>
        public void SetInput(string fileName, InputFormat inputFormat)
        {
            ThrowHelper.ThrowIfEnumUndefined(inputFormat);
            if (inputFormat == InputFormat.Automatic)
                throw new ArgumentOutOfRangeException(nameof(inputFormat));

            SetInput(fileName);
        }

        /// <summary>
        /// Writes the specified line to the output.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <returns>
        /// Returns <see langword="true"/> if after writing this line was written. A pure writer would not filter any
        /// lines. Is <see langword="false"/> if the line was filtered out.
        /// </returns>
        public bool Write(DltTraceLineBase line)
        {
            lock (m_WriteLock) {
                if (ShowPosition) {
                    Write(line.TimeStamp, "{0:x8}: {1}", line.Position, line.ToString());
                } else {
                    Write(line.TimeStamp, line.ToString());
                }
            }
            return true;
        }

        /// <summary>
        /// Writes the specified line to the output.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <param name="packet">The original packet data that generated the line to help write the output.</param>
        /// <returns>
        /// Returns <see langword="true"/> if after writing this line was written. A pure writer would not filter any
        /// lines. Is <see langword="false"/> if the line was filtered out.
        /// </returns>
        /// <remarks>The output knows of the input format through the method <see cref="SetInput"/>.</remarks>
        public bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            return Write(line);
        }
    }
}
