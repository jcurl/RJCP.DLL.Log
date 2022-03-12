namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Dlt;
    using Infrastructure.Net;

    /// <summary>
    /// A connection to a TCP listen socket on the server.
    /// </summary>
    public sealed class DltTcpStream : IInputStream
    {
        private readonly TcpClientStream m_TcpStream;

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
            m_TcpStream = new TcpClientStream(hostname, port) {
                DisconnectTimeout = 5000
            };
        }

        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        public string Scheme { get { return "tcp"; } }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        public Stream InputStream { get { return m_TcpStream; } }

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
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>Returns if the input stream was connected.</returns>
        public Task<bool> ConnectAsync()
        {
            return m_TcpStream.ConnectAsync();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_TcpStream.Dispose();
        }
    }
}
