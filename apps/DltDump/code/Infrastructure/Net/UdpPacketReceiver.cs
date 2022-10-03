namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using Resources;

    /// <summary>
    /// A class that can listen and bind an interface to receive UDP packets.
    /// </summary>
    public class UdpPacketReceiver : IPacket
    {
        private readonly IPEndPoint m_BindAddress;
        private readonly IPAddress m_MulticastGroup;

        private readonly object m_ChannelLock = new object();
        private readonly List<IPEndPoint> m_Channels = new List<IPEndPoint>();

        private Socket m_Socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpPacketReceiver"/> class.
        /// </summary>
        /// <param name="endPoint">The end point to bind and listen to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="endPoint"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="endPoint"/> is not an IPv4 address;
        /// <para>- or -</para>
        /// <paramref name="endPoint"/> port is out of range (1..65535);
        /// </exception>
        /// <remarks>
        /// If <paramref name="endPoint"/> is a multicast address, the socket is configured to bind to all addresses and
        /// to join the multicast group. If you want to bind only to a specific address, specify with
        /// <see cref="UdpPacketReceiver(IPEndPoint, IPAddress)"/>.
        /// </remarks>
        public UdpPacketReceiver(IPEndPoint endPoint)
        {
            if (endPoint == null) throw new ArgumentNullException(nameof(endPoint));

            if (endPoint.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException(AppResources.InfraUdpReceiverInvalidFamily, nameof(endPoint));

            // It's not possible to assign an EndPoint with a port of 65536 or larger, but we make it explicit anyway.
            if (endPoint.Port <= 0 || endPoint.Port > 65535) {
                string message = string.Format(AppResources.InfraUdpReceiverInvalidPort, endPoint.Port);
                throw new ArgumentException(message, nameof(endPoint));
            }

            if (IsMulticast(endPoint.Address)) {
                m_BindAddress = new IPEndPoint(IPAddress.Any, endPoint.Port);
                m_MulticastGroup = endPoint.Address;
            } else {
                m_BindAddress = endPoint;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpPacketReceiver"/> class.
        /// </summary>
        /// <param name="bindAddr">
        /// The local address to bind to (must be an existing interface, or <see cref="IPAddress.Any"/>).
        /// </param>
        /// <param name="multicastGroup">The multicast group to join.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bindAddr"/> or <paramref name="multicastGroup"/> is <see langword="null"/>.
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
        public UdpPacketReceiver(IPEndPoint bindAddr, IPAddress multicastGroup)
        {
            if (bindAddr == null) throw new ArgumentNullException(nameof(bindAddr));
            if (multicastGroup == null) throw new ArgumentNullException(nameof(multicastGroup));

            if (bindAddr.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException(AppResources.InfraUdpReceiverInvalidFamily, nameof(bindAddr));
            if (multicastGroup.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException(AppResources.InfraUdpReceiverInvalidFamily, nameof(multicastGroup));

            if (!IsMulticast(multicastGroup)) {
                string message = string.Format(AppResources.InfraUdpReceiverNotMulticast, multicastGroup);
                throw new ArgumentException(message, nameof(multicastGroup));
            }

            if (IsMulticast(bindAddr.Address)) {
                string message = string.Format(AppResources.InfraUdpReceiverNotUnicast, bindAddr);
                throw new ArgumentException(message, nameof(bindAddr));
            }

            // It's not possible to assign an EndPoint with a port of 65536 or larger, but we make it explicit anyway.
            if (bindAddr.Port <= 0 || bindAddr.Port > 65535) {
                string message = string.Format(AppResources.InfraUdpReceiverInvalidPort, bindAddr.Port);
                throw new ArgumentException(message, nameof(bindAddr));
            }

            m_BindAddress = bindAddr;
            m_MulticastGroup = multicastGroup;
        }

        private static bool IsMulticast(IPAddress addr)
        {
            byte[] addrBytes = addr.GetAddressBytes();
            return (addrBytes[0] & 0xF0) == 0xE0;
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="UdpPacketReceiver"/> is already open.
        /// </exception>
        /// <exception cref="SocketException">An error occurred when attempting to access the socket.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// A caller higher in the call stack does not have permission for the requested operation.
        /// </exception>
        /// <exception cref="ObjectDisposedException">This object is already disposed.</exception>
        public void Open()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(UdpPacketReceiver));

            if (m_Socket != null)
                throw new InvalidOperationException(AppResources.InfraUdpReceiverOpen);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            try {
                if (m_MulticastGroup != null) {
                    MulticastOption mcastOption
                        = new MulticastOption(m_MulticastGroup, m_BindAddress.Address);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcastOption);
                }
                socket.Bind(m_BindAddress);
            } catch {
                socket.Close();
                throw;
            }

            m_Socket = socket;
        }

        /// <summary>
        /// Reads data from the source asynchronously.
        /// </summary>
        /// <param name="buffer">The buffer where the data is written to.</param>
        /// <param name="token">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        /// <exception cref="SocketException">
        /// An error occurred while waiting for data. For example calling <see cref="Close()"/> will result in this
        /// exception.
        /// </exception>
        /// <returns>
        /// A task that has the <see cref="Tuple"/> result with the number of bytes read, and the channel identifier
        /// where the data originated. Data always originates from the first value of channel 0, and increments for the
        /// lifetime of the source.
        /// </returns>
        public async ValueTask<PacketReadResult> ReadAsync(Memory<byte> buffer)
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(UdpPacketReceiver));

            if (m_Socket == null)
                throw new InvalidOperationException(AppResources.InfraUdpReceiverNotOpen);

            if (m_Socket.Available > 0)
                return ReadInternal(buffer);

            return await Task.Run(() => ReadInternal(buffer));
        }

        private PacketReadResult ReadInternal(Memory<byte> buffer)
        {
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            try {
#if NET6_0_OR_GREATER
                // This is the quickest fix, but ReceiveFromAsync would be better.
                int read = m_Socket.ReceiveFrom(buffer.Span, SocketFlags.None, ref remote);
#else
                // .NET 5 and earlier doesn't have a ReceiveFrom() that works with a Memory<byte>. So we need to convert
                // the buffer to an array in place.
                byte[] localBuff;
                if (!MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> segment))
                    return new PacketReadResult(0, 0);
                localBuff = segment.Array;
                int read = m_Socket.ReceiveFrom(localBuff, 0, localBuff.Length, SocketFlags.None, ref remote);
#endif
                lock (m_ChannelLock) {
                    // As we don't expect to have many different sources, a list with a loop is likely faster.
                    IPEndPoint remoteIp = (IPEndPoint)remote;
                    for (int i = 0; i < m_Channels.Count; i++) {
                        if (m_Channels[i].Address.Equals(remoteIp.Address) && m_Channels[i].Port == remoteIp.Port) {
                            return new PacketReadResult(read, i);
                        }
                    }

                    m_Channels.Add(remoteIp);
                    return new PacketReadResult(read, m_Channels.Count - 1);
                }
            } catch (SocketException ex) {
                if (m_IsDisposed && ex.SocketErrorCode == SocketError.Interrupted)
                    throw new ObjectDisposedException(nameof(UdpPacketReceiver));
                throw;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (m_IsDisposed || m_Socket == null) return;

            m_Socket.Close();
            m_Socket = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed, or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private volatile bool m_IsDisposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                if (!m_IsDisposed) {
                    m_IsDisposed = true;
                    if (m_Socket != null) {
                        m_Socket.Close();
                        m_Socket = null;
                    }
                }
            }
        }
    }
}
