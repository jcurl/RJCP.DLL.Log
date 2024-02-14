namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Domain.Dlt;
    using Infrastructure.IO;
    using Infrastructure.Net;
    using NUnit.Framework;

    // To test integration tests under Linux:
    //
    // dotnet test --filter=FullyQualifiedName~RJCP.App.DltDump.Infrastructure.Net.DltUdpPacketReceiverTest
    // dotnet test --filter="FullyQualifiedName~RJCP.App.DltDump.Infrastructure.Net.DltUdpPacketReceiverTest&TestCategory=Integration"

    [TestFixture]
    public class DltUdpPacketReceiverTest
    {
        [Test]
        public void ConstructUnicast()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            using (DltUdpPacketReceiver receiver = new(address, 3490)) {
                Assert.That(receiver.Connection, Is.EqualTo("udp://127.0.0.1:3490"));
                Assert.That(receiver.InputStream, Is.Null);
                Assert.That(receiver.InputPacket, Is.Null);
                Assert.That(receiver.IsLiveStream, Is.True);
                Assert.That(receiver.RequiresConnection, Is.False);
                Assert.That(receiver.Scheme, Is.EqualTo("udp"));
                Assert.That(receiver.SuggestedFormat, Is.EqualTo(InputFormat.Network));
            }
        }

        [Test]
        public void ConstructUnicastAny()
        {
            using (DltUdpPacketReceiver receiver = new(IPAddress.Any, 3490)) {
                Assert.That(receiver.Connection, Is.EqualTo("udp://0.0.0.0:3490"));
                Assert.That(receiver.InputStream, Is.Null);
                Assert.That(receiver.InputPacket, Is.Null);
                Assert.That(receiver.IsLiveStream, Is.True);
                Assert.That(receiver.RequiresConnection, Is.False);
                Assert.That(receiver.Scheme, Is.EqualTo("udp"));
                Assert.That(receiver.SuggestedFormat, Is.EqualTo(InputFormat.Network));
            }
        }

        [Test]
        public void ConstructMulticast()
        {
            IPAddress address = IPAddress.Parse("239.200.200.10");
            using (DltUdpPacketReceiver receiver = new(address, 3490)) {
                Assert.That(receiver.Connection, Is.EqualTo("udp://239.200.200.10:3490"));
                Assert.That(receiver.InputStream, Is.Null);
                Assert.That(receiver.InputPacket, Is.Null);
                Assert.That(receiver.IsLiveStream, Is.True);
                Assert.That(receiver.RequiresConnection, Is.False);
                Assert.That(receiver.Scheme, Is.EqualTo("udp"));
                Assert.That(receiver.SuggestedFormat, Is.EqualTo(InputFormat.Network));
            }
        }

        [Test]
        public void ConstructNullAddress()
        {
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(null, 3490);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructPortZero()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(address, 0);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ConstructPortTooLarge()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(address, 65536);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ConstructPortNegative()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(address, -1);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ConstructUnicastIPv6()
        {
            IPAddress address = IPAddress.Parse("ff80::1");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(address, 3490);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructMulticastBind()
        {
            IPAddress bindAddress = IPAddress.Parse("10.0.0.1");
            IPAddress groupCast = IPAddress.Parse("224.100.150.255");
            using (DltUdpPacketReceiver receiver = new(bindAddress, 3490, groupCast)) {
                Assert.That(receiver.Connection, Is.EqualTo("udp://224.100.150.255:3490/?bindto=10.0.0.1"));
                Assert.That(receiver.InputStream, Is.Null);
                Assert.That(receiver.InputPacket, Is.Null);
                Assert.That(receiver.IsLiveStream, Is.True);
                Assert.That(receiver.RequiresConnection, Is.False);
                Assert.That(receiver.Scheme, Is.EqualTo("udp"));
                Assert.That(receiver.SuggestedFormat, Is.EqualTo(InputFormat.Network));
            }
        }

        [Test]
        public void ConstructInvalidMulticastBind()
        {
            IPAddress bindAddress = IPAddress.Parse("10.0.0.1");
            IPAddress groupCast = IPAddress.Parse("192.168.1.1");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(bindAddress, 3490, groupCast);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructBindNullAddress()
        {
            IPAddress groupCast = IPAddress.Parse("224.100.150.255");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(null, 3490, groupCast);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructGroupNullAddress()
        {
            IPAddress bindAddress = IPAddress.Parse("10.0.0.1");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(bindAddress, 3490, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructMulticastPortZero()
        {
            IPAddress bindAddress = IPAddress.Parse("127.0.0.1");
            IPAddress groupCast = IPAddress.Parse("224.100.150.255");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(bindAddress, 0, groupCast);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ConstructMulticastPortTooLarge()
        {
            IPAddress bindAddress = IPAddress.Parse("127.0.0.1");
            IPAddress groupCast = IPAddress.Parse("224.100.150.255");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(bindAddress, 65536, groupCast);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ConstructMulticastPortNegative()
        {
            IPAddress bindAddress = IPAddress.Parse("127.0.0.1");
            IPAddress groupCast = IPAddress.Parse("224.100.150.255");
            Assert.That(() => {
                _ = new DltUdpPacketReceiver(bindAddress, -1, groupCast);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenLocalClose()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            using (DltUdpPacketReceiver receiver = new(address, 3490)) {
                receiver.Open();
                Assert.That(receiver.InputPacket, Is.Not.Null);
                Assert.That(receiver.InputPacket, Is.TypeOf<UdpPacketReceiver>());

                receiver.Close();
                Assert.That(receiver.InputPacket, Is.Null);
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenLocalDispose()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            DltUdpPacketReceiver receiver;
            using (receiver = new DltUdpPacketReceiver(address, 3490)) {
                receiver.Open();
                Assert.That(receiver.InputPacket, Is.Not.Null);
                Assert.That(receiver.InputPacket, Is.TypeOf<UdpPacketReceiver>());
            }

            Assert.That(receiver.InputPacket, Is.Null);
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public async Task OpenLocalConnectClose()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            using (DltUdpPacketReceiver receiver = new(address, 3490)) {
                receiver.Open();
                Assert.That(receiver.InputPacket, Is.Not.Null);
                Assert.That(receiver.InputPacket, Is.TypeOf<UdpPacketReceiver>());

                Assert.That(await receiver.ConnectAsync(), Is.True);

                receiver.Close();
                Assert.That(receiver.InputPacket, Is.Null);
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenTwice()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            using (DltUdpPacketReceiver receiver = new(address, 3490)) {
                receiver.Open();
                IPacket packet = receiver.InputPacket;
                Assert.That(packet, Is.Not.Null);
                Assert.That(packet, Is.TypeOf<UdpPacketReceiver>());

                // No exception, this is ignored.
                receiver.Open();
                Assert.That(receiver.InputPacket, Is.SameAs(packet));
            }
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void OpenWhenDisposed()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            DltUdpPacketReceiver receiver = null;
            using (receiver = new DltUdpPacketReceiver(address, 3490)) {
                // Nothing to do
            }

            Assert.That(() => {
                receiver.Open();
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void ConnectWhenDisposed()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            DltUdpPacketReceiver receiver = null;
            using (receiver = new DltUdpPacketReceiver(address, 3490)) {
                receiver.Open();
            }

            Assert.That(async () => {
                await receiver.ConnectAsync();
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void ConnectWhenNotOpen()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            DltUdpPacketReceiver receiver = null;
            using (receiver = new DltUdpPacketReceiver(address, 3490)) {
                Assert.That(async () => {
                    await receiver.ConnectAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }
    }
}
