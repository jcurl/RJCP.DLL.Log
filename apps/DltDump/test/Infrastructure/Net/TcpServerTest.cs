namespace RJCP.App.DltDump.Infrastructure.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    // To test integration tests under Linux:
    //
    // dotnet test --filter=FullyQualifiedName~RJCP.App.DltDump.Infrastructure.Net.TcpServerTest
    // dotnet test --filter="FullyQualifiedName~RJCP.App.DltDump.Infrastructure.Net.TcpServerTest&TestCategory=Integration"

    [TestFixture]
    public class TcpServerTest
    {
        [Test]
        public void NoListener()
        {
            using (new TcpServer(IPAddress.Parse("127.0.0.1"), 3490)) {
                /* Do nothing, just let it dispose */
            }
        }

        [Test]
        [Repeat(50)]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public void ListenerDoNothing()
        {
            using (TcpServer server = new(IPAddress.Parse("127.0.0.1"), 3490)) {
                _ = server.ListenAsync();
                Thread.Sleep(10);
            }
        }

        [Test]
        [Repeat(10)]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public async Task ListenerDoNothingDispose()
        {
            TcpServer server = null;
            try {
                server = new TcpServer(IPAddress.Parse("127.0.0.1"), 3490);
                Task listen = server.ListenAsync();
                Thread.Sleep(100);
                server.Dispose();   // Disposing should stop the listener.
                server = null;

                await listen;
            } finally {
                if (server is not null) server.Dispose();
            }
        }

        [Test]
        [Repeat(3)]
        [Explicit("Integration Test")]
        [Category("Integration")]
        public async Task ListenConnect()
        {
            using (TcpServer server = new(IPAddress.Parse("127.0.0.1"), 3490)) {
                server.ClientConnected += (s, e) => {
                    Console.WriteLine("Client connected");
                    Task.Factory.StartNew(() => {
                        Thread.Sleep(50);
                        Console.WriteLine("Client closing");
                        e.ConnectedClient.Dispose();
                        server.Dispose();
                        Console.WriteLine("Client closed");
                    });
                    Console.WriteLine("Client connected callback finished");
                };

                // 1. The server is started.
                // 2. As soon as possible, the client starts and connects
                // 3. There is a connection, it spawns and then waits 50ms and closes.
                //
                // Test should take about 100ms

                Console.WriteLine("Listener started in the background");
                Task listen = server.ListenAsync();

                using (TcpClient tcpClient = new()) {
                    await tcpClient.ConnectAsync("127.0.0.1", 3490);

                    NetworkStream stream = tcpClient.GetStream();
                    int readBytes = await stream.ReadAsync(new byte[1].AsMemory());
                    Console.WriteLine($"ReadBytes = {readBytes}");
                }

                await listen;
                Console.WriteLine("Listener stopped");
            }
        }
    }
}
