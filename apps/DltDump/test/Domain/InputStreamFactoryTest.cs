namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.IO;
    using Infrastructure.Dlt;
    using InputStream;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class InputStreamFactoryTest
    {
        [Test]
        public void NullString()
        {
            IInputStreamFactory factory = new InputStreamFactory();
            Assert.That(() => {
                factory.Create((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullUri()
        {
            IInputStreamFactory factory = new InputStreamFactory();
            Assert.That(() => {
                factory.Create((Uri)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InstantiateNullInput()
        {
            IInputStreamFactory factory = new TestInputStreamFactory();
            using (IInputStream input = factory.Create("null:")) {
                Assert.That(input, Is.TypeOf<NullInputStream>());
            }
        }

        [Test]
        public void UnknownScheme()
        {
            IInputStreamFactory factory = new InputStreamFactory();
            using (IInputStream input = factory.Create("unknown:")) {
                Assert.That(input, Is.Null);
            }
        }

        private readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        [Test]
        public void InstantiateFileInput()
        {
            IInputStreamFactory factory = new InputStreamFactory();
            using (IInputStream input = factory.Create(EmptyFile)) {
                Assert.That(input, Is.TypeOf<DltFileStream>());
                Assert.That(input.IsLiveStream, Is.False);
                Assert.That(input.SuggestedFormat, Is.EqualTo(InputFormat.File));
            }
        }

        [Test]
        public void SetFactoryScheme()
        {
            InputStreamFactory factory = new InputStreamFactory();
            Assert.That(() => {
                factory.SetFactory(null, new NullInputStreamFactory());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void UnregisterFactoryScheme()
        {
            InputStreamFactory factory = new InputStreamFactory();
            factory.SetFactory("file", null);
            using (IInputStream stream = factory.Create(EmptyFile)) {
                Assert.That(stream, Is.Null);
            }
        }

        [Test]
        public void RegisterFactoryScheme()
        {
            InputStreamFactory factory = new InputStreamFactory();
            factory.SetFactory("null", new NullInputStreamFactory());
            using (IInputStream stream = factory.Create("null:")) {
                Assert.That(stream, Is.TypeOf<NullInputStream>());
            }
        }

        [Test]
        public void InvalidStreamFactory()
        {
            TestInputStreamFactory factory = new TestInputStreamFactory();
            factory.SetFactory("invalid", new InvalidStreamFactory());
            using (IInputStream stream = factory.Create("invalid://")) {
                Assert.That(stream, Is.Null);
            }
        }
    }
}
