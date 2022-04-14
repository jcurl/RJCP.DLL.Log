namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class DetailedFileInfoTest
    {
        // A more detailed test would change these files to point to a different file system, e.g. FAT32, which behaves
        // differently to NTFS.
        private const string File1 = @"file.txt";
        private const string File2 = @"file2.txt";

        [Test]
        [SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "Explicit Comparison Check")]
        public void GetInfoSameFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                using (FileStream stream = new FileStream(File1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    /* Create empty file */
                }

                DetailedFileInfo info1 = new DetailedFileInfo(File1);
                DetailedFileInfo info2 = new DetailedFileInfo(Path.Combine(pad.Path, File1));

                Assert.That(info1, Is.EqualTo(info2));
                Assert.That(info1 == info2, Is.True);
                Assert.That(info1 != info2, Is.False);
                Assert.That(info1.Equals(info2), Is.True);
                Assert.That(((object)info1).Equals(info2), Is.True);
                Assert.That(info1.GetHashCode(), Is.EqualTo(info2.GetHashCode()));
            }
        }

        [Test]
        [SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "Explicit Comparison Check")]
        public void GetInfoDifferentFile()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                using (FileStream stream = new FileStream(File1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    /* Create empty file */
                }

                using (FileStream stream = new FileStream(File2, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    /* Create empty file */
                }

                DetailedFileInfo info1 = new DetailedFileInfo(File1);
                DetailedFileInfo info2 = new DetailedFileInfo(File2);

                Assert.That(info1, Is.Not.EqualTo(info2));
                Assert.That(info1 == info2, Is.False);
                Assert.That(info1 != info2, Is.True);
                Assert.That(info1.Equals(info2), Is.False);
                Assert.That(((object)info1).Equals(info2), Is.False);
            }
        }

        [Test]
        [SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "Explicit Comparison Check")]
        public void EqualsNull()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                using (FileStream stream = new FileStream(File1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    /* Create empty file */
                }

                DetailedFileInfo info1 = new DetailedFileInfo(File1);
                DetailedFileInfo info2 = null;

                Assert.That(info1, Is.Not.EqualTo(info2));
                Assert.That(info2, Is.Not.EqualTo(info1));
                Assert.That(info1 == info2, Is.False);
                Assert.That(info2 == info1, Is.False);
                Assert.That(info1 != info2, Is.True);
                Assert.That(info2 != info1, Is.True);
                Assert.That(info1.Equals(info2), Is.False);
                Assert.That(((object)info1).Equals(info2), Is.False);

                DetailedFileInfo info3 = null;
                Assert.That(info2, Is.EqualTo(info3));
                Assert.That(info3, Is.EqualTo(info2));
                Assert.That(info2 == info3, Is.True);
                Assert.That(info3 == info2, Is.True);
                Assert.That(info2 != info3, Is.False);
                Assert.That(info3 != info2, Is.False);
            }
        }

        [Test]
        public void NullInfo()
        {
            Assert.That(() => {
                _ = new DetailedFileInfo(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyInfo()
        {
            Assert.That(() => {
                _ = new DetailedFileInfo(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NoFile()
        {
            Assert.That(() => {
                _ = new DetailedFileInfo("nonexistent.txt");
            }, Throws.TypeOf<FileNotFoundException>());
        }
    }
}
