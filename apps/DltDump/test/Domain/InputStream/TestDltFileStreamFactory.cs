namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    /// <summary>
    /// A test file stream factory that can simulate errors opening files. No files are accessed.
    /// </summary>
    public class TestDltFileStreamFactory : InputStreamFactoryBase
    {
        /// <summary>
        /// Used to simulate an error on opening a file name.
        /// </summary>
        /// <value>The exception that should be simulated when opening a file.</value>
        public FileOpenError OpenError { get; set; } = FileOpenError.None;

        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from. This is ignored as it is a mock.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="InputStreamException">Simulated exception that is caught.</exception>
        /// <exception cref="InvalidOperationException">Simulated Invalid Operation Exception.</exception>
        public override IInputStream Create(Uri uri)
        {
            return new TestDltFileStream() {
                OpenError = OpenError
            };
        }
    }
}
