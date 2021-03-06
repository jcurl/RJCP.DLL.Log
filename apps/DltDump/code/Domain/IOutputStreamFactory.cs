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
        /// Creates the output stream based on the output format.
        /// </summary>
        /// <param name="outFormat">The output format that should be used.</param>
        /// <param name="outFileName">Name of the output file.</param>
        /// <returns>An output stream to support the output format.</returns>
        IOutputStream Create(OutputFormat outFormat, string outFileName);
    }
}
