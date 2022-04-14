namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Infrastructure.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// An output module that can write binary data as it is received.
    /// </summary>
    public sealed class DltOutput : OutputBase, IOutputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName) : base(fileName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName, bool force) : base(fileName, 0, force) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName, long split, bool force) : base(fileName, split, force) { }

        /// <summary>
        /// Indicates if this output stream can write binary data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this object can write binary data; otherwise, <see langword="false"/>.
        /// </value>
        public bool SupportsBinary { get { return true; } }

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <exception cref="ArgumentNullException">fileName</exception>
        /// <exception cref="ArgumentOutOfRangeException">inputFormat</exception>
        /// <remarks>
        /// Setting the input file name and the format can assist with knowing how to write binary data, and optionally
        /// set the name of the file that should be written.
        /// </remarks>
        public void SetInput(string fileName, InputFormat inputFormat)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!Enum.IsDefined(typeof(InputFormat), inputFormat))
                throw new ArgumentOutOfRangeException(nameof(inputFormat));
            if (inputFormat == InputFormat.Automatic)
                throw new ArgumentOutOfRangeException(nameof(inputFormat));

            if (inputFormat != InputFormat.File)
                throw new NotImplementedException();

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
            /* Not yet implemented */
            return false;
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
            Write(line.TimeStamp, packet);
            return true;
        }
    }
}
