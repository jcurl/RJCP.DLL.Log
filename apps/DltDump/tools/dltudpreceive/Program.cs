namespace RJCP.App.DltUdpReceive
{
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;

    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 1) {
                Console.WriteLine("Usage: dltudpreceive [<localipaddress>]");
                return 1;
            }

            IPAddress localAddr = IPAddress.Any;
            if (args.Length == 1) {
                if (!IPAddress.TryParse(args[0], out localAddr)) {
                    Console.WriteLine("Usage: dltudpreceive <localipaddress>");
                    Console.WriteLine("  Couldn't parse {0}", args[0]);
                    return 1;
                }
            }

            if (!IPAddress.TryParse("239.255.42.99", out IPAddress mcastAddr)) {
                Console.WriteLine("Usage: dltudpreceive <localipaddress>");
                Console.WriteLine("  Internal error, couldn't parse 239.255.42.99");
                return 1;
            }

            IPEndPoint localEndPoint = new IPEndPoint(localAddr, 3490);
            MulticastOption mcastOption = new MulticastOption(mcastAddr, localAddr);
            Socket mcastSocket = null;

            try {
                mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                mcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                mcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcastOption);
                mcastSocket.Bind(localEndPoint);

                // Join to the multicast group

                int count = 0;
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = new byte[65536];
                StringBuilder sb = new StringBuilder();
                while (count < 100) {
                    int length = mcastSocket.ReceiveFrom(data, ref remoteEP);
                    if (length > 0) {
                        Console.WriteLine("{0:10}: Packet Received from {1}:{2}",
                            Environment.TickCount,
                            ((IPEndPoint)remoteEP).Address.ToString(),
                            ((IPEndPoint)remoteEP).Port);

                        HexConvert.ConvertToHex(sb, data.AsSpan(0, length));
                        Console.WriteLine("          -> {0}\n", sb.ToString());
                        count++;

                        sb.Clear();
                    }
                }

                // Remove the membership
                mcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, mcastOption);
            } catch (Exception ex) {
                Console.WriteLine("Error {0}", ex.ToString());
                return 1;
            } finally {
                if (mcastSocket != null) mcastSocket.Close();
            }

            return 0;
        }
    }
}
