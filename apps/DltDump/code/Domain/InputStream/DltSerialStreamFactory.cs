namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Infrastructure.Uri;
    using Resources;
    using RJCP.IO.Ports;

    public sealed class DltSerialStreamFactory : InputStreamFactoryBase
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

            if (!uri.Scheme.Equals("ser", StringComparison.InvariantCulture)) {
                string message = string.Format(AppResources.DomainInputStreamFactoryUnsupported, uri.Scheme);
                throw new InputStreamException(message);
            }

            if (!string.IsNullOrEmpty(uri.Authority))
                throw new InputStreamException(AppResources.SerialOpenError_AuthorityNotSupported);
            if (!string.IsNullOrEmpty(uri.Fragment))
                throw new InputStreamException(AppResources.SerialOpenError_InvalidUri);
            if (!string.IsNullOrEmpty(uri.UserInfo))
                throw new InputStreamException(AppResources.SerialOpenError_InvalidUri);
            if (!string.IsNullOrEmpty(uri.Query))
                throw new InputStreamException(AppResources.SerialOpenError_InvalidUri);

            string path = uri.LocalPath;
            IReadOnlyList<string> par = UriExtensions.ParseCommaSeparatedList(path);
            if (par.Count >= 5) {
                string port = par[0];
                int baud;
                int databits;
                try {
                    baud = int.Parse(par[1], CultureInfo.InvariantCulture);
                    databits = int.Parse(par[2], CultureInfo.InvariantCulture);
                } catch (OverflowException ex) {
                    throw new InputStreamException(ex.Message, ex);
                }

                Parity parity;
                if (par[3].Equals("N", StringComparison.OrdinalIgnoreCase)) {
                    parity = Parity.None;
                } else if (par[3].Equals("E", StringComparison.OrdinalIgnoreCase)) {
                    parity = Parity.Even;
                } else if (par[3].Equals("O", StringComparison.OrdinalIgnoreCase)) {
                    parity = Parity.Odd;
                } else if (par[3].Equals("M", StringComparison.OrdinalIgnoreCase)) {
                    parity = Parity.Mark;
                } else if (par[3].Equals("S", StringComparison.OrdinalIgnoreCase)) {
                    parity = Parity.Space;
                } else {
                    throw new InputStreamException(AppResources.SerialOpenError_InvalidParity);
                }

                StopBits stopbits;
                if (par[4].Equals("1", StringComparison.OrdinalIgnoreCase)) {
                    stopbits = StopBits.One;
                } else if (par[4].Equals("1.5", StringComparison.OrdinalIgnoreCase)) {
                    stopbits = StopBits.One5;
                } else if (par[4].Equals("2", StringComparison.OrdinalIgnoreCase)) {
                    stopbits = StopBits.Two;
                } else {
                    throw new InputStreamException(AppResources.SerialOpenError_InvalidStopBits);
                }

                Handshake handshake = Handshake.None;
                if (par.Count == 6) {
                    if (!Enum.TryParse(par[5], true, out handshake)) {
                        throw new InputStreamException(AppResources.SerialOpenError_InvalidHandshake);
                    }
                }

                return new DltSerialStream(port, baud, databits, parity, stopbits, handshake);
            }
            throw new InputStreamException(AppResources.SerialOpenError_InvalidUri);
        }
    }
}
