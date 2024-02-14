namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.IO;
    using System.Threading;
    using Moq;
    using NUnit.Framework;
    using RJCP.CodeQuality.IO;
    using RJCP.CodeQuality.NUnitExtensions;

    [TestFixture]
    public class OutputWriterTest
    {
        [Test]
        public void StreamDefaults()
        {
            using (OutputWriter writer = new()) {
                Assert.That(writer.Length, Is.EqualTo(0));
                Assert.That(writer.IsOpen, Is.False);
            }
        }

        [Test]
        public void WriterNotOpenWrite()
        {
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
                Assert.That(writer.IsOpen, Is.False);
                Assert.That(() => {
                    writer.Flush();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void WriterNotOpenClose()
        {
            using (OutputWriter writer = new()) {
                Assert.That(writer.IsOpen, Is.False);
                writer.Close();
            }
        }

        [Test]
        public void WriterOpenNullName()
        {
            using (OutputWriter writer = new()) {
                Assert.That(() => {
                    writer.Open((string)null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void WriterOpenNullStream()
        {
            using (OutputWriter writer = new()) {
                Assert.That(() => {
                    writer.Open((Stream)null);
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
        public void WriteOpenStream()
        {
            using (MemoryStream stream = new()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    Assert.That(writer.IsOpen, Is.False);
                    writer.Open(stream);

                    Assert.That(writer.IsOpen, Is.True);
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

                FileInfo fileInfo = new("File.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriterOpenCloseStream()
        {
            byte[] buffer = new byte[10];
            using (MemoryStream stream = new()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    writer.Open(stream);
                    writer.Write(buffer.AsSpan());

                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));
                    Assert.That(writer.IsOpen, Is.True);

                    Assert.That(stream.Length, Is.EqualTo(buffer.Length));
                    writer.Close();
                    Assert.That(writer.IsOpen, Is.False);
                    Assert.That(stream.Length, Is.EqualTo(buffer.Length));
                }

                Assert.That(writer.IsOpen, Is.False);
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

                    Assert.That(() => {
                        writer.Open(new MemoryStream());
                    }, Throws.TypeOf<InvalidOperationException>());

                    Assert.That(File.Exists("File.txt"), Is.True);
                    Assert.That(File.Exists("File2.txt"), Is.False);

                    // Test that the original file is still open.
                    Assert.That(writer.Length, Is.EqualTo(0));
                    writer.Write(buffer.AsSpan());
                    writer.Flush();
                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));

                    // Must call writer.Flush() before this call, that it is written to the file system.
                    FileInfo fileInfo = new("File.txt");
                    Assert.That(fileInfo.Length, Is.EqualTo(buffer.Length));
                }

                Assert.That(writer.IsOpen, Is.False);
                Assert.That(writer.Length, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriterOpenStreamTwice()
        {
            byte[] buffer = new byte[10];

            using (MemoryStream stream = new()) {
                OutputWriter writer;
                using (writer = new OutputWriter()) {
                    writer.Open(stream);

                    Assert.That(() => {
                        writer.Open("File2.txt");
                    }, Throws.TypeOf<InvalidOperationException>());

                    Assert.That(() => {
                        writer.Open(new MemoryStream());
                    }, Throws.TypeOf<InvalidOperationException>());

                    // Test that the original file is still open.
                    Assert.That(writer.Length, Is.EqualTo(0));
                    writer.Write(buffer.AsSpan());
                    writer.Flush();
                    Assert.That(writer.Length, Is.EqualTo(buffer.Length));

                    // Must call writer.Flush() before this call, that it is written to the file system.
                    Assert.That(stream.Length, Is.EqualTo(buffer.Length));
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
            using (OutputWriter writer = new()) {
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
        public void WriterOpenStreamCloseOpen()
        {
            byte[] buffer = new byte[10];

            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                writer.Write(buffer.AsSpan());
                Assert.That(writer.Length, Is.EqualTo(buffer.Length));
                writer.Close();
                Assert.That(writer.IsOpen, Is.False);

                writer.Open(stream);

                // On the second open, the length must be reset to zero.
                Assert.That(writer.Length, Is.EqualTo(buffer.Length));

                writer.Write(buffer.AsSpan());
                writer.Write(buffer.AsSpan());
                Assert.That(writer.Length, Is.EqualTo(3 * buffer.Length));

                writer.Close();
                Assert.That(writer.Length, Is.EqualTo(3 * buffer.Length));
                Assert.That(writer.IsOpen, Is.False);

                // Close twice, second should be ignored.
                writer.Close();
                Assert.That(writer.Length, Is.EqualTo(3 * buffer.Length));
                Assert.That(writer.IsOpen, Is.False);
            }
        }

        [Test]
        public void OverWriteFileWithoutForce()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
                writer.Open("File.txt");
                Assert.That(writer.IsOpen, Is.True);
                writer.Write(buffer.AsSpan()[..5]);
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
                using (OutputWriter writer = new()) {
                    writer.Open("File.txt");
                    Assert.That(writer.IsOpen, Is.True);
                    writer.Write(buffer.AsSpan()[..5]);
                    Assert.That(writer.Length, Is.EqualTo(5));
                    writer.Close();
                    Assert.That(writer.IsOpen, Is.False);

                    writer.Open("File.txt", FileMode.Append);
                    Assert.That(writer.IsOpen, Is.True);
                    writer.Write(buffer.AsSpan());
                    Assert.That(writer.Length, Is.EqualTo(5 + buffer.Length));
                }

                FileInfo fileInfo = new("File.txt");
                Assert.That(fileInfo.Length, Is.EqualTo(5 + buffer.Length));
            }
        }

        [Test]
        public void WriterOpenWhenDisposed()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new()) {
                writer.Dispose();
                Assert.That(() => {
                    writer.Open("File.txt");
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteOpenStreamWhenDisposed()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Dispose();
                Assert.That(() => {
                    writer.Open(stream);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteOpenDisposedStream()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                stream.Dispose();
                Assert.That(() => {
                    // Stream is not writable, as it's disposed.
                    writer.Open(stream);
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void WriteOpenStreamErrorInPosition()
        {
            var streamMock = new Mock<Stream>();
            streamMock.Setup(stream => stream.CanWrite).Returns(true);
            streamMock.Setup(stream => stream.Position).Throws<PlatformNotSupportedException>();

            using (OutputWriter writer = new()) {
                Assert.That(() => {
                    // Stream is not writable, as it's disposed.
                    writer.Open(streamMock.Object);
                }, Throws.TypeOf<PlatformNotSupportedException>());
            }
        }

        [Test]
        public void WriterFlushWhenDisposed()
        {
            using (ScratchPad pad = Deploy.ScratchPad())
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
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
            using (OutputWriter writer = new()) {
                writer.Open("file.txt");
                writer.Dispose();
                Assert.That(() => {
                    writer.Write(buffer.AsSpan());
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void WriteBufferNull()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                Assert.That(() => {
                    writer.Write(null, 0, 1);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void WriteBufferNegativeOffset()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                Assert.That(() => {
                    writer.Write(new byte[10], -1, 1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void WriteBufferNegativeCount()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                Assert.That(() => {
                    writer.Write(new byte[10], 0, -1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void WriteBufferOutOfBoundsIndex()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                Assert.That(() => {
                    writer.Write(new byte[10], 11, 1);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void WriteBufferOutOfBoundsCount()
        {
            using (MemoryStream stream = new())
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                Assert.That(() => {
                    writer.Write(new byte[10], 5, 10);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void WriteBufferWithException()
        {
            var streamMock = new Mock<SimpleStream>(StreamMode.ReadWrite) {
                CallBase = true
            };
            streamMock
                .Setup(stream => stream.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PlatformNotSupportedException>();

            using (OutputWriter writer = new()) {
                writer.Open(streamMock.Object);
                Assert.That(() => {
                    writer.Write(new byte[10], 0, 10);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<PlatformNotSupportedException>());
                Assert.That(writer.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void WriteBufferWithException2()
        {
            var streamMock = new Mock<SimpleStream>(StreamMode.ReadWrite) {
                CallBase = true
            };

            // The first write works, the second fails.
            int count = 0;
            streamMock.Setup(stream => stream.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() => {
                    if (count == 1) throw new PlatformNotSupportedException();
                    count++;
                })
                .CallBase();

            using (OutputWriter writer = new()) {
                writer.Open(streamMock.Object);
                writer.Write(new byte[50], 0, 50);
                Assert.That(writer.Length, Is.EqualTo(50));

                // The second should fail.
                Assert.That(() => {
                    writer.Write(new byte[10], 0, 10);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<PlatformNotSupportedException>());
                Assert.That(writer.Length, Is.EqualTo(50));
            }
        }

        // We can't use Moq4 with ReadOnlySpan<> as a parameter, so must implement our own mock.
        private class NullStreamSpan : SimpleStream
        {
            private int m_Count;

            public NullStreamSpan(int count) : base(StreamMode.ReadWrite)
            {
                m_Count = count;
            }

            public override void Write(ReadOnlySpan<byte> buffer)
            {
                if (m_Count == 0) throw new PlatformNotSupportedException();
                --m_Count;
                base.Write(buffer);
            }
        }

        [Test]
        public void WriteSpanWithException()
        {
            using (Stream stream = new NullStreamSpan(0))
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                Assert.That(() => {
                    writer.Write(new byte[10]);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<PlatformNotSupportedException>());
                Assert.That(writer.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void WriteSpanWithException2()
        {
            using (Stream stream = new NullStreamSpan(1))
            using (OutputWriter writer = new()) {
                writer.Open(stream);
                writer.Write(new byte[50]);
                Assert.That(writer.Length, Is.EqualTo(50));

                // The second should fail.
                Assert.That(() => {
                    writer.Write(new byte[10]);
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<PlatformNotSupportedException>());
                Assert.That(writer.Length, Is.EqualTo(50));
            }
        }

        [Test]
        public void FlushWithException()
        {
            var streamMock = new Mock<SimpleStream>(StreamMode.ReadWrite) {
                CallBase = true
            };
            streamMock.Setup(stream => stream.Flush()).Throws<PlatformNotSupportedException>();

            using (OutputWriter writer = new()) {
                writer.Open(streamMock.Object);
                writer.Write(new byte[50], 0, 50);
                Assert.That(writer.Length, Is.EqualTo(50));

                // The second should fail.
                Assert.That(() => {
                    writer.Flush();
                }, Throws.TypeOf<OutputStreamException>().With.InnerException.TypeOf<PlatformNotSupportedException>());
                Assert.That(writer.Length, Is.EqualTo(50));
            }
        }

        [Test]
        public void AutoFlush()
        {
            using ManualResetEventSlim ev = new();
            var streamMock = new Mock<SimpleStream>(StreamMode.ReadWrite) {
                CallBase = true
            };

            int flushCount = 0;
            streamMock.Setup(stream => stream.Flush()).Callback(() => {
                flushCount++;
                if (flushCount == 2) ev.Set();
            });

            using (OutputWriter writer = new()) {
                writer.Open(streamMock.Object);
                writer.AutoFlush(50);
                Assert.That(ev.Wait(500), Is.True);

                // Check that the task has stopped calling flush after close.
                writer.Close();
                int currentFlush = flushCount;

                // Make sure the thread is closed. We can't wait for something, as we don't have the internal state such
                // as the Task that does the flushing.
                Thread.Sleep(200);
                Assert.That(flushCount, Is.EqualTo(currentFlush));
            }
        }

        [Test]
        public void AutoFlushTwice()
        {
            using ManualResetEventSlim ev = new();
            var streamMock = new Mock<SimpleStream>(StreamMode.ReadWrite) {
                CallBase = true
            };

            int flushCount = 0;
            streamMock.Setup(stream => stream.Flush()).Callback(() => {
                flushCount++;
                ev.Set();
            });

            using (OutputWriter writer = new()) {
                writer.Open(streamMock.Object);

                // Only start autoflush with 5s
                writer.AutoFlush(5000);
                writer.AutoFlush(10);

                // Because we sleep only 500ms, no flush has occurred yet.
                Assert.That(ev.Wait(500), Is.False);
                Console.WriteLine("Flushes = {0}", flushCount);
            }
        }

        [Test]
        public void AutoFlushWhenNotOpen()
        {
            using (OutputWriter writer = new()) {
                Assert.That(() => {
                    writer.AutoFlush(10);
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void AutoFlushWithException()
        {
            using ManualResetEventSlim ev = new();
            var streamMock = new Mock<SimpleStream>(StreamMode.ReadWrite) {
                CallBase = true
            };

            int flushCount = 1;
            streamMock.Setup(stream => stream.Flush())
                .Callback(() => {
                    if (flushCount == 0) {
                        ev.Set();
                        throw new PlatformNotSupportedException();
                    }
                    --flushCount;
                })
                .CallBase();

            using (OutputWriter writer = new()) {
                writer.Open(streamMock.Object);
                writer.AutoFlush(50);
                ev.Wait(500);

                // Wait 50ms so that the background task can complete, in case it crashes the program.
                Thread.Sleep(50);

                // An exception occurs after to flush counts. Else it would go negative.
                Assert.That(flushCount, Is.EqualTo(0));
            }
        }
    }
}
