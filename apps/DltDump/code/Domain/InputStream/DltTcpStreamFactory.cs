namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using Resources;

    /// <summary>
    /// An input factory to create a TCP stream for reading DLT files.
    /// </summary>
    public sealed class DltTcpStreamFactory : InputStreamFactoryBase
    {
        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="ArgumentException"><paramref name="uri"/> is an invalid scheme.</exception>
        /// <exception cref="InputStreamException">Invalid host name.</exception>
        public override IInputStream Create(Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            if (!uri.Scheme.Equals("tcp", StringComparison.InvariantCulture)) {
                string message = string.Format(AppResources.DomainInputStreamFactoryUnsupported, uri.Scheme);
                throw new InputStreamException(message);
            }

            if (!string.IsNullOrEmpty(uri.UserInfo) ||
                !string.IsNullOrEmpty(uri.Query) ||
                !string.IsNullOrEmpty(uri.Fragment) ||
                !string.IsNullOrEmpty(uri.AbsolutePath) && !uri.AbsolutePath.Equals("/")) {
                string message = string.Format(AppResources.DomainInputStreamFactoryInvalid, uri);
                throw new InputStreamException(message);
            }

            string hostName = uri.Host;
            UriHostNameType hostType = Uri.CheckHostName(hostName);
            switch (hostType) {
            case UriHostNameType.IPv4:
            case UriHostNameType.Dns:
                break;
            default:
                string message = string.Format(AppResources.TcpOpenError_InvalidHostName, hostName);
                throw new InputStreamException(message);
            }

            int port = uri.Port;
            if (port <= 0) port = 3490;

            return new DltTcpStream(hostName, port);
        }
    }
}
