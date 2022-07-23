namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;
    using Infrastructure.Net;
    using Resources;

    /// <summary>
    /// A connection to a TCP listen socket on the server.
    /// </summary>
    public sealed class DltTcpStream : IInputStream
    {
        private readonly string m_HostName;
        private readonly int m_Port;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTcpStream"/> class.
        /// </summary>
        /// <param name="hostname">The host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hostname"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="hostname"/> is whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="port"/> is less or equal to zero, or more than 65535
        /// </exception>
        public DltTcpStream(string hostname, int port)
        {
            if (hostname == null) throw new ArgumentNullException(nameof(hostname));
            if (string.IsNullOrWhiteSpace(hostname))
                throw new ArgumentException(AppResources.InfraTcpStreamInvalidHostName, nameof(hostname));
            if (port <= 0 || port > 65535) throw new ArgumentOutOfRangeException(nameof(port));

            m_HostName = hostname;
            m_Port = port;
        }

        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        public string Scheme { get { return "tcp"; } }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string Connection
        {
            get
            {
                return string.Format("{0}://{1}:{2}", Scheme, m_HostName, m_Port);
            }
        }

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
        public bool RequiresConnection { get { return true; } }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        public Stream InputStream { get; private set; }

        /// <summary>
        /// Gets the input packet provider.
        /// </summary>
        /// <value>
        /// The input packet provider. This object doesn't support packets and always returns <see langword="null"/>.
        /// </value>
        public IPacket InputPacket { get { return null; } }

        /// <summary>
        /// Opens the input stream.
        /// </summary>
        /// <returns>The input stream.</returns>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public void Open()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(DltTcpStream));
            if (InputStream != null) return;

            InputStream = new TcpClientStream(m_HostName, m_Port) {
                DisconnectTimeout = 5000
            };
        }

        /// <summary>
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>Returns if the input stream was connected.</returns>
        public Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(DltTcpStream));
            if (InputStream == null)
                throw new InvalidOperationException(AppResources.DomainInputStreamNotOpen);

            return ((TcpClientStream)InputStream).ConnectAsync();
        }

        /// <summary>
        /// Closes this stream, but does not dispose, so it can be reopened.
        /// </summary>
        public void Close()
        {
            if (InputStream != null) InputStream.Close();
            InputStream = null;
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
