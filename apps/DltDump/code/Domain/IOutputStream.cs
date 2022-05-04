namespace RJCP.App.DltDump.Domain
{
    using System;
    using Domain.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Interface for writing a DLT output stream.
    /// </summary>
    public interface IOutputStream : IDisposable
    {
        /// <summary>
        /// Indicates if this output stream can write binary data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this object can write binary data; otherwise, <see langword="false"/>.
        /// </value>
        bool SupportsBinary { get; }

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <remarks>
        /// Setting the input file name and the format can assist with knowing how to write binary data, and optionally
        /// set the name of the file that should be written.
        /// </remarks>
        void SetInput(string fileName, InputFormat inputFormat);

        /// <summary>
        /// Writes the specified line to the output.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <returns>
        /// Returns <see langword="true"/> if after writing this line was written. A pure writer would not filter any
        /// lines. Is <see langword="false"/> if the line was filtered out.
        /// </returns>
        bool Write(DltTraceLineBase line);

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
        bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet);
    }
}
