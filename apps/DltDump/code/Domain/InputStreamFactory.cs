namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using InputStream;
    using Resources;

    /// <summary>
    /// The factory that generates an <see cref="IInputStream"/> object from a URI.
    /// </summary>
    public class InputStreamFactory : InputStreamFactoryBase
    {
        /// <summary>
        /// Sets the factory for a specific scheme
        /// </summary>
        /// <param name="scheme">The scheme to register for.</param>
        /// <param name="factory">
        /// The factory that should be used for creating a <see cref="IInputStream"/> object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="scheme"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If the <paramref name="factory"/> is <see langword="null"/>, this deregisters the <paramref name="scheme"/>.
        /// </remarks>
        public void SetFactory(string scheme, IInputStreamFactory factory)
        {
            ArgumentNullException.ThrowIfNull(scheme);

            if (factory is null) {
                m_Factories.Remove(scheme);
                return;
            }

            m_Factories[scheme] = factory;
        }

        private readonly Dictionary<string, IInputStreamFactory> m_Factories = new Dictionary<string, IInputStreamFactory>() {
            { "file", new DltFileStreamFactory() },
            { "tcp", new DltTcpStreamFactory() },
            { "ser", new DltSerialStreamFactory() },
            { "udp", new DltUdpPacketReceiverFactory() }
        };

        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>
        /// The input stream that can be used for creating a decoder. <see langword="null"/> can be returned if the
        /// scheme is unknown or if the registered factory cannot instantiate the stream from the scheme.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
        public override IInputStream Create(Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            if (!m_Factories.TryGetValue(uri.Scheme, out IInputStreamFactory factory)) {
                Log.App.TraceEvent(TraceEventType.Warning, AppResources.DomainInputStreamFactoryUnknown, uri.Scheme);
                return null;
            }

            IInputStream inputStream = factory.Create(uri);
            if (inputStream is null) {
                Log.App.TraceEvent(TraceEventType.Warning, AppResources.DomainInputStreamFactoryUnsupported, uri.Scheme);
                return null;
            }

            return inputStream;
        }
    }
}
