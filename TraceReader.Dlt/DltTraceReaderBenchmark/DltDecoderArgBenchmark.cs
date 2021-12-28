namespace RJCP.Diagnostics.Log
{
    using System.Text;
    using BenchmarkDotNet.Attributes;
    using Dlt.Verbose;

    public class DltDecoderArgBenchmark
    {
        public DltDecoderArgBenchmark()
        {
            // Required to decode ISO-8859-15 when encoded as ASCII.
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
        }

        private static readonly SignedIntArgDecoder SignedIntArgDecoder = new SignedIntArgDecoder();
        private static readonly byte[] SignedInt32 = new byte[] { 0x23, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };

        private static readonly UnsignedIntArgDecoder UnsignedIntArgDecoder = new UnsignedIntArgDecoder();
        private static readonly byte[] UnsignedInt32 = new byte[] { 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };
        private static readonly byte[] UnsignedInt32Hex = new byte[] { 0x43, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 };
        private static readonly byte[] UnsignedInt32Bin = new byte[] { 0x43, 0x80, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 };

        private static readonly FloatArgDecoder FloatArgDecoder = new FloatArgDecoder();
        private static readonly byte[] Float32LE = new byte[] { 0x83, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0xBF };
        private static readonly byte[] Float32BE = new byte[] { 0x83, 0x00, 0x00, 0x00, 0xBF, 0xF3, 0x33, 0x33 };

        private static readonly BoolArgDecoder BoolArgDecoder = new BoolArgDecoder();
        private static readonly byte[] Bool = new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 };

        private static readonly StringArgDecoder StringArgDecoder = new StringArgDecoder();
        private static readonly byte[] StringUtf8ArgLE = new byte[] { 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };
        private static readonly byte[] StringUtf8ArgBE = new byte[] { 0x00, 0x82, 0x00, 0x00, 0x00, 0x0B, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };
        private static readonly byte[] StringAsciiArgLE = new byte[] { 0x00, 0x02, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };
        private static readonly byte[] StringASciiArgBE = new byte[] { 0x00, 0x02, 0x00, 0x00, 0x00, 0x0B, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };

        [Benchmark]
        public void DecodeSignedInteger32bitLE() => SignedIntArgDecoder.Decode(SignedInt32, false, out _);

        [Benchmark]
        public void DecodeSignedInteger32bitBE() => SignedIntArgDecoder.Decode(SignedInt32, true, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitLE() => UnsignedIntArgDecoder.Decode(UnsignedInt32, false, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitBE() => UnsignedIntArgDecoder.Decode(UnsignedInt32, true, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitHex() => UnsignedIntArgDecoder.Decode(UnsignedInt32Hex, false, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitBin() => UnsignedIntArgDecoder.Decode(UnsignedInt32Bin, false, out _);

        [Benchmark]
        public void DecodeFloat32bitLE() => FloatArgDecoder.Decode(Float32LE, false, out _);

        [Benchmark]
        public void DecodeFloat32bitBE() => FloatArgDecoder.Decode(Float32BE, true, out _);

        [Benchmark]
        public void DecodeBool() => BoolArgDecoder.Decode(Bool, false, out _);

        [Benchmark]
        public void DecodeStringUtf8LE() => StringArgDecoder.Decode(StringUtf8ArgLE, false, out _);

        [Benchmark]
        public void DecodeStringUtf8BE() => StringArgDecoder.Decode(StringUtf8ArgBE, true, out _);

        [Benchmark]
        public void DecodeStringAsciiLE() => StringArgDecoder.Decode(StringAsciiArgLE, false, out _);

        [Benchmark]
        public void DecodeStringAsciiBE() => StringArgDecoder.Decode(StringASciiArgBE, true, out _);
    }
}
