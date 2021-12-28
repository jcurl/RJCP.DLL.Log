﻿namespace RJCP.Diagnostics.Log
{
    using System.Text;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Engines;
    using Decoder;

    public class DltDecoderBenchmark
    {
        [GlobalSetup]
        public void InitializeCodePages()
        {
            // Required to decode ISO-8859-15 when encoded as ASCII.
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
        }

        private static readonly DltFileTraceDecoder DltFilePacketsDecoder = new DltFileTraceDecoder();

        // 10 DLT packets with a storage header. Each line is one packet.
        private static readonly byte[] DltStringFilePackets = {
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0x20, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0x83, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x80, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x20, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x31, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0xFF, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x82, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x32, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x20, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x83, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x33, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x45, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x84, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x34, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x6A, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x85, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x35, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x87, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x86, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x36, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0xA0, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x87, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x37, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0xBE, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x88, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x38, 0x00,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0xD6, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x89, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x39, 0x00,
        };

        private static readonly byte[] DltInt32FilePackets = {
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0x20, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x7F, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0x83, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x80, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x20, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x79,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x61, 0xFF, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x82, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x7A,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x20, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x83, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x7B,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x45, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x84, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x7C,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x6A, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x85, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x7D,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0x87, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x86, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x7E,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0xA0, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x87, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x7F,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0xBE, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x88, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x80,
            0x44, 0x4C, 0x54, 0x01, 0xB7, 0xA8, 0xBB, 0x62, 0xD6, 0xA0, 0x03, 0x00, 0x45, 0x43, 0x55, 0x31, 0x3D, 0x89, 0x00, 0x22, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x2A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x81,
        };

        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public void DecodeStringFilePackets() =>
            DltFilePacketsDecoder.Decode(DltStringFilePackets, 0).Consume(consumer);

        [Benchmark]
        public void DecodeIntFilePackets() =>
            DltFilePacketsDecoder.Decode(DltInt32FilePackets, 0).Consume(consumer);
    }
}
