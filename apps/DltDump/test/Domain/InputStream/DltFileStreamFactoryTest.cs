namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
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
                _ = factory.Create(string.Empty);
            }, Throws.TypeOf<InputStreamException>());
        }

        private readonly string EmptyFile = Path.Combine(Deploy.TestDirectory, "TestResources", "Input", "EmptyFile.dlt");

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenEmptyAbsoluteDltFile(Factory factoryType)
        {
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create(EmptyFile)) {
                Assert.That(stream, Is.TypeOf<DltFileStream>());
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
                    Assert.That(stream, Is.TypeOf<DltFileStream>());
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
                    Assert.That(stream, Is.TypeOf<DltFileStream>());
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
                    Assert.That(stream, Is.TypeOf<DltFileStream>());
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
            Assert.That(() => {
                _ = factory.Create("./nonexistent.dlt");
            }, Throws.TypeOf<InputStreamException>());
        }

        [TestCase(Factory.InputStreamFactory)]
        [TestCase(Factory.DltFileFactory)]
        public void OpenDltFileUri(Factory factoryType)
        {
            // Need to also use the InputStreamFactory that it properly converts the URI to a file://
            IInputStreamFactory factory = GetFactory(factoryType);
            using (IInputStream stream = factory.Create(new Uri(EmptyFile))) {
                Assert.That(stream, Is.TypeOf<DltFileStream>());
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
    }
}
