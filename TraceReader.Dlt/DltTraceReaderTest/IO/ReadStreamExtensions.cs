namespace RJCP.Diagnostics.Log.IO
{
    using System;
    using System.IO;

    internal static class ReadStreamExtensions
    {
        public static byte[] ReadStream(this Stream stream)
        {
            byte[] buffer = new byte[65535];
            Span<byte> readBuff = buffer.AsSpan();

            stream.Seek(0, SeekOrigin.Begin);

            int len = 0;
            while (true) {
                int read = stream.Read(readBuff);
                if (read == 0)
                    return buffer[0..len];
                len += read;
            }
        }
    }
}
