namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;
    using Infrastructure.Net;
    using Resources;

    /// <summary>
    /// Receives UDP packets from the network.
    /// </summary>
    public sealed class DltUdpPacketReceiver : IInputStream
    {
        private readonly UdpPacketReceiver m_Receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltUdpPacketReceiver"/> class.
        /// </summary>
        /// <param name="endPoint">The end point to receive UDP packets on.</param>
        /// <param name="port">The port to receive packets on.</param>
        /// <exception cref="ArgumentNullException"><paramref name="endPoint"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="port"/> is less or equal to zero, or more than 65535.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="endPoint"/> is not an IPv4 address.
        /// </exception>
        public DltUdpPacketReceiver(IPAddress endPoint, int port)
        {
            ArgumentNullException.ThrowIfNull(endPoint);
            ThrowHelper.ThrowIfNotBetween(port, 1, 65535);

            m_Receiver = new UdpPacketReceiver(new IPEndPoint(endPoint, port));
            Connection = string.Format("{0}://{1}:{2}", Scheme, endPoint, port);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltUdpPacketReceiver"/> class.
        /// </summary>
        /// <param name="bindAddr">The local address to bind to.</param>
        /// <param name="port">The port to receive packets on.</param>
        /// <param name="multicastGroup">The multicast group address to join where packets are sent.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bindAddr"/> or <paramref name="multicastGroup"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="port"/> is less than or equal to zero, or more than 65535.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="bindAddr"/> or <paramref name="multicastGroup"/> is not an IPv4 address.
        /// <para>- or -</para>
        /// <paramref name="multicastGroup"/> is not a multicast address.
        /// <para>- or -</para>
        /// <paramref name="bindAddr"/> is not a unicast address (it must be a local interface, but it is not checked here).
        /// <para>- or -</para>
        /// <paramref name="bindAddr"/> port is out of range (1..65535);
        /// </exception>
        public DltUdpPacketReceiver(IPAddress bindAddr, int port, IPAddress multicastGroup)
        {
            ArgumentNullException.ThrowIfNull(bindAddr);
            ArgumentNullException.ThrowIfNull(multicastGroup);
            ThrowHelper.ThrowIfNotBetween(port, 1, 65535);

            m_Receiver = new UdpPacketReceiver(new IPEndPoint(bindAddr, port), multicastGroup);
            Connection = string.Format("{0}://{1}:{2}/?bindto={3}", Scheme, multicastGroup, port, bindAddr);
        }

        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        public string Scheme { get { return "udp"; } }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string Connection { get; }

        /// <summary>
        /// Gets the name of the input file, associated with the connection string.
        /// </summary>
        /// <value>The name of the input file.</value>
        /// <remarks>
        /// The input file name can be used to calculate the output file name, given the input is loaded from a file
        /// system like URI.
        /// </remarks>
        public string InputFileName { get { return null; } }

        /// <summary>
        /// Gets a value indicating whether this instance is live stream.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is live stream; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// A live stream generally indicates that time stamps should be based on the PC clock, and is not part of the
        /// input stream.
        /// </remarks>
        public bool IsLiveStream { get { return true; } }

        /// <summary>
        /// Gets the suggested format that should be used for instantiating a decoder.
        /// </summary>
        /// <value>The suggested format.</value>
        public InputFormat SuggestedFormat { get { return InputFormat.Network; } }

        /// <summary>
        /// Gets a value indicating if this input stream requires a connection.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, this stream requires a connection; otherwise, <see langword="false"/>.
        /// </value>
        public bool RequiresConnection { get { return false; } }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        /// <remarks>There is no input stream, see <see cref="InputPacket"/>.</remarks>
        public Stream InputStream { get { return null; } }

        /// <summary>
        /// Gets the input packet provider.
        /// </summary>
        /// <value>The input packet provider. This is <see langword="null"/> if the object is not open.</value>
        public IPacket InputPacket { get; private set; }

        /// <summary>
        /// Opens the input stream.
        /// </summary>
        /// <returns>The input stream.</returns>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public void Open()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(DltUdpPacketReceiver));
            if (InputPacket is object) return;

            m_Receiver.Open();
            InputPacket = m_Receiver;
        }

        /// <summary>
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>Returns if the input stream was connected.</returns>
        public Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(DltUdpPacketReceiver));
            if (InputPacket is null)
                throw new InvalidOperationException(AppResources.DomainInputStreamNotOpen);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Closes this stream, but does not dispose, so it can be reopened.
        /// </summary>
        public void Close()
        {
            if (InputPacket is object) InputPacket.Close();
            InputPacket = null;
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed) {
                Close();
                m_IsDisposed = true;
            }
        }
    }
}
