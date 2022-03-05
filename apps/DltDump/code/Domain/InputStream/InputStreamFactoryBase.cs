namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;

    /// <summary>
    /// Base class to simplify common code for an input stream factory.
    /// </summary>
    public abstract class InputStreamFactoryBase : IInputStreamFactory
    {
        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
        public IInputStream Create(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            Uri inputUri;
            try {
                inputUri = new Uri(uri);
                return Create(inputUri);
            } catch (UriFormatException) {
                // The URI could not be determined. Assume it is a file.
            }

            try {
                inputUri = new Uri(Path.Combine(Environment.CurrentDirectory, uri));
                return Create(inputUri);
            } catch (UriFormatException) {
                return null;
            }
        }

        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
        public abstract IInputStream Create(Uri uri);
    }
}
