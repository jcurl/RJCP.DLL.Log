namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using Resources;

    /// <summary>
    /// A TCP based client stream with read timeouts to disconnect.
    /// </summary>
    public sealed class TcpClientStream : Stream
    {
        private readonly string m_HostName;
        private readonly int m_Port;
        private TcpClient m_TcpClient;
        private NetworkStream m_TcpStream;
        private OneShotTimer m_Timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientStream"/> class.
        /// </summary>
        /// <param name="hostname">The host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hostname"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="hostname"/> is whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="port"/> is less or equal to zero, or more than 65535
        /// </exception>
        public TcpClientStream(string hostname, int port)
        {
            ArgumentNullException.ThrowIfNull(hostname);
            if (string.IsNullOrWhiteSpace(hostname))
                throw new ArgumentException(AppResources.InfraTcpStreamInvalidHostName, nameof(hostname));
            if (port <= 0 || port > 65535) throw new ArgumentOutOfRangeException(nameof(port));

            m_HostName = hostname;
            m_Port = port;
        }

        private volatile bool m_IsConnected;

        /// <summary>
        /// Connects this instance to the TCP server
        /// </summary>
        /// <returns>Is <see langword="true"/> if the connect succeeded, <see langword="false"/> otherwise.</returns>
        /// <exception cref="InvalidOperationException">TCP Stream already connected.</exception>
        /// <remarks>
        /// Attempts to connect. If the connect isn't made within <see cref="DisconnectTimeout"/>, or if the service
        /// refuses the connection, then <see langword="false"/> is returned. The exception is not propagated to the
        /// caller.
        /// </remarks>
        public bool Connect()
        {
            if (m_TcpClient is object)
                throw new InvalidOperationException(AppResources.InfraTcpStreamAlreadyConnected);

            object connectLock = new object();
            m_TcpClient = new TcpClient();

            OneShotTimer timer = new OneShotTimer(DisconnectTimeout);
            timer.TimerEvent += (s, e) => {
                lock (connectLock) {
                    if (m_IsConnected || m_TcpClient is null) return;
                    m_TcpClient.Close();
                }
            };
            timer.Start();

            try {
                m_TcpClient.Connect(m_HostName, m_Port);
            } catch (Exception ex) when (ex is SocketException ||
                                         ex is ObjectDisposedException) {
                lock (connectLock) {
                    timer.Dispose();
                    m_TcpClient.Dispose();
                    m_TcpClient = null;
                }
                return false;
            }
            lock (connectLock) {
                if (timer.Fired) return false;

                m_TcpStream = m_TcpClient.GetStream();
                m_TcpStream.ReadTimeout = ReadTimeout;
                m_TcpStream.WriteTimeout = WriteTimeout;
                m_IsConnected = true;
                timer.Dispose();
            }

            m_Timer = new OneShotTimer(DisconnectTimeout);
            m_Timer.TimerEvent += Timer_TimerEvent;
            return true;
        }

        /// <summary>
        /// Connect as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A task that returns <see langword="true"/> if the connect succeeded, <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="InvalidOperationException">This instance has already been connected.</exception>
        /// <remarks>
        /// Attempts to connect. If the connect isn't made within <see cref="DisconnectTimeout"/>, or if the service
        /// refuses the connection, then <see langword="false"/> is returned. The exception is not propagated to the
        /// caller.
        /// </remarks>
        public async Task<bool> ConnectAsync()
        {
            if (m_TcpClient is object)
                throw new InvalidOperationException(AppResources.InfraTcpStreamAlreadyConnected);

            object connectLock = new object();
            m_TcpClient = new TcpClient();

            OneShotTimer timer = new OneShotTimer(DisconnectTimeout);
            timer.TimerEvent += (s, e) => {
                lock (connectLock) {
                    if (m_IsConnected || m_TcpClient is null) return;
                    m_TcpClient.Dispose();
                }
            };
            timer.Start();

            try {
                await m_TcpClient.ConnectAsync(m_HostName, m_Port);
            } catch (Exception ex) when (ex is SocketException ||
                                         ex is ObjectDisposedException) {
                lock (connectLock) {
                    timer.Dispose();
                    m_TcpClient.Dispose();
                    m_TcpClient = null;
                }
                return false;
            }
            lock (connectLock) {
                if (timer.Fired) return false;

                m_TcpStream = m_TcpClient.GetStream();
                m_TcpStream.ReadTimeout = ReadTimeout;
                m_TcpStream.WriteTimeout = WriteTimeout;
                m_IsConnected = true;
            }

            m_Timer = new OneShotTimer(DisconnectTimeout);
            m_Timer.TimerEvent += Timer_TimerEvent;
            return true;
        }

        private void Timer_TimerEvent(object sender, TimerEventArgs e)
        {
            lock (m_DisposeLock) {
                if (m_IsDisposed) return;
                Dispose();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is connected; otherwise, <see langword="false"/>.</value>
        public bool IsConnected
        {
            get { return !m_IsDisposed && m_IsConnected && m_TcpClient.Connected; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can read; otherwise, <see langword="false"/>.</value>
        public override bool CanRead
        {
            get { return IsConnected; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can seek; otherwise, <see langword="false"/>.</value>
        /// <remarks>This stream cannot seek, so <see langword="false"/> is always returned.</remarks>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can write; otherwise, <see langword="false"/>.</value>
        public override bool CanWrite
        {
            get { return IsConnected; }
        }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can timeout; otherwise, <see langword="false"/>.</value>
        /// <remarks>This instance always returns <see langword="false"/> when not connected.</remarks>
        public override bool CanTimeout
        {
            get { return IsConnected && m_TcpStream.CanTimeout; }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>The length of the stream.</value>
        /// <exception cref="NotSupportedException">
        /// This property is not supported as <see cref="CanSeek"/> is <see langword="false"/>.
        /// </exception>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>The position.</value>
        /// <exception cref="NotSupportedException">
        /// This property is not supported as <see cref="CanSeek"/> is <see langword="false"/>.
        /// </exception>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        private int m_ReadTimeout = Timeout.Infinite;

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before
        /// timing out.
        /// </summary>
        /// <value>The read timeout in milliseconds.</value>
        /// <exception cref="ArgumentOutOfRangeException">This value is negative.</exception>
        public override int ReadTimeout
        {
            get { return m_ReadTimeout; }
            set
            {
                if (value != Timeout.Infinite && value < 0)
                    throw new ArgumentOutOfRangeException(nameof(ReadTimeout));

                m_ReadTimeout = value;
                if (IsConnected) m_TcpStream.ReadTimeout = m_ReadTimeout;
            }
        }

        private int m_WriteTimeout = Timeout.Infinite;

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before timing out.
        /// </summary>
        /// <value>The write timeout in milliseconds.</value>
        /// <exception cref="ArgumentOutOfRangeException">This value is negative.</exception>
        public override int WriteTimeout
        {
            get { return m_WriteTimeout; }
            set
            {
                if (value != Timeout.Infinite && value < 0)
                    throw new ArgumentOutOfRangeException(nameof(WriteTimeout));

                m_WriteTimeout = value;
                if (IsConnected) m_TcpStream.WriteTimeout = m_WriteTimeout;
            }
        }

        private int m_DisconnectTimeout = -1;

        /// <summary>
        /// Gets or sets the read disconnect timeout, in milliseconds, when a disconnect should occur on a read timeout.
        /// </summary>
        /// <value>The read disconnect timeout.</value>
        /// <exception cref="ArgumentOutOfRangeException">This value is negative.</exception>
        /// <remarks>
        /// If there is a timeout caused by the disconnect timeout, the stream is closed. Therefore it is advised that
        /// if this property is not <see cref="Timeout.Infinite"/> that the <see cref="ReadTimeout"/> is set to
        /// <see cref="Timeout.Infinite"/> to not confuse the results of a read.
        /// </remarks>
        public int DisconnectTimeout
        {
            get { return m_DisconnectTimeout; }
            set
            {
                if (value != Timeout.Infinite && value < 0)
                    throw new ArgumentOutOfRangeException(nameof(DisconnectTimeout));

                m_DisconnectTimeout = value;
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            if (IsConnected) m_TcpStream.Flush();
        }

        /// <summary>
        /// Asynchronously clears all buffers for this stream, causes any buffered data to be written to the underlying
        /// device, and monitors cancellation requests.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (IsConnected) return m_TcpStream.FlushAsync(cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number
        /// of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with the values
        /// between <paramref name="offset"/> and ( <paramref name="offset"/> + <paramref name="count"/> - 1) replaced
        /// by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the
        /// current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        /// many bytes are not currently available, or zero (0) if the end of the stream has been reached. It may also
        /// return zero (0) if there is a timeout within <see cref="ReadTimeout"/> or <see cref="DisconnectTimeout"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">TcpClientStream is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not connected, see <see cref="IsConnected"/>.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(TcpClientStream));
            if (!IsConnected) throw new InvalidOperationException(AppResources.InfraNetTcpStream_NotConnected);

            try {
                m_Timer.Reset();
                return m_TcpStream.Read(buffer, offset, count);
            } catch (Exception) {
                if (m_Timer.Fired) return 0;
                throw;
            }
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by
        /// the number of bytes read, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">
        /// The byte offset in <paramref name="buffer"/> at which to begin writing data from the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The value of the result parameter contains the total
        /// number of bytes read into the buffer. The result value can be less than the number of bytes requested if the
        /// number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of
        /// the stream has been reached. It may also return zero (0) if there is a timeout within
        /// <see cref="ReadTimeout"/> or <see cref="DisconnectTimeout"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">TcpClientStream is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not connected, see <see cref="IsConnected"/>.
        /// </exception>
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "Method wrapper")]
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(TcpClientStream));
            if (!IsConnected) throw new InvalidOperationException(AppResources.InfraNetTcpStream_NotConnected);

            try {
                m_Timer.Reset();

                // We have to await here, so that we start the task inside the try/catch block.
                return await m_TcpStream.ReadAsync(buffer, offset, count, cancellationToken);
            } catch (Exception) {
                if (m_Timer.Fired) return 0;
                throw;
            }
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by
        /// the number of bytes read, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The region of memory to write the data into.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The value of its result property contains the total
        /// number of bytes read into the buffer. The result value can be less than the number of bytes allocated in the
        /// buffer if that many bytes are not currently available, or it can be 0 (zero) if the end of the stream has
        /// been reached. It may also return zero (0) if there is a timeout within <see cref="ReadTimeout"/> or
        /// <see cref="DisconnectTimeout"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">TcpClientStream is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not connected, see <see cref="IsConnected"/>.
        /// </exception>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(TcpClientStream));
            if (!IsConnected) throw new InvalidOperationException(AppResources.InfraNetTcpStream_NotConnected);

            try {
                m_Timer.Reset();

                // We have to await here, so that we start the task inside the try/catch block.
                return await m_TcpStream.ReadAsync(buffer, cancellationToken);
            } catch (Exception) {
                if (m_Timer.Fired) return 0;
                throw;
            }
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.
        /// </param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="NotSupportedException">
        /// This property is not supported as <see cref="CanSeek"/> is <see langword="false"/>.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="NotSupportedException">
        /// This property is not supported as <see cref="CanSeek"/> is <see langword="false"/>.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the
        /// current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the
        /// current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current
        /// stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="ObjectDisposedException">TcpClientStream is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not connected, see <see cref="IsConnected"/>.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(TcpClientStream));
            if (!IsConnected) throw new InvalidOperationException(AppResources.InfraNetTcpStream_NotConnected);

            m_TcpStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this
        /// stream by the number of bytes written, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> from which to begin copying bytes to the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="ObjectDisposedException">TcpClientStream is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not connected, see <see cref="IsConnected"/>.
        /// </exception>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(TcpClientStream));
            if (!IsConnected) throw new InvalidOperationException(AppResources.InfraNetTcpStream_NotConnected);

            return m_TcpStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this
        /// stream by the number of bytes written, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The region of memory to write data from.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="ObjectDisposedException">TcpClientStream is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        /// The stream is not connected, see <see cref="IsConnected"/>.
        /// </exception>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(TcpClientStream));
            if (!IsConnected) throw new InvalidOperationException(AppResources.InfraNetTcpStream_NotConnected);

            return m_TcpStream.WriteAsync(buffer, cancellationToken);
        }

        private readonly object m_DisposeLock = new object();
        private bool m_IsDisposed;

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Stream"/> and optionally releases the managed
        /// resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!m_IsDisposed) {
                lock (m_DisposeLock) {
                    if (!m_IsDisposed) {
                        m_IsDisposed = true;
                        if (m_Timer is object) m_Timer.Dispose();

                        if (m_TcpClient is object) {
                            m_TcpClient.Close();
                            if (m_TcpStream is object) m_TcpStream.Dispose();
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
