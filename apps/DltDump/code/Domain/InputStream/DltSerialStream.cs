namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;
    using Resources;
    using RJCP.IO.Ports;

    /// <summary>
    /// Provides an input stream for DLT serial streams and recommend the serial header.
    /// </summary>
    public sealed class DltSerialStream : IInputStream
    {
        private readonly string m_PortName;
        private readonly int m_Baud;
        private readonly int m_Data;
        private readonly Parity m_Parity;
        private readonly StopBits m_StopBits;
        private readonly Handshake m_Handshake;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSerialStream"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="baud">The baud.</param>
        /// <param name="data">The data.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="stopbits">The stopbits.</param>
        /// <param name="handshake">The handshake.</param>
        /// <exception cref="ArgumentNullException">port</exception>
        /// <exception cref="ArgumentOutOfRangeException">baud</exception>
        /// <exception cref="ArgumentOutOfRangeException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException">parity</exception>
        /// <exception cref="ArgumentOutOfRangeException">stopbits</exception>
        /// <exception cref="ArgumentOutOfRangeException">handshake</exception>
        public DltSerialStream(string port, int baud, int data, Parity parity, StopBits stopbits, Handshake handshake)
        {
            ArgumentNullException.ThrowIfNull(port);
            if (string.IsNullOrWhiteSpace(port))
                throw new ArgumentException(AppResources.SerialOpenError_InvalidPort, nameof(port));
            if (baud <= 0)
                throw new ArgumentOutOfRangeException(nameof(baud), AppResources.SerialOpenError_InvalidBaud);
            if (data is < 5 or > 8)
                throw new ArgumentOutOfRangeException(nameof(data), AppResources.SerialOpenError_InvalidDataBits);
            if (!Enum.IsDefined(typeof(Parity), parity))
                throw new ArgumentOutOfRangeException(nameof(parity), AppResources.SerialOpenError_InvalidParity);
            if (!Enum.IsDefined(typeof(StopBits), stopbits))
                throw new ArgumentOutOfRangeException(nameof(stopbits), AppResources.SerialOpenError_InvalidStopBits);
            if (!Enum.IsDefined(typeof(Handshake), handshake))
                throw new ArgumentOutOfRangeException(nameof(handshake), AppResources.SerialOpenError_InvalidHandshake);

            m_PortName = port;
            m_Baud = baud;
            m_Data = data;
            m_Parity = parity;
            m_StopBits = stopbits;
            m_Handshake = handshake;

            if (handshake == Handshake.None) {
                Connection = string.Format("ser:{0},{1},{2},{3},{4}",
                    port, baud, data, GetParity(parity), GetStopBits(stopbits));
            } else {
                Connection = string.Format("ser:{0},{1},{2},{3},{4},{5}",
                    port, baud, data, GetParity(parity), GetStopBits(stopbits), GetHandshake(handshake));
            }
        }

        private static string GetParity(Parity parity)
        {
            switch (parity) {
            case Parity.None: return "N";
            case Parity.Even: return "E";
            case Parity.Odd: return "O";
            case Parity.Mark: return "M";
            case Parity.Space: return "S";
            default: return "?";
            }
        }

        private static string GetStopBits(StopBits stopbits)
        {
            switch (stopbits) {
            case StopBits.One: return "1";
            case StopBits.One5: return "1.5";
            case StopBits.Two: return "2";
            default: return "?";
            }
        }

        private static string GetHandshake(Handshake handshake)
        {
            if (!Enum.IsDefined(typeof(Handshake), handshake)) return "?";
            return handshake.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Gets the input scheme.
        /// </summary>
        /// <value>The input scheme.</value>
        public string Scheme { get { return "ser"; } }

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
        /// input stream. This stream always returns <see langword="false"/>.
        /// </remarks>
        public bool IsLiveStream { get { return true; } }

        /// <summary>
        /// Gets the suggested format that should be used for instantiating a decoder.
        /// </summary>
        /// <value>The suggested format, which is <see cref="InputFormat.Serial"/>.</value>
        public InputFormat SuggestedFormat { get { return InputFormat.Serial; } }

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
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        public void Open()
        {
            if (m_IsDisposed) throw new ObjectDisposedException(nameof(DltSerialStream));
            if (InputStream is not null) return;

            SerialPortStream stream = new() {
                PortName = m_PortName,
                BaudRate = m_Baud,
                DataBits = m_Data,
                Parity = m_Parity,
                StopBits = m_StopBits,
                Handshake = m_Handshake,
                ReadBufferSize = 262144
            };
            stream.Open();
            InputStream = stream;
        }

        /// <summary>
        /// Connects the input stream asynchronously (e.g. for network streams).
        /// </summary>
        /// <returns>
        /// Returns if the input stream was connected. As a file is never connected, this always returns
        /// <see langword="true"/>.
        /// </returns>
        public Task<bool> ConnectAsync()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(DltSerialStream));
            if (InputStream is null)
                throw new InvalidOperationException(AppResources.DomainInputStreamNotOpen);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Closes this stream, but does not dispose, so it can be reopened.
        /// </summary>
        public void Close()
        {
            if (InputStream is not null) InputStream.Close();
            InputStream = null;
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and/or unmanaged
        /// resources.
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
