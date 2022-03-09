namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class TcpClientStreamTest
    {
        [Test]
        public void DefaultState()
        {
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(stream.CanRead, Is.False);
                Assert.That(stream.CanWrite, Is.False);
                Assert.That(stream.CanTimeout, Is.False);  // We're not connected, so we don't know.
                Assert.That(stream.ReadTimeout, Is.EqualTo(Timeout.Infinite));
                Assert.That(stream.WriteTimeout, Is.EqualTo(Timeout.Infinite));
                Assert.That(stream.IsConnected, Is.False);
            }
        }

        [Test]
        public void NullHostName()
        {
            Assert.That(() => {
                _ = new TcpClientStream(null, 3490);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyHostName()
        {
            Assert.That(() => {
                _ = new TcpClientStream("", 3490);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void BlankHostName()
        {
            Assert.That(() => {
                _ = new TcpClientStream("  ", 3490);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NegativePort()
        {
            Assert.That(() => {
                _ = new TcpClientStream("localhost", -1);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void VeryHighPort()
        {
            Assert.That(() => {
                _ = new TcpClientStream("localhost", 70000);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void NegativeReadTimeout()
        {
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(() => {
                    stream.ReadTimeout = -2;
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void NegativeWriteTimeout()
        {
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(() => {
                    stream.WriteTimeout = -2;
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void NegativeDisconnectTimeout()
        {
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(() => {
                    stream.DisconnectTimeout = -2;
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public void ConnectNoListener()
        {
            // It throws an instance of SocketException, but not the exact type on .NET Core. The time it takes to
            // execute this test is dependent on the Operating System.
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(stream.Connect(), Is.False);
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public async Task ConnectNoListenerAsync()
        {
            // It throws an instance of SocketException, but not the exact type on .NET Core. The time it takes to
            // execute this test is dependent on the Operating System.
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(await stream.ConnectAsync(), Is.False);
            }
        }

        private static bool ConnectedState(TcpClientStream stream)
        {
            Assert.That(stream.CanRead, Is.True);
            Assert.That(stream.CanWrite, Is.True);
            Assert.That(stream.CanTimeout, Is.True);
            Assert.That(stream.ReadTimeout, Is.EqualTo(Timeout.Infinite));
            Assert.That(stream.WriteTimeout, Is.EqualTo(Timeout.Infinite));
            Assert.That(stream.IsConnected, Is.True);
            return true;
        }

        [Test]
        [Repeat(10)]
        [Explicit("Integration Test")]
        public void Connect()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                    Assert.That(stream.Connect(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);
                }
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public void ConnectTwice()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                    Assert.That(stream.Connect(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);

                    Assert.That(() => {
                        _ = stream.Connect();
                    }, Throws.TypeOf<InvalidOperationException>());
                }
            }
        }

        [Test]
        [Repeat(10)]
        [Explicit("Integration Test")]
        public async Task ConnectAsync()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                    Assert.That(await stream.ConnectAsync(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);
                }
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public async Task ConnectAsyncTwice()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                    Assert.That(await stream.ConnectAsync(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);

                    Assert.That(async () => {
                        _ = await stream.ConnectAsync();
                    }, Throws.TypeOf<InvalidOperationException>());
                }
            }
        }

        private static bool SeekUnsupported(TcpClientStream stream)
        {
            Assert.That(stream.CanSeek, Is.False);
            Assert.That(() => {
                _ = stream.Length;
            }, Throws.TypeOf<NotSupportedException>());
            Assert.That(() => {
                _ = stream.Position;
            }, Throws.TypeOf<NotSupportedException>());
            Assert.That(() => {
                stream.Position = 0;
            }, Throws.TypeOf<NotSupportedException>());
            Assert.That(() => {
                _ = stream.Seek(0, System.IO.SeekOrigin.Begin);
            }, Throws.TypeOf<NotSupportedException>());
            Assert.That(() => {
                stream.SetLength(100);
            }, Throws.TypeOf<NotSupportedException>());
            return true;
        }

        [Test]
        [Explicit("Integration Test")]
        public void ConnectSeekUnsupported()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                    Assert.That(stream.Connect(), Is.True);
                    Assert.That(SeekUnsupported(stream), Is.True);
                }
            }
        }

        [Test]
        public void SeekUnsupported()
        {
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490)) {
                Assert.That(SeekUnsupported(stream), Is.True);
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public void ConnectTimeout()
        {
            // Note, changing this to a valid IP address on your network will work faster.
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490) {
                DisconnectTimeout = 200
            }) {
                Assert.That(stream.Connect(), Is.False);
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public async Task ConnectAsyncTimeout()
        {
            // Note, changing this to a valid IP address on your network will work faster.
            using (TcpClientStream stream = new TcpClientStream("localhost", 3490) {
                DisconnectTimeout = 200
            }) {
                Assert.That(await stream.ConnectAsync(), Is.False);
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public void ConnectReadTimeout()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490) {
                    DisconnectTimeout = 200
                }) {
                    Assert.That(stream.Connect(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);
                    Assert.That(stream.Read(new byte[100].AsSpan()), Is.EqualTo(0));
                }
            }
        }

        [Test]
        [Explicit("Integration Test")]
        public async Task ConnectReadAsyncTimeout()
        {
            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490) {
                    DisconnectTimeout = 200
                }) {
                    Assert.That(await stream.ConnectAsync(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);
                    Assert.That(await stream.ReadAsync(new byte[100].AsMemory()), Is.EqualTo(0));
                }
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Explicit("Integration Test")]
        public async Task ConnectReadTimeoutReset(bool close)
        {
            // A rather large test that shows we don't timeout in 200ms if data arrives.

            using (TcpServer server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                server.ClientConnected += (s, e) => {
                    Task.Factory.StartNew(() => {
                        NetworkStream serverStream = e.ConnectedClient.GetStream();
                        int startTime = Environment.TickCount;
                        int duration = 0;
                        byte value = 0;
                        do {
                            serverStream.WriteByte(value);
                            value++;
                            Thread.Sleep(50);
                            duration = unchecked(Environment.TickCount - startTime);
                        } while (duration < 900);

                        // 900ms have passed, in which case we close.
                        if (close) {
                            serverStream.Dispose();
                            e.ConnectedClient.Dispose();
                        } else {
                            Thread.Sleep(2000);
                        }
                    });
                };

                // Start the server listener in the background via a task. It will stop when the server is disposed.
                _ = server.ListenAsync();

                using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490) {
                    DisconnectTimeout = 200
                }) {
                    Assert.That(await stream.ConnectAsync(), Is.True);
                    Assert.That(ConnectedState(stream), Is.True);

                    byte[] buffer = new byte[1024];
                    int startTime = Environment.TickCount;
                    int duration;
                    int read;
                    do {
                        read = await stream.ReadAsync(buffer.AsMemory());
                        duration = unchecked(Environment.TickCount - startTime);
                        Console.WriteLine("{0}: Read={1}", duration, read);
                    } while (read > 0 && duration < 2000);

                    Assert.That(duration, Is.GreaterThan(900).And.LessThan(1500));

                    // One would expect this be always false, but when the remote socket closes, .NET TcpClient will
                    // still say it's connected for the first ~500ms until even after Read() has returned zero (0).
                    if (!close) Assert.That(stream.IsConnected, Is.False);
                }
            }
        }

        [Test]
        public void ReadNotConnected()
        {
            using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                Assert.That(() => {
                    _ = stream.Read(new byte[100].AsSpan());
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadDisposed()
        {
            TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490);
            stream.Dispose();

            Assert.That(() => {
                _ = stream.Read(new byte[100].AsSpan());
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void ReadAsyncNotConnected()
        {
            using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                Assert.That(async () => {
                    _ = await stream.ReadAsync(new byte[100].AsMemory());
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadAsyncDisposed()
        {
            TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490);
            stream.Dispose();

            Assert.That(async () => {
                _ = await stream.ReadAsync(new byte[100].AsMemory());
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "Test Case")]
        public void ReadAsyncBufferNotConnected()
        {
            using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                Assert.That(async () => {
                    _ = await stream.ReadAsync(new byte[100], 0, 100);
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "Test Case")]
        public void ReadAsyncBufferDisposed()
        {
            TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490);
            stream.Dispose();

            Assert.That(async () => {
                _ = await stream.ReadAsync(new byte[100], 0, 100);
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void WriteNotConnected()
        {
            using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                Assert.That(() => {
                    stream.Write(new byte[100].AsSpan());
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void WriteDisposed()
        {
            TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490);
            stream.Dispose();

            Assert.That(() => {
                stream.Write(new byte[100].AsSpan());
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void WriteAsyncNotConnected()
        {
            using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                Assert.That(async () => {
                    await stream.WriteAsync(new byte[100].AsMemory());
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void WriteAsyncDisposed()
        {
            TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490);
            stream.Dispose();

            Assert.That(async () => {
                await stream.WriteAsync(new byte[100].AsMemory());
            }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "Test Case")]
        public void WriteAsyncBufferNotConnected()
        {
            using (TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490)) {
                Assert.That(async () => {
                    await stream.WriteAsync(new byte[100], 0, 100);
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "Test Case")]
        public void WriteAsyncBufferDisposed()
        {
            TcpClientStream stream = new TcpClientStream("127.0.0.1", 3490);
            stream.Dispose();

            Assert.That(async () => {
                await stream.WriteAsync(new byte[100], 0, 100);
            }, Throws.TypeOf<ObjectDisposedException>());
        }
    }
}
