namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    /// <summary>
    /// An input factory to create a file stream for reading DLT files.
    /// </summary>
    public sealed class DltFileStreamFactory : InputStreamFactoryBase
    {
        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="InputStreamException">
        /// There was a problem instantiating the stream. The message and inner exception provide more information on
        /// the fault.
        /// </exception>
        public override IInputStream Create(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            string fileName = uri.LocalPath;
            return new DltFileStream(fileName);
        }
    }
}
