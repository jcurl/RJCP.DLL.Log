namespace RJCP.App.DltDump.Domain
{
    /// <summary>
    /// Factory for creating a <see cref="IOutputStream"/> object based on the output format and output file name.
    /// </summary>
    public interface IOutputStreamFactory
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IOutputStream"/> should force overwrite.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if the output should force overwrite if it exists; otherwise,
        /// <see langword="false"/>.
        /// </value>
        bool Force { get; set; }

        /// <summary>
        /// Gets or sets the split.
        /// </summary>
        /// <value>The number of bytes each file should grow to before splitting.</value>
        long Split { get; set; }

        /// <summary>
        /// Determines if non-verbose messages should be converted to verbose.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if decoded non-verbose messages should be converted to verbose; otherwise,
        /// <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This flag is only useful for DLT writers.
        /// </remarks>
        bool ConvertNonVerbose { get; set; }

        /// <summary>
        /// A list of input files that the output stream will read from.
        /// </summary>
        /// <value>The list of input files that the output stream will read from.</value>
        /// <remarks>
        /// This is useful to give the output stream the list of input files, so that it can ensure it won't ever
        /// overwrite the input files.
        /// </remarks>
        InputFiles InputFiles { get; }

        /// <summary>
        /// Creates the output stream based on the output format.
        /// </summary>
        /// <param name="outFormat">The output format that should be used.</param>
        /// <param name="outFileName">Name of the output file.</param>
        /// <returns>An output stream to support the output format.</returns>
        IOutputStream Create(OutputFormat outFormat, string outFileName);
    }
}
