namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using Dlt;
    using NUnit.Framework;

    [TestFixture]
    public class DltUdpPacketReceiverFactoryTest
    {
        public enum Factory
        {
            InputStreamFactory,
            DltUdpFactory
        }

        private static IInputStreamFactory GetFactory(Factory factoryType)
        {
            switch (factoryType) {
            case Factory.InputStreamFactory:
                return new InputStreamFactory();
            case Factory.DltUdpFactory:
                return new DltUdpPacketReceiverFactory();
            default:
                throw new ArgumentException("Unknown factory");
            }
        }

        [Test]
        public void OpenNullUriString()
        {
            IInputStreamFactory factory = new DltUdpPacketReceiverFactory();
            Assert.That(() => {
                _ = factory.Create((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OpenNullUri()
        {
            IInputStreamFactory factory = new DltUdpPacketReceiverFactory();
            Assert.That(() => {
                _ = factory.Create((Uri)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(Factory.DltUdpFactory)]
        public void OpenEmptyConnectionString(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create(string.Empty);
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidNoHost(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidNoHost2(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp:");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidWithHostNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp:127.0.0.1");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidWithHostPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp:127.0.0.1:3490");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidWithFragment(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://127.0.0.1:80/#fragment");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidWithPath(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://127.0.0.1:80/path");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidWithUserInfo1(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://foo@127.0.0.1:80");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpInvalidWithUserInfo2(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://foo:bar@127.0.0.1:80");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostNameNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://localhost");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostNamePort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("udp://localhost:3490");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://127.0.0.1")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://127.0.0.1:123")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpMulticastHostNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://224.10.10.10")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpMulticastHostPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://224.10.10.10:123")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostBindNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://224.10.10.10/?bindto=127.0.0.1")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostBindNoPort2(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://224.10.10.10?bindto=127.0.0.1")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostBindPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://224.10.10.10:123/?bindto=127.0.0.1")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltUdpFactory)]
        public void UdpLocalHostBindPort2(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("udp://224.10.10.10:123?bindto=127.0.0.1")) {
                Assert.That(stream, Is.TypeOf<DltUdpPacketReceiver>());
                Assert.That(stream.Scheme, Is.EqualTo("udp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }
    }
}
