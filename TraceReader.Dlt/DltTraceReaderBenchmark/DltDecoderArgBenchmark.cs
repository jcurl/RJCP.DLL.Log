﻿namespace RJCP.Diagnostics.Log
{
    using BenchmarkDotNet.Attributes;
    using Dlt.Verbose;

    public class DltDecoderArgBenchmark
    {
        private static readonly SignedIntArgDecoder SignedIntArgDecoder = new();
        private static readonly byte[] SignedInt32 = { 0x23, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };

        private static readonly UnsignedIntArgDecoder UnsignedIntArgDecoder = new();
        private static readonly byte[] UnsignedInt32 = { 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };
        private static readonly byte[] UnsignedInt32Hex = { 0x43, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 };
        private static readonly byte[] UnsignedInt32Bin = { 0x43, 0x80, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 };

        private static readonly FloatArgDecoder FloatArgDecoder = new();
        private static readonly byte[] Float32LE = { 0x83, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0xBF };
        private static readonly byte[] Float32BE = { 0x83, 0x00, 0x00, 0x00, 0xBF, 0xF3, 0x33, 0x33 };

        private static readonly BoolArgDecoder BoolArgDecoder = new();
        private static readonly byte[] Bool = { 0x11, 0x00, 0x00, 0x00, 0x00 };

        private static readonly StringArgDecoder StringArgDecoder = new();
        private static readonly byte[] StringUtf8ArgLE = { 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };
        private static readonly byte[] StringUtf8ArgBE = { 0x00, 0x82, 0x00, 0x00, 0x00, 0x0B, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };
        private static readonly byte[] StringAsciiArgLE = { 0x00, 0x02, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };
        private static readonly byte[] StringASciiArgBE = { 0x00, 0x02, 0x00, 0x00, 0x00, 0x0B, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00 };

        [Benchmark]
        public void DecodeSignedInteger32bitLE() => SignedIntArgDecoder.Decode(0x23, SignedInt32, false, out _);

        [Benchmark]
        public void DecodeSignedInteger32bitBE() => SignedIntArgDecoder.Decode(0x23, SignedInt32, true, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitLE() => UnsignedIntArgDecoder.Decode(0x43, UnsignedInt32, false, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitBE() => UnsignedIntArgDecoder.Decode(0x43, UnsignedInt32, true, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitHex() => UnsignedIntArgDecoder.Decode(0x010043, UnsignedInt32Hex, false, out _);

        [Benchmark]
        public void DecodeUnsignedInteger32bitBin() => UnsignedIntArgDecoder.Decode(0x018043, UnsignedInt32Bin, false, out _);

        [Benchmark]
        public void DecodeFloat32bitLE() => FloatArgDecoder.Decode(0x83, Float32LE, false, out _);

        [Benchmark]
        public void DecodeFloat32bitBE() => FloatArgDecoder.Decode(0x83, Float32BE, true, out _);

        [Benchmark]
        public void DecodeBool() => BoolArgDecoder.Decode(0x11, Bool, false, out _);

        [Benchmark]
        public void DecodeStringUtf8LE() => StringArgDecoder.Decode(0x8200, StringUtf8ArgLE, false, out _);

        [Benchmark]
        public void DecodeStringUtf8BE() => StringArgDecoder.Decode(0x8200, StringUtf8ArgBE, true, out _);

        [Benchmark]
        public void DecodeStringAsciiLE() => StringArgDecoder.Decode(0x0200, StringAsciiArgLE, false, out _);

        [Benchmark]
        public void DecodeStringAsciiBE() => StringArgDecoder.Decode(0x0200, StringASciiArgBE, true, out _);
    }
}
