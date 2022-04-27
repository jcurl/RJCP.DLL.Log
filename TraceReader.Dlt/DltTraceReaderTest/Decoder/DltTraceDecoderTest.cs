namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Dlt;

    // Testing the DltTraceDecoderBase is mostly done by the test classes `DltTraceDecoderCommonTest` and
    // `DltTraceDecoderCorruptTest`, which tests by instantiating the decoder through a factory and getting the reader,
    // thus an integration test. This test must access the APIs of the `DltTraceDecoderBase` directly.

    [TestFixture]
    public class DltTraceDecoderTest
    {
        private static readonly byte[] Data = new byte[] {
            0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
        };
        private static readonly int[] ReadChunks = { 0, 1, 2, 3, 5, 8, 13, 21, 50, 100 };

        private static IList<DltTraceLineBase> Decode(DltTraceDecoderBase traceDecoder, ReadOnlySpan<byte> data, bool flush, int chunkSize)
        {
            if (chunkSize == 0)
                return new List<DltTraceLineBase>(traceDecoder.Decode(data, 0, flush));

            List<DltTraceLineBase> lines = new List<DltTraceLineBase>();
            int offset = 0;
            while (offset < data.Length) {
                int chunkLength = Math.Min(chunkSize, data.Length - offset);

                IEnumerable<DltTraceLineBase> decodedLines;
                if (flush && offset + chunkLength == data.Length) {
                    // The last set of bytes, so flush
                    decodedLines = traceDecoder.Decode(data[offset..(offset + chunkLength)], offset, true);
                } else {
                    decodedLines = traceDecoder.Decode(data[offset..(offset + chunkLength)], offset, false);
                }
                lines.AddRange(decodedLines);
                offset += chunkSize;
            }

            return lines;
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeDlt(int chunkSize)
        {
            using (DltTraceDecoder decoder = new DltTraceDecoder()) {
                var lines = Decode(decoder, Data, false, chunkSize);

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].Position, Is.EqualTo(0));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeDltPacket(int chunkSize)
        {
            using (DltTraceDecoder decoder = new DltTraceDecoder()) {
                var lines = Decode(decoder, Data, true, chunkSize);

                Assert.That(lines.Count, Is.EqualTo(1));
                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].Position, Is.EqualTo(0));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeDltPacketDataAtEnd(int chunkSize)
        {
            byte[] packet = new byte[] {
                0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,
                0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73,
                0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00,
                0xFF, 0xFF, 0xFF, 0xFF, 0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31
            };

            using (DltTraceDecoder decoder = new DltTraceDecoder()) {
                var lines = Decode(decoder, packet, true, chunkSize);

                Assert.That(lines.Count, Is.EqualTo(2));

                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].Position, Is.EqualTo(0));

                Assert.That(lines[1].EcuId, Is.EqualTo(""));
                Assert.That(lines[1].ApplicationId, Is.EqualTo("SKIP"));
                Assert.That(lines[1].ContextId, Is.EqualTo("SKIP"));
                Assert.That(lines[1].Position, Is.EqualTo(43));
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeDltPacketDataAtStart(int chunkSize)
        {
            byte[] packet = new byte[] {
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,

                0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,
                0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73,
                0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
            };

            using (DltTraceDecoder decoder = new DltTraceDecoder()) {
                var lines = Decode(decoder, packet, true, chunkSize);

                Assert.That(lines.Count, Is.EqualTo(2));

                Assert.That(lines[0].EcuId, Is.EqualTo(""));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("SKIP"));
                Assert.That(lines[0].ContextId, Is.EqualTo("SKIP"));
                Assert.That(lines[0].Position, Is.EqualTo(0));

                Assert.That(lines[1].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[1].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[1].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[1].Position, Is.EqualTo(7));
            }
        }

        [Test]
        public void DecodeDltPacketSkippedStart()
        {
            byte[] packet = new byte[] {
                0xFF, 0xFF, 0xFF, 0xFF,

                0x44, 0x4C, 0x54, 0x01, 0x64, 0xFE, 0xF5, 0x5E, 0xFD, 0x3C, 0x0C, 0x00,
                0x45, 0x43, 0x55, 0x31,

                0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54,
                0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73,
                0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00
            };

            using (DltFileTraceDecoder decoder = new DltFileTraceDecoder()) {
                List<DltTraceLineBase> lines = new List<DltTraceLineBase>();

                lines.AddRange(decoder.Decode(packet.AsSpan(0, 4), 0, false));
                Assert.That(lines.Count, Is.EqualTo(0));

                lines.AddRange(decoder.Decode(packet.AsSpan(4), 4, true));

                Assert.That(lines.Count, Is.EqualTo(2));

                Assert.That(lines[0].EcuId, Is.EqualTo(""));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("SKIP"));
                Assert.That(lines[0].ContextId, Is.EqualTo("SKIP"));
                Assert.That(lines[0].Position, Is.EqualTo(0));

                Assert.That(lines[1].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[1].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[1].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[1].Position, Is.EqualTo(4));
            }
        }

        [Test]
        public void DecodeDltPacketSkippedEnd()
        {
            byte[] packet = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0x64, 0xFE, 0xF5, 0x5E, 0xFD, 0x3C, 0x0C, 0x00, //  0 ..  11
                0x45, 0x43, 0x55, 0x31,                                                 // 12 ..  15

                0x3D, 0x7F, 0x00, 0x2B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x32, // 16 ..  27
                0x00, 0x00, 0x30, 0x16, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, // 28 ..  39
                0x58, 0x31, 0x00, 0x82, 0x00, 0x00, 0x0B, 0x00, 0x4D, 0x65, 0x73, 0x73, // 40 ..  51
                0x61, 0x67, 0x65, 0x20, 0x30, 0x30, 0x00,                               // 52 ..  58

                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 59 ..  70
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 71 ..  82
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 83 ..  94
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF  // 95 .. 106
            };

            using (DltFileTraceDecoder decoder = new DltFileTraceDecoder()) {
                List<DltTraceLineBase> lines = new List<DltTraceLineBase>();

                // This special sequence causes the decoder to enter a if statement where there is no cached data (it's
                // looking for a header), but there are skipped bytes.
                lines.AddRange(decoder.Decode(packet.AsSpan(0, 59), 0, false));
                Assert.That(lines.Count, Is.EqualTo(1));

                lines.AddRange(decoder.Decode(packet.AsSpan(59, 21), 59, false));
                Assert.That(lines.Count, Is.EqualTo(1));

                lines.AddRange(decoder.Decode(packet.AsSpan(80), 80, true));
                Assert.That(lines.Count, Is.EqualTo(2));

                Assert.That(lines[0].EcuId, Is.EqualTo("ECU1"));
                Assert.That(lines[0].ApplicationId, Is.EqualTo("APP1"));
                Assert.That(lines[0].ContextId, Is.EqualTo("CTX1"));
                Assert.That(lines[0].Position, Is.EqualTo(0));

                Assert.That(lines[1].EcuId, Is.EqualTo(""));
                Assert.That(lines[1].ApplicationId, Is.EqualTo("SKIP"));
                Assert.That(lines[1].ContextId, Is.EqualTo("SKIP"));
                Assert.That(lines[1].Position, Is.EqualTo(59));
                Assert.That(lines[1].Text, Is.EqualTo("Skipped: 48 bytes; Reason: Searching for next packet"));
            }
        }
    }
}
