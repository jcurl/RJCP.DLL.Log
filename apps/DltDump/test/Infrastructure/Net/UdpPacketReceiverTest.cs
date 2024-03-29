﻿namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Versioning;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using NUnit.Framework;

    // To test integration tests under Linux:
    //
    // dotnet test --filter=FullyQualifiedName~RJCP.App.DltDump.Infrastructure.Net.UdpPacketReceiverTest
    // dotnet test --filter="FullyQualifiedName~RJCP.App.DltDump.Infrastructure.Net.UdpPacketReceiverTest&TestCategory=Integration"

    [TestFixture]
    public class UdpPacketReceiverTest
    {
        [Test]
        public void NullEndPoint()
        {
            Assert.That(() => {
                _ = new UdpPacketReceiver(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvalidIPv6Family()
        {
            IPEndPoint ep = new(IPAddress.Parse("::1"), 3490);
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidIPv4Port0()
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 0);
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void UnicastAnyEndPoint()
        {
            IPEndPoint ep = new(IPAddress.Any, 3490);
            UdpPacketReceiver receiver = new(ep);
            Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            receiver.Dispose();
        }

        [Test]
        public void UnicastEndPoint()
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 3490);
            UdpPacketReceiver receiver = new(ep);
            Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            receiver.Dispose();
        }

        [Test]
        public void MulticastEndPoint()
        {
            IPEndPoint ep = new(IPAddress.Parse("224.0.1.1"), 3490);
            UdpPacketReceiver receiver = new(ep);
            Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            receiver.Dispose();
        }

        [Test]
        public void NullBindEndPoint()
        {
            Assert.That(() => {
                _ = new UdpPacketReceiver(null, IPAddress.Parse("224.0.1.1"));
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullMulticastAddr()
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 3490);
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvalidBindIPv6Family()
        {
            IPEndPoint ep = new(IPAddress.Parse("::1"), 3490);
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep, IPAddress.Parse("224.0.1.1"));
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidMulticastIPv6Family()
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 3490);
            IPAddress mc = IPAddress.Parse("ff00::1");
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep, mc);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidBindIPv4Port0()
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 0);
            IPAddress mc = IPAddress.Parse("224.0.1.1");
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep, mc);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InvalidBindMulticast()
        {
            IPEndPoint ep = new(IPAddress.Parse("224.0.0.1"), 3490);
            IPAddress mc = IPAddress.Parse("224.0.1.1");
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep, mc);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidMulticastAsUnicast()
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 3490);
            IPAddress mc = IPAddress.Parse("10.0.1.1");
            Assert.That(() => {
                _ = new UdpPacketReceiver(ep, mc);
            }, Throws.TypeOf<ArgumentException>());
        }

        [TestCase("224.0.0.0")]
        [TestCase("224.0.0.0")]
        [TestCase("239.255.255.255")]
        public void BindMulticast(string multicastGroup)
        {
            IPEndPoint ep = new(IPAddress.Parse("10.0.0.1"), 3490);
            IPAddress mc = IPAddress.Parse(multicastGroup);
            UdpPacketReceiver receiver = new(ep, mc);
            Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            receiver.Dispose();
        }

        [TestCase("224.0.0.0")]
        [TestCase("224.0.0.0")]
        [TestCase("239.255.255.255")]
        public void BindMulticastAny(string multicastGroup)
        {
            IPEndPoint ep = new(IPAddress.Any, 3490);
            IPAddress mc = IPAddress.Parse(multicastGroup);
            UdpPacketReceiver receiver = new(ep, mc);
            Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            receiver.Dispose();
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenLocalHost()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                receiver.Open();
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenLocalHostCloseAfterDispose()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            UdpPacketReceiver receiver = null;
            try {
                receiver = new UdpPacketReceiver(ep);
                receiver.Open();
                receiver.Dispose();
                receiver.Close();
                receiver = null;
            } finally {
                if (receiver is not null) receiver.Dispose();
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenLocalHostDisposeAfterClose()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            UdpPacketReceiver receiver = null;
            try {
                receiver = new UdpPacketReceiver(ep);
                receiver.Open();
                receiver.Close();
            } finally {
                if (receiver is not null) receiver.Dispose();
            }
        }

        [Test]
        public void OpenAfterDispose()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            UdpPacketReceiver receiver = new(ep);
            receiver.Dispose();
            Assert.That(() => {
                receiver.Open();
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void CloseAfterInstantiation()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                receiver.Close();
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenTwice()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                receiver.Open();
                Assert.That(() => {
                    receiver.Open();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        private sealed class Udp : IDisposable
        {
            private readonly IPEndPoint m_Destination;
            private readonly Socket m_Socket;

            private static bool IsMulticast(IPAddress addr)
            {
                byte[] addrBytes = addr.GetAddressBytes();
                if ((addrBytes[0] & 0xF0) == 0xE0) return true;
                return false;
            }

            public Udp(IPEndPoint srcAddr, IPEndPoint destAddr)
            {
                m_Destination = destAddr;
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                if (!IsMulticast(destAddr.Address)) m_Socket.Bind(srcAddr);
            }

            public static Udp Send(IPEndPoint srcAddr, IPEndPoint destAddr)
            {
                Udp sender = null;
                try {
                    sender = new Udp(srcAddr, destAddr);
                    return sender.Send();
                } catch {
                    if (sender is not null) sender.Dispose();
                    throw;
                }
            }

            public Udp Send()
            {
                m_Socket.SendTo(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }, m_Destination);
                return this;
            }

            private bool m_IsDisposed;

            public void Dispose()
            {
                if (!m_IsDisposed) {
                    m_Socket.Dispose();
                    m_IsDisposed = true;
                }
            }
        }

        [TestCase("127.0.0.1")]
        [TestCase("224.0.100.1")]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveAsync(string endpoint)
        {
            IPEndPoint src = new(IPAddress.Parse("127.0.0.1"), 8000);
            IPEndPoint ep = new(IPAddress.Parse(endpoint), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                int count = -1;
                receiver.NewChannel += (s, e) => {
                    if (count <= e.ChannelNumber) count = e.ChannelNumber;
                };
                receiver.Open();
                Assert.That(receiver.ChannelCount, Is.EqualTo(0));

                byte[] buffer = new byte[65536];
                Udp.Send(src, ep).Dispose();
                PacketReadResult result = await receiver.ReadAsync(buffer);
                Assert.That(result.ReceivedBytes, Is.EqualTo(8));
                Assert.That(result.Channel, Is.EqualTo(0));
                Assert.That(buffer[0..8],
                    Is.EqualTo(
                        new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                    ).AsCollection);

                Assert.That(count, Is.EqualTo(0));
                Assert.That(receiver.ChannelCount, Is.EqualTo(1));
            }
        }

        [TestCase("127.0.0.1")]
        [TestCase("224.0.100.1")]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveAsyncWait(string endpoint)
        {
            IPEndPoint src = new(IPAddress.Parse("127.0.0.1"), 8000);
            IPEndPoint ep = new(IPAddress.Parse(endpoint), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                int count = -1;
                receiver.NewChannel += (s, e) => {
                    if (count <= e.ChannelNumber) count = e.ChannelNumber;
                };
                receiver.Open();
                Assert.That(receiver.ChannelCount, Is.EqualTo(0));

                byte[] buffer = new byte[65536];
                ValueTask<PacketReadResult> task = receiver.ReadAsync(buffer);
                Thread.Sleep(100);
                Udp.Send(src, ep).Dispose();

                PacketReadResult result = await task;
                Assert.That(result.ReceivedBytes, Is.EqualTo(8));
                Assert.That(result.Channel, Is.EqualTo(0));
                Assert.That(buffer[0..8],
                    Is.EqualTo(
                        new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                    ).AsCollection);

                Assert.That(count, Is.EqualTo(0));
                Assert.That(receiver.ChannelCount, Is.EqualTo(1));
            }
        }

        [TestCase("127.0.0.1")]
        [TestCase("224.0.100.1")]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveAsync2(string endpoint)
        {
            IPEndPoint src1 = new(IPAddress.Parse("127.0.0.1"), 8000);
            IPEndPoint src2 = new(IPAddress.Parse("127.0.0.1"), 8001);
            IPEndPoint ep = new(IPAddress.Parse(endpoint), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                int count = -1;
                receiver.NewChannel += (s, e) => {
                    if (count <= e.ChannelNumber) count = e.ChannelNumber;
                };
                receiver.Open();
                Assert.That(receiver.ChannelCount, Is.EqualTo(0));

                byte[] buffer = new byte[65536];
                Udp.Send(src1, ep).Dispose();
                PacketReadResult result1 = await receiver.ReadAsync(buffer);
                Assert.That(result1.ReceivedBytes, Is.EqualTo(8));
                Assert.That(result1.Channel, Is.EqualTo(0));
                Assert.That(receiver.ChannelCount, Is.EqualTo(1));
                Assert.That(buffer[0..8],
                    Is.EqualTo(
                        new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                    ).AsCollection);

                buffer = new byte[65536];
                Udp.Send(src2, ep).Dispose();
                PacketReadResult result2 = await receiver.ReadAsync(buffer);
                Assert.That(result2.ReceivedBytes, Is.EqualTo(8));
                Assert.That(result2.Channel, Is.EqualTo(1));
                Assert.That(receiver.ChannelCount, Is.EqualTo(2));
                Assert.That(buffer[0..8],
                    Is.EqualTo(
                        new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                    ).AsCollection);

                Assert.That(count, Is.EqualTo(1));
            }
        }

        [TestCase("127.0.0.1")]
        [TestCase("224.0.100.1")]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveAsync3(string endpoint)
        {
            IPEndPoint src1 = new(IPAddress.Parse("127.0.0.1"), 8000);
            IPEndPoint src2 = new(IPAddress.Parse("127.0.0.1"), 8001);
            IPEndPoint ep = new(IPAddress.Parse(endpoint), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                int count = -1;
                receiver.NewChannel += (s, e) => {
                    if (count <= e.ChannelNumber) count = e.ChannelNumber;
                };
                receiver.Open();
                Assert.That(receiver.ChannelCount, Is.EqualTo(0));

                using (Udp udp1 = new(src1, ep))
                using (Udp udp2 = new(src2, ep)) {
                    byte[] buffer = new byte[65536];
                    udp1.Send();
                    PacketReadResult result1 = await receiver.ReadAsync(buffer);
                    Assert.That(result1.ReceivedBytes, Is.EqualTo(8));
                    Assert.That(result1.Channel, Is.EqualTo(0));
                    Assert.That(receiver.ChannelCount, Is.EqualTo(1));
                    Assert.That(count, Is.EqualTo(0));
                    Assert.That(buffer[0..8],
                        Is.EqualTo(
                            new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                        ).AsCollection);

                    buffer = new byte[65536];
                    udp2.Send();
                    PacketReadResult result2 = await receiver.ReadAsync(buffer);
                    Assert.That(result2.ReceivedBytes, Is.EqualTo(8));
                    Assert.That(result2.Channel, Is.EqualTo(1));
                    Assert.That(receiver.ChannelCount, Is.EqualTo(2));
                    Assert.That(count, Is.EqualTo(1));
                    Assert.That(buffer[0..8],
                        Is.EqualTo(
                            new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                        ).AsCollection);

                    buffer = new byte[65536];
                    udp1.Send();
                    result1 = await receiver.ReadAsync(buffer);
                    Assert.That(result1.ReceivedBytes, Is.EqualTo(8));
                    Assert.That(result1.Channel, Is.EqualTo(0));
                    Assert.That(receiver.ChannelCount, Is.EqualTo(2));
                    Assert.That(count, Is.EqualTo(1));
                    Assert.That(buffer[0..8],
                        Is.EqualTo(
                            new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                        ).AsCollection);

                    buffer = new byte[65536];
                    udp2.Send();
                    result2 = await receiver.ReadAsync(buffer);
                    Assert.That(result2.ReceivedBytes, Is.EqualTo(8));
                    Assert.That(result2.Channel, Is.EqualTo(1));
                    Assert.That(receiver.ChannelCount, Is.EqualTo(2));
                    Assert.That(count, Is.EqualTo(1));
                    Assert.That(buffer[0..8],
                        Is.EqualTo(
                            new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }
                        ).AsCollection);
                }
            }
        }

        [Test]
        public async Task ReceiveWhenNotOpen()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                byte[] buffer = new byte[65536];
                await Assert.ThatAsync(async () => {
                    _ = await receiver.ReadAsync(buffer);
                }, Throws.TypeOf<InvalidOperationException>());
                Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveWhenClosed()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                byte[] buffer = new byte[65536];
                receiver.Open();
                receiver.Close();

                await Assert.ThatAsync(async () => {
                    _ = await receiver.ReadAsync(buffer);
                }, Throws.TypeOf<InvalidOperationException>());
                Assert.That(receiver.ChannelCount, Is.EqualTo(0));
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveWhenDisposed()
        {
            IPEndPoint ep = new(IPAddress.Parse("127.0.0.1"), 3490);
            UdpPacketReceiver receiver = null;
            try {
                receiver = new UdpPacketReceiver(ep);
                receiver.Dispose();

                byte[] buffer = new byte[65536];
                await Assert.ThatAsync(async () => {
                    _ = await receiver.ReadAsync(buffer);
                }, Throws.TypeOf<ObjectDisposedException>());
                receiver = null;
            } finally {
                if (receiver is not null) receiver.Dispose();
            }
        }

        [TestCase("127.0.0.1")]
        [TestCase("224.0.100.1")]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveAsyncClose(string endpoint)
        {
            IPEndPoint ep = new(IPAddress.Parse(endpoint), 3490);
            using (UdpPacketReceiver receiver = new(ep)) {
                receiver.Open();
                byte[] buffer = new byte[65536];

                ValueTask<PacketReadResult> task = receiver.ReadAsync(buffer);
                Thread.Sleep(300);
                Assert.That(task.IsCompleted, Is.False);
                receiver.Close(); // Should cause thread to abort

                await Assert.ThatAsync(async () => {
                    _ = await task;
                }, Throws.TypeOf<SocketException>()
                    .With.Property("SocketErrorCode").EqualTo(SocketError.Interrupted).Or
                    .With.Property("SocketErrorCode").EqualTo(SocketError.OperationAborted));
            }
        }

        [TestCase("127.0.0.1")]
        [TestCase("224.0.100.1")]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [CancelAfter(5000)]
        public async Task ReceiveAsyncDispose(string endpoint)
        {
            IPEndPoint ep = new(IPAddress.Parse(endpoint), 3490);
            UdpPacketReceiver receiver = null;
            try {
                receiver = new UdpPacketReceiver(ep);
                receiver.Open();
                byte[] buffer = new byte[65536];

                ValueTask<PacketReadResult> task = receiver.ReadAsync(buffer);
                Thread.Sleep(300);
                Assert.That(task.IsCompleted, Is.False);
                receiver.Dispose(); // Should cause thread to abort

                await Assert.ThatAsync(async () => {
                    _ = await task;
                }, Throws.TypeOf<ObjectDisposedException>());
                receiver = null;
            } finally {
                if (receiver is not null) receiver.Dispose();
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        [Platform("Win", Reason = "Exception isn't raised on Linux")]
        [SupportedOSPlatform("windows")]
        [CancelAfter(5000)]
        public void OpenInvalidAddress()
        {
            IPEndPoint ep = new(IPAddress.Parse("255.255.255.255"), 3490);
            Assert.That(() => {
                using (UdpPacketReceiver receiver = new(ep)) {
                    receiver.Open();
                }
            }, Throws.TypeOf<SocketException>());
        }
    }
}
