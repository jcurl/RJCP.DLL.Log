namespace RJCP.Diagnostics.Log.Dlt
{
    using System.IO;
    using System.Threading.Tasks;
    using NonVerbose;
    using NUnit.Framework;
    using Packet;
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
            : this(decoderType, endian, null) { }

        protected DecoderTestBase(DecoderType decoderType, Endianness endian, IFrameMap map)
        {
            Type = decoderType;
            Endian = endian;
            Map = map;
        }

        protected DecoderType Type { get; }

        protected Endianness Endian { get; }

        protected IFrameMap Map { get; }

        protected DltTraceLineBase DecodeLine(DltFactory factory, DltType dltType, byte[] data, string fileName)
        {
            factory ??= new DltFactory(DltFactoryType.File, Map);

            using (DltPacketWriter writer = new DltPacketWriter() {
                EcuId = "ECU1", AppId = "APP1", CtxId = "CTX1", Counter = 127, SessionId = 50
            }) {
                bool isBig = Endian == Endianness.Big;
                CreateLine(factory, writer, dltType, data, isBig);
                using (Stream stream = writer.Stream()) {
                    if (!string.IsNullOrEmpty(fileName)) {
                        string argType = "args";
                        switch (dltType) {
                        case DltType.CONTROL_REQUEST:
                        case DltType.CONTROL_RESPONSE:
                        case DltType.CONTROL_TIME:
                            argType = "control";
                            break;
                        }
                        string dir = Path.Combine(Deploy.WorkDirectory, "dltout", "read", argType, isBig ? "big" : "little");
                        string outPath = Path.Combine(dir, $"{fileName}.dlt");
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
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
            using (ITraceReader<DltTraceLineBase> reader = await factory.DltReaderFactory(stream)) {
                DltTraceLineBase line = await reader.GetLineAsync();

                // This is the only way to check the length, that this was the only packet decoded.
                DltTraceLineBase lastLine = await reader.GetLineAsync();
                Assert.That(lastLine, Is.Null);
                return line;
            }
        }
    }
}
