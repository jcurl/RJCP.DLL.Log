namespace RJCP.Diagnostics.Log.Dlt
{
    using System.IO;
    using System.Threading.Tasks;
    using Dlt.Packet;
    using NUnit.Framework;
    using RJCP.CodeQuality.NUnitExtensions;

    public abstract class DecoderTestBase
    {
        public enum DecoderType
        {
            Line,
            Packet,
            Specialized,
        }

        public enum Endianness
        {
            Little = 1,
            Big = 2
        }

        protected DecoderTestBase(DecoderType decoderType, Endianness endian)
        {
            Type = decoderType;
            Endian = endian;
        }

        protected DecoderType Type { get; }

        protected Endianness Endian { get; }

        protected DltTraceLineBase DecodeLine(DltFactory factory, DltType dltType, byte[] data, string fileName)
        {
            if (factory == null) factory = new DltFactory(DltFactoryType.File);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                bool isBig = Endian == Endianness.Big;
                CreateLine(factory, writer, dltType, data, isBig);
                using (Stream stream = writer.Stream()) {
                    if (!string.IsNullOrEmpty(fileName)) {
                        string dir = Path.Combine(Deploy.WorkDirectory, "dltout", "control", isBig ? "big" : "little");
                        string outPath = Path.Combine(dir, $"{fileName}.dlt");
                        if (!Directory.Exists(dir)) {
                            Directory.CreateDirectory(dir);
                        }
                        writer.Write(outPath);
                    }

                    Task<DltTraceLineBase> lineTask = WriteDltPacket(factory, stream);
                    return lineTask.GetAwaiter().GetResult();
                }
            }
        }

        protected abstract void CreateLine(DltFactory factory, DltPacketWriter writer, DltType dltType, byte[] data, bool msbf);

        private static async Task<DltTraceLineBase> WriteDltPacket(DltFactory factory, Stream stream)
        {
            DltTraceLineBase line;
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                line = await reader.GetLineAsync();

                // This is the only way to check the length, that this was the only packet decoded.
                DltTraceLineBase lastLine = await reader.GetLineAsync();
                Assert.That(lastLine, Is.Null);
                return line;
            }
        }
    }
}
