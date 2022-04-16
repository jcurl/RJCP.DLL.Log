namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using OutputStream;
    using Resources;

    /// <summary>
    /// Factory to create an output stream.
    /// </summary>
    public class OutputStreamFactory : IOutputStreamFactory
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IOutputStream"/> should force overwrite.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if the output should force overwrite if it exists; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool Force { get; set; }

        /// <summary>
        /// Gets or sets the split.
        /// </summary>
        /// <value>The number of bytes each file should grow to before splitting.</value>
        public long Split { get; set; }

        /// <summary>
        /// Creates the output stream based on the output format.
        /// </summary>
        /// <param name="outFormat">The output format that should be used.</param>
        /// <param name="outFileName">Name of the output file.</param>
        /// <returns>An output stream to support the output format.</returns>
        public IOutputStream Create(OutputFormat outFormat, string outFileName)
        {
            if (outFormat == OutputFormat.Automatic) {
                outFormat = GetOutputFormat(outFileName);
            }

            switch (outFormat) {
            case OutputFormat.Console:
                return new ConsoleOutput();
            case OutputFormat.Text:
                return new TextOutput(outFileName, Split, Force);
            case OutputFormat.Dlt:
                return new DltOutput(outFileName, Split, Force);
            default:
                Log.App.TraceEvent(TraceEventType.Warning, AppResources.DomainOutputStreamFactoryUnknown, outFormat.ToString());
                return null;
            }
        }

        private static OutputFormat GetOutputFormat(string outFileName)
        {
            if (string.IsNullOrWhiteSpace(outFileName)) return OutputFormat.Console;
            if (outFileName.Equals("CON:", StringComparison.OrdinalIgnoreCase)) return OutputFormat.Console;
            if (outFileName.Equals("/dev/stdout", StringComparison.OrdinalIgnoreCase)) return OutputFormat.Console;

            // We don't know how to write DLT files
            if (Path.GetExtension(outFileName).Equals(".dlt", StringComparison.InvariantCultureIgnoreCase))
                return OutputFormat.Dlt;

            return OutputFormat.Text;
        }
    }
}
