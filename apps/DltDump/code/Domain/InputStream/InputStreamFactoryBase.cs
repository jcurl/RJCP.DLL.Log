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
            ArgumentNullException.ThrowIfNull(uri);

            Uri inputUri;
            try {
                inputUri = new Uri(uri);
                return Create(inputUri);
            } catch (UriFormatException ex) {
                // If we have the format "xxx://", then we assume this was intended for a URI, even if it couldn't be
                // parsed, so we don't assume it is a file. But, "null:" is a DOS device name, so isn't a URI.
                if (IsUriFormat(uri))
                    throw new InputStreamException(ex.Message, ex);

                // The URI could not be determined. Assume it is a file.
            }

            try {
                inputUri = new Uri(Path.Combine(Environment.CurrentDirectory, uri));
                return Create(inputUri);
            } catch (UriFormatException ex) {
                throw new InputStreamException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Checks if the format of the URI is xxx://
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the string looks like a URI; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsUriFormat(string uri)
        {
            int len = uri.Length;
            int pos = 0;
            while (pos < len) {
                char c = uri[pos];
                if (c == ':') {
                    if (pos + 2 <= len && uri[pos + 1] == '/' && uri[pos + 2] == '/')
                        return true;
                    return false;
                } else if (c is (< 'a' or > 'z') and (< 'A' or > 'Z')) {
                    return false;
                }
                pos++;
            }
            return false;
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
