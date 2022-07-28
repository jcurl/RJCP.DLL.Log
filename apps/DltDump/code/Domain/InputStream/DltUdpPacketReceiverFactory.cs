namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.Net;
    using Infrastructure.Uri;
    using Resources;

    /// <summary>
    /// An input factory to create a UDP packet receiver for reading DLT files.
    /// </summary>
    public class DltUdpPacketReceiverFactory : InputStreamFactoryBase
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
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.Scheme.Equals("udp", StringComparison.InvariantCulture)) {
                string message = string.Format(AppResources.DomainInputStreamFactoryUnsupported, uri.Scheme);
                throw new InputStreamException(message);
            }

            if (!string.IsNullOrEmpty(uri.UserInfo) ||
                !string.IsNullOrEmpty(uri.AbsolutePath) && !uri.AbsolutePath.Equals("/") ||
                !string.IsNullOrEmpty(uri.Fragment)) {
                string message = string.Format(AppResources.DomainInputStreamFactoryInvalid, uri);
                throw new InputStreamException(message);
            }

            string hostName = uri.Host;
            UriHostNameType hostType = Uri.CheckHostName(hostName);
            switch (hostType) {
            case UriHostNameType.IPv4:
                break;
            default:
                string message = string.Format(AppResources.TcpOpenError_InvalidHostName, hostName);
                throw new InputStreamException(message);
            }

            IPAddress endPoint = IPAddress.Parse(hostName);
            int port = uri.Port;
            if (port <= 0) port = 3490;

            IPAddress bindAddr = null;
            var keys = uri.ParseQuery();
            if (keys.Count > 0) {
                foreach (var kvp in keys) {
                    if (kvp.Key.Equals("bindto", StringComparison.OrdinalIgnoreCase)) {
                        bindAddr = IPAddress.Parse(kvp.Value);
                        break;
                    }
                }
            }

            return bindAddr == null ?
                new DltUdpPacketReceiver(endPoint, port) :
                new DltUdpPacketReceiver(bindAddr, port, endPoint);
        }
    }
}
