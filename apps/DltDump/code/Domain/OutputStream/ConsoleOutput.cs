namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Domain.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Writes the output to the console.
    /// </summary>
    public sealed class ConsoleOutput : IOutputStream
    {
        private readonly object m_WriteLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOutput"/> class.
        /// </summary>
        public ConsoleOutput() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOutput"/> class.
        /// </summary>
        /// <param name="showPosition">Write the position of the timestamp if <see langword="true"/>.</param>
        public ConsoleOutput(bool showPosition)
        {
            ShowPosition = showPosition;
        }

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
        /// <remarks>Writing to the console only supports writing lines.</remarks>
        public bool SupportsBinary { get { return false; } }

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <remarks>
        /// The console output does not need to know the input format, nor does it do anything with the file name. This
        /// method does nothing.
        /// </remarks>
        public void SetInput(string fileName, InputFormat inputFormat)
        {
            /* Nothing to do */
        }

        /// <summary>
        /// Writes the specified line to the output.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <returns>This method always writes the output, so returns <see langword="true"/>.</returns>
        public bool Write(DltTraceLineBase line)
        {
            lock (m_WriteLock) {
                if (ShowPosition) {
                    Global.Instance.Terminal.StdOut.WriteLine("{0:x8}: {1}", line.Position, line.ToString());
                } else {
                    Global.Instance.Terminal.StdOut.WriteLine(line.ToString());
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
        /// <remarks>
        /// As this class doesn't write binary data, this is the same as <see cref="Write(DltTraceLineBase)"/>.
        /// </remarks>
        public bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            return Write(line);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            /* Nothing to dispose */
        }
    }
}
