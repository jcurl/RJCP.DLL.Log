namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using Domain.Dlt;
    using NUnit.Framework;

    [TestFixture]
    public class DltSerialStreamFactoryTest
    {
        public enum Factory
        {
            InputStreamFactory,
            DltSerialFactory
        }

        private static IInputStreamFactory GetFactory(Factory factoryType)
        {
            switch (factoryType) {
            case Factory.InputStreamFactory:
                return new InputStreamFactory();
            case Factory.DltSerialFactory:
                return new DltSerialStreamFactory();
            default:
                throw new ArgumentException("Unknown factory");
            }
        }

        [Test]
        public void OpenNullUriString()
        {
            IInputStreamFactory factory = new DltSerialStreamFactory();
            Assert.That(() => {
                _ = factory.Create((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OpenNullUri()
        {
            IInputStreamFactory factory = new DltSerialStreamFactory();
            Assert.That(() => {
                _ = factory.Create((Uri)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(Factory.DltSerialFactory)]
        public void OpenEmptyUri(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create(string.Empty);
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.DltSerialFactory, "ser:")]
        [TestCase(Factory.DltSerialFactory, "ser://com1,115200,8,n,1")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,q,1")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n,")]
        [TestCase(Factory.DltSerialFactory, "ser:0,8,n,1")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n,0")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n,3")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,4,n,1")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,9,n,1")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n,1#fragment")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n,1&x")]
        [TestCase(Factory.DltSerialFactory, "ser:115200,8,n,1/?query")]
        [TestCase(Factory.InputStreamFactory, "ser:")]
        [TestCase(Factory.InputStreamFactory, "ser://com1,115200,8,n,1")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,q,1")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n,")]
        [TestCase(Factory.InputStreamFactory, "ser:0,8,n,1")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n,0")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n,3")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,4,n,1")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,9,n,1")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n,1#fragment")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n,1&x")]
        [TestCase(Factory.InputStreamFactory, "ser:115200,8,n,1/?query")]
        public void SerInvalid(Factory factoryType, string uri)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                _ = factory.Create(uri);
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltSerialFactory)]
        public void ParseSerialUnix(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("ser:/dev/ttyUSB0,115200,8,n,1")) {
                Assert.That(stream.InputStream, Is.Null);
                Assert.That(stream.InputPacket, Is.Null);
                Assert.That(stream.IsLiveStream, Is.True);
                Assert.That(stream.RequiresConnection, Is.False);
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Serial));
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltSerialFactory)]
        public void ParseSerialWindows(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("ser:com1,115200,8,n,1")) {
                Assert.That(stream.InputStream, Is.Null);
                Assert.That(stream.InputPacket, Is.Null);
                Assert.That(stream.IsLiveStream, Is.True);
                Assert.That(stream.RequiresConnection, Is.False);
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Serial));
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltSerialFactory)]
        public void ParseSerialUnixHandshake(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("ser:/dev/ttyUSB0,115200,8,n,1,rts")) {
                Assert.That(stream.InputStream, Is.Null);
                Assert.That(stream.InputPacket, Is.Null);
                Assert.That(stream.IsLiveStream, Is.True);
                Assert.That(stream.RequiresConnection, Is.False);
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Serial));
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltSerialFactory)]
        public void ParseSerialWindowsHandshake(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create("ser:com1,115200,8,n,1,rts")) {
                Assert.That(stream.InputStream, Is.Null);
                Assert.That(stream.InputPacket, Is.Null);
                Assert.That(stream.IsLiveStream, Is.True);
                Assert.That(stream.RequiresConnection, Is.False);
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.Serial));
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltSerialFactory)]
        public void OpenDisposedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create("ser:com1,115200,8,n,1");
                input.Dispose();
                Assert.That(() => {
                    input.Open();
                }, Throws.TypeOf<ObjectDisposedException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltSerialFactory)]
        public void ConnectUnopenedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create("ser:com1,115200,8,n,1");
                Assert.That(async () => {
                    _ = await input.ConnectAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }
    }
}
