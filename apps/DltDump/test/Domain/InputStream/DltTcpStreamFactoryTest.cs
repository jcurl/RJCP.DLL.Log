namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using Domain.Dlt;
    using NUnit.Framework;

    [TestFixture]
    public class DltTcpStreamFactoryTest
    {
        public enum Factory
        {
            InputStreamFactory,
            DltTcpFactory
        }

        private static IInputStreamFactory GetFactory(Factory factoryType)
        {
            switch (factoryType) {
            case Factory.InputStreamFactory:
                return new InputStreamFactory();
            case Factory.DltTcpFactory:
                return new DltTcpStreamFactory();
            default:
                throw new ArgumentException("Unknown factory");
            }
        }

        [Test]
        public void OpenNullUriString()
        {
            IInputStreamFactory factory = new DltTcpStreamFactory();
            Assert.That(() => {
                _ = factory.Create((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OpenNullUri()
        {
            IInputStreamFactory factory = new DltTcpStreamFactory();
            Assert.That(() => {
                _ = factory.Create((Uri)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(Factory.DltTcpFactory)]
        public void OpenEmptyConnectionString(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create(string.Empty);
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidNoHost(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp://");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidNoHost2(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp:");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithHostNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp:127.0.0.1");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithHostPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp:127.0.0.1:80");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithQuery(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp://127.0.0.1:80/?query");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithFragment(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp://127.0.0.1:80/#fragment");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithPath(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp://127.0.0.1:80/path");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithUserInfo1(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp://foo@127.0.0.1:80");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpInvalidWithUserInfo2(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create("tcp://foo:bar@127.0.0.1:80");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpLocalHostNameNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("tcp://localhost")) {
                Assert.That(stream, Is.TypeOf<DltTcpStream>());
                Assert.That(stream.Scheme, Is.EqualTo("tcp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpLocalHostNamePort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("tcp://localhost:123")) {
                Assert.That(stream, Is.TypeOf<DltTcpStream>());
                Assert.That(stream.Scheme, Is.EqualTo("tcp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpLocalHostNoPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("tcp://127.0.0.1")) {
                Assert.That(stream, Is.TypeOf<DltTcpStream>());
                Assert.That(stream.Scheme, Is.EqualTo("tcp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void TcpLocalHostPort(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("tcp://127.0.0.1:123")) {
                Assert.That(stream, Is.TypeOf<DltTcpStream>());
                Assert.That(stream.Scheme, Is.EqualTo("tcp"));
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Network));
                Assert.That(stream.IsLiveStream, Is.True);
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void OpenDisposedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create("tcp://127.0.0.1:123");
                input.Dispose();
                Assert.That(() => {
                    input.Open();
                }, Throws.TypeOf<ObjectDisposedException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void ConnectUnopenedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create("tcp://127.0.0.1:123");
                Assert.That(async () => {
                    _ = await input.ConnectAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltTcpFactory)]
        public void ConnectDisposedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create("tcp://127.0.0.1:123");
                input.Open();
                Assert.That(input.InputStream, Is.Not.Null);
                Assert.That(input.InputPacket, Is.Null);
                input.Dispose();
                Assert.That(async () => {
                    _ = await input.ConnectAsync();
                }, Throws.TypeOf<ObjectDisposedException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }
    }
}
