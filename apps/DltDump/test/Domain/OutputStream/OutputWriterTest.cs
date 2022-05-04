namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class OutputWriterTest
    {
        [Test]
        public void StreamDefaults()
        {
            using (OutputWriter writer = new OutputWriter()) {
                Assert.That(writer.Length, Is.EqualTo(0));
                Assert.That(writer.IsOpen, Is.False);
            }
        }

        [Test]
        public void WriterNotOpenWrite()
        {
            using (OutputWriter writer = new OutputWriter()) {
                byte[] buffer = new byte[10];

                Assert.That(writer.IsOpen, Is.False);
                Assert.That(() => {
                    writer.Write(buffer.AsSpan());
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void WriterNotOpenWriteBuffer()
        {
            using (OutputWriter writer = new OutputWriter()) {
                byte[] buffer = new byte[10];

                Assert.That(writer.IsOpen, Is.False);
                Assert.That(() => {
                    writer.Write(buffer, 0, buffer.Length);
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }
        [Test]
        public void WriterNotOpenFlush()
        {
            using (OutputWriter writer = new OutputWriter()) {
                Assert.That(writer.IsOpen, Is.False);
                Assert.That(() => {
                    writer.Flush();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void WriterNotOpenClose()
        {
            using (OutputWriter writer = new OutputWriter()) {
                Assert.That(writer.IsOpen, Is.False);
                writer.Close();
            }
        }

        [Test]
        public void WriterOpenNullName()
        {
            using (OutputWriter writer = new OutputWriter()) {
                Assert.That(() => {
                    writer.Open(null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void WriterOpen()
        {
            using (ScratchPad pad = Deploy.ScratchPad()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    Assert.That(writer.IsOpen, Is.False);
                    writer.Open("File.txt");

                    Assert.That(writer.IsOpen, Is.True);
                    Assert.That(File.Exists("File.txt"), Is.True);
                    Assert.That(writer.Length, Is.EqualTo(0));
                }

                Assert.That(writer.IsOpen, Is.False);
            }
        }

        [Test]
        public void WriterOpenClose()
        {
            byte[] buffer = new byte[10];
            using (ScratchPad pad = Deploy.ScratchPad()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    writer.Open("File.txt");
                    writer.Write(buffer.AsSpan());

                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));
                    Assert.That(writer.IsOpen, Is.True);

                    writer.Close();
                    Assert.That(writer.IsOpen, Is.False);
                }

                Assert.That(writer.IsOpen, Is.False);

                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriterOpenTwice()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    writer.Open("File.txt");

                    Assert.That(() => {
                        writer.Open("File2.txt");
                    }, Throws.TypeOf<InvalidOperationException>());

                    Assert.That(File.Exists("File.txt"), Is.True);
                    Assert.That(File.Exists("File2.txt"), Is.False);

                    // Test that the original file is still open.
                    Assert.That(writer.Length, Is.EqualTo(0));
                    writer.Write(buffer.AsSpan());
                    writer.Flush();
                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));

                    // Must call writer.Flush() before this call, that it is written to the file system.
                    FileInfo fileInfo = new FileInfo("File.txt");
                    Assert.That(fileInfo.Length, Is.EqualTo(buffer.Length));
                }

                Assert.That(writer.IsOpen, Is.False);
                Assert.That(writer.Length, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriterOpenDisposeTwice()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    writer.Open("File.txt");
                    writer.Write(buffer.AsSpan());
                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));
                }

                writer.Dispose();
            }
        }

        [Test]
        public void WriterOpenDisposeClose()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    writer.Open("File.txt");
                    writer.Write(buffer.AsSpan());
                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));
                }

                writer.Close();
            }
        }

        [Test]
        public void WriterOpenCloseOpen()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Open("File.txt");
                writer.Write(buffer.AsSpan());
                Assert.That(writer.Length, Is.EqualTo(buffer.Length));
                writer.Close();
                Assert.That(writer.IsOpen, Is.False);

                writer.Open("File2.txt");

                // On the second open, the length must be reset to zero.
                Assert.That(writer.Length, Is.EqualTo(0));

                writer.Write(buffer.AsSpan());
                writer.Write(buffer.AsSpan());
                Assert.That(writer.Length, Is.EqualTo(2 * buffer.Length));

                writer.Close();
                Assert.That(writer.Length, Is.EqualTo(2 * buffer.Length));
                Assert.That(writer.IsOpen, Is.False);

                // Close twice, second should be ignored.
                writer.Close();
                Assert.That(writer.Length, Is.EqualTo(2 * buffer.Length));
                Assert.That(writer.IsOpen, Is.False);
            }
        }

        [Test]
        public void OverWriteFileWithoutForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Open("File.txt");
                Assert.That(writer.IsOpen, Is.True);
                writer.Close();
                Assert.That(writer.IsOpen, Is.False);

                Assert.That(() => {
                    writer.Open("File.txt");
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<IOException>());
                Assert.That(writer.IsOpen, Is.False);
            }
        }

        [Test]
        public void OverWriteFileWithForce()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Open("File.txt");
                Assert.That(writer.IsOpen, Is.True);
                writer.Write(buffer.AsSpan().Slice(0, 5));
                Assert.That(writer.Length, Is.EqualTo(5));
                writer.Close();
                Assert.That(writer.IsOpen, Is.False);

                writer.Open("File.txt", FileMode.Create);
                Assert.That(writer.IsOpen, Is.True);
                writer.Write(buffer.AsSpan());
                Assert.That(writer.Length, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void OverWriteFileWithAppend()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad()) {
                using (OutputWriter writer = new OutputWriter()) {
                    writer.Open("File.txt");
                    Assert.That(writer.IsOpen, Is.True);
                    writer.Write(buffer.AsSpan().Slice(0, 5));
                    Assert.That(writer.Length, Is.EqualTo(5));
                    writer.Close();
                    Assert.That(writer.IsOpen, Is.False);

                    writer.Open("File.txt", FileMode.Append);
                    Assert.That(writer.IsOpen, Is.True);
                    writer.Write(buffer.AsSpan());
                    Assert.That(writer.Length, Is.EqualTo(5 + buffer.Length));
                }

                FileInfo fileInfo = new FileInfo("File.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(5 + buffer.Length));
            }
        }

        [Test]
        public void WriterOpenWhenDisposed()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Dispose();
                Assert.That(() => {
                    writer.Open("File.txt");
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriterFlushWhenDisposed()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Dispose();
                Assert.That(() => {
                    writer.Flush();
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriterOpenFlushWhenDisposed()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Open("file.txt");
                writer.Dispose();
                Assert.That(() => {
                    writer.Flush();
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriterWriteWhenDisposed()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Dispose();
                Assert.That(() => {
                    writer.Write(buffer.AsSpan());
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriterWriteBuffWhenDisposed()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Dispose();
                Assert.That(() => {
                    writer.Write(buffer, 0, buffer.Length);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriterOpenWriteWhenDisposed()
        {
            byte[] buffer = new byte[10];

            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new OutputWriter()) {
                writer.Open("file.txt");
                writer.Dispose();
                Assert.That(() => {
                    writer.Write(buffer.AsSpan());
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }
    }
}
