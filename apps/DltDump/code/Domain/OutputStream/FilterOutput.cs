namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Domain.Dlt;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// An <see cref="IOutputStream"/> that applies a filter, and chains to the next output if the filter matches.
    /// </summary>
    public sealed class FilterOutput : IOutputStream
    {
        private readonly Constraint m_Filter;
        private readonly IOutputStream m_Output;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterOutput"/> class.
        /// </summary>
        /// <param name="filter">The filter that should be used when emitting output.</param>
        /// <param name="output">The object to output filtered data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> or <paramref name="output"/> is <see langword="null"/>.
        /// </exception>
        public FilterOutput(Constraint filter, IOutputStream output)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (output == null) throw new ArgumentNullException(nameof(output));

            m_Filter = filter;
            m_Output = output;
        }

        /// <summary>
        /// Indicates if this output stream can write binary data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this object can write binary data; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This value is obtained by the next chained <see cref="IOutputStream"/> element as given in the constructor.
        /// </remarks>
        public bool SupportsBinary { get { return m_Output.SupportsBinary; } }

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <remarks>
        /// Setting the input file name and the format can assist with knowing how to write binary data, and optionally
        /// set the name of the file that should be written.
        /// </remarks>
        public void SetInput(string fileName, InputFormat inputFormat)
        {
            m_Output.SetInput(fileName, inputFormat);
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
            if (m_Filter.Check(line)) {
                m_Output.Write(line);
                return true;
            }

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
            if (m_Filter.Check(line)) {
                m_Output.Write(line, packet);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            m_Output.Dispose();
        }
    }
}
