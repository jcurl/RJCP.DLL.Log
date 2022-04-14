namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using Domain.Dlt;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class DltFileStreamFactoryTest
    {
        // This class tests both the InputStreamFactory and the DltFileStreamFactory for creating a DltFileStream.
        // Either one needs to detect that the file is not a URI and convert it to the correct URI and create the
        // correct stream.

        public enum Factory
        {
            InputStreamFactory,
            DltFileFactory
        }

        private static IInputStreamFactory GetFactory(Factory factoryType)
        {
            switch (factoryType) {
            case Factory.InputStreamFactory:
                return new InputStreamFactory();
            case Factory.DltFileFactory:
                return new DltFileStreamFactory();
            default:
                throw new ArgumentException("Unknown factory");
            }
        }

        [Test]
        public void OpenNullFile()
        {
            IInputStreamFactory factory = new DltFileStreamFactory();
            Assert.That(() => {
                _ = factory.Create((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OpenNullUri()
        {
            IInputStreamFactory factory = new DltFileStreamFactory();
            Assert.That(() => {
                _ = factory.Create((Uri)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenEmptyDltFile(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            Assert.That(() => {
                IInputStream input = factory.Create(string.Empty);
                input.Open();
            }, Throws.TypeOf<InputStreamException>());
        }

        private readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenEmptyAbsoluteDltFile(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create(EmptyFile)) {
                stream.Open();
                Assert.That(stream, Is.TypeOf<DltFileStream>());
                Assert.That(stream.Scheme, Is.EqualTo("file"));
                Assert.That(stream.InputStream, Is.TypeOf<FileStream>());
                Assert.That(stream.IsLiveStream, Is.False);
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.File));
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenEmptyRelativeDltFileNoPath(Factory factoryType)
        {
            using (var scratch = Deploy.ScratchPad()) {
                scratch.DeployItem(EmptyFile);
                IInputStreamFactory factory = GetFactory(factoryType);
                using (IInputStream stream = factory.Create("EmptyFile.dlt")) {
                    stream.Open();
                    Assert.That(stream, Is.TypeOf<DltFileStream>());
                    Assert.That(stream.Scheme, Is.EqualTo("file"));
                    Assert.That(stream.InputStream, Is.TypeOf<FileStream>());
                    Assert.That(stream.IsLiveStream, Is.False);
                    Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.File));
                }
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenEmptyRelativeDltFileCurrentDir(Factory factoryType)
        {
            using (var scratch = Deploy.ScratchPad()) {
                scratch.DeployItem(EmptyFile);
                IInputStreamFactory factory = GetFactory(factoryType);
                using (IInputStream stream = factory.Create(Path.Combine(".", "EmptyFile.dlt"))) {
                    stream.Open();
                    Assert.That(stream, Is.TypeOf<DltFileStream>());
                    Assert.That(stream.Scheme, Is.EqualTo("file"));
                    Assert.That(stream.InputStream, Is.TypeOf<FileStream>());
                    Assert.That(stream.IsLiveStream, Is.False);
                    Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.File));
                }
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenEmptyRelativeDltFileDir(Factory factoryType)
        {
            using (var scratch = Deploy.ScratchPad(ScratchOptions.KeepCurrentDir)) {
                scratch.DeployItem(EmptyFile);
                IInputStreamFactory factory = GetFactory(factoryType);
                using (IInputStream stream = factory.Create(Path.Combine(scratch.RelativePath, "EmptyFile.dlt"))) {
                    stream.Open();
                    Assert.That(stream, Is.TypeOf<DltFileStream>());
                    Assert.That(stream.Scheme, Is.EqualTo("file"));
                    Assert.That(stream.InputStream, Is.TypeOf<FileStream>());
                    Assert.That(stream.IsLiveStream, Is.False);
                    Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.File));
                }
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenNonExistentDltFile(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream input = factory.Create("./nonexistent.dlt")) {
                Assert.That(() => {
                    input.Open();
                }, Throws.TypeOf<InputStreamException>());
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenDltFileUri(Factory factoryType)
        {
            // Need to also use the InputStreamFactory that it properly converts the URI to a file://
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create(new Uri(EmptyFile))) {
                stream.Open();
                Assert.That(stream, Is.TypeOf<DltFileStream>());
                Assert.That(stream.Scheme, Is.EqualTo("file"));
                Assert.That(stream.InputStream, Is.TypeOf<FileStream>());
                Assert.That(stream.IsLiveStream, Is.False);
                Assert.That(stream.SuggestedFormat, Is.EqualTo(InputFormat.File));
            }
        }

        [Test]
        public void OpenDltFileStreamNullFile()
        {
            Assert.That(() => {
                _ = new DltFileStream(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenDisposedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create(EmptyFile);
                input.Dispose();
                Assert.That(() => {
                    input.Open();
                }, Throws.TypeOf<ObjectDisposedException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void ConnectUnopenedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create(EmptyFile);
                Assert.That(async () => {
                    _ = await input.ConnectAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            } finally {
                if (input != null) input.Dispose();
            }
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void ConnectDisposedInputStream(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            IInputStream input = null;
            try {
                input = factory.Create(EmptyFile);
                input.Open();
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
