namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class TracePacketReaderTest
    {
        [Test]
        public void NullPacket()
        {
            Assert.That(() => {
                _ = new TracePacketReader(null, new DltTraceDecoderFactory());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullDecoder()
        {
            Assert.That(() => {
                _ = new TracePacketReader(new EmptyPacketReceiver(), null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InitialPacketReader()
        {
            TracePacketReader reader = new(new EmptyPacketReceiver(), new DltTraceDecoderFactory());
            reader.Dispose();
        }

        private static readonly byte[] line1 = new byte[] {
            0x35, 0x00, 0x00, 0x49, 0x45, 0x43, 0x55, 0x31, 0x32, 0xac, 0xe6, 0xba, 0x41, 0x01, 0x41, 0x50,
            0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x2d, 0x00, 0x41, 0x20, 0x44, 0x4c,
            0x54, 0x20, 0x6d, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x66, 0x72, 0x6f, 0x6d, 0x20, 0x31,
            0x39, 0x32, 0x2e, 0x31, 0x36, 0x38, 0x2e, 0x31, 0x2e, 0x31, 0x30, 0x39, 0x2e, 0x20, 0x43, 0x6f,
            0x75, 0x6e, 0x74, 0x20, 0x69, 0x73, 0x20, 0x31, 0x00
        };

        private static readonly byte[] line2 = new byte[] {
            0x35, 0x01, 0x00, 0x49, 0x45, 0x43, 0x55, 0x31, 0x32, 0xac, 0xe6, 0xba, 0x41, 0x01, 0x41, 0x50,
            0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x2d, 0x00, 0x41, 0x20, 0x44, 0x4c,
            0x54, 0x20, 0x6d, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x20, 0x66, 0x72, 0x6f, 0x6d, 0x20, 0x31,
            0x39, 0x32, 0x2e, 0x31, 0x36, 0x38, 0x2e, 0x31, 0x2e, 0x31, 0x30, 0x39, 0x2e, 0x20, 0x43, 0x6f,
            0x75, 0x6e, 0x74, 0x20, 0x69, 0x73, 0x20, 0x32, 0x00
        };

        [Test]
        public async Task GetPacketSingleChannel()
        {
            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1),
                (0, line2)
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 1"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(0));

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 2"));
                Assert.That(line.Position, Is.EqualTo(line1.Length));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetPacketMultiChannel()
        {
            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1),
                (1, line2)
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 1"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(0));

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 2"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetFragmentedPacketSingleChannel()
        {
            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1[0..16]),
                (0, line1[16..32]),
                (0, line1[32..48]),
                (0, line1[48..64]),
                (0, line1[64..])
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 1"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(0));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetFragmentedPacketMultiChannelInterleaved()
        {
            // Interleaving shows that we are really using two different decoders.

            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1[0..16]),
                (1, line2[0..16]),
                (0, line1[16..32]),
                (1, line2[16..32]),
                (0, line1[32..48]),
                (1, line2[32..48]),
                (0, line1[48..64]),
                (1, line2[48..64]),
                (0, line1[64..]),
                (1, line2[64..]),
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 1"));
                Assert.That(line.Position, Is.EqualTo(0));

                // Because the packets are incomplete and they're interleaved, we'll get two callbacks in the last call.
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 2"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetFragmentedPacketMultiChannelInterleavedIncomplete1()
        {
            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1[0..16]),
                (1, line2[0..16]),
                (0, line1[16..32]),
                (1, line2[16..32]),
                (0, line1[32..48]),
                (1, line2[32..48]),
                (0, line1[48..64]),
                (1, line2[48..64]),
                (1, line2[64..]),
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 2"));
                Assert.That(line.Position, Is.EqualTo(0));

                // Because the packets are incomplete and they're interleaved, we'll get two callbacks in the last call.
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("Skipped: 64 bytes; Reason: Flushing packets at end of stream"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetFragmentedPacketMultiChannelInterleavedIncomplete2()
        {
            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1[0..16]),
                (1, line2[0..16]),
                (0, line1[16..32]),
                (1, line2[16..32]),
                (0, line1[32..48]),
                (1, line2[32..48]),
                (0, line1[48..64]),
                (1, line2[48..64]),
                (0, line1[64..]),
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("A DLT message from 192.168.1.109. Count is 1"));
                Assert.That(line.Position, Is.EqualTo(0));

                // Because the packets are incomplete and they're interleaved, we'll get two callbacks in the last call.
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("Skipped: 64 bytes; Reason: Flushing packets at end of stream"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetFragmentedPacketMultiChannelInterleavedIncompleteBoth()
        {
            using (SimulatedPacketReceiver packet = new(new[] {
                (0, line1[0..16]),
                (1, line2[0..16]),
                (0, line1[16..32]),
                (1, line2[16..32]),
                (0, line1[32..48]),
                (1, line2[32..48]),
                (0, line1[48..64]),
                (1, line2[48..60]),
            })) {
                int channels = -1;
                packet.Open();
                packet.NewChannel += (s, e) => {
                    channels = e.ChannelNumber;
                };
                Assert.That(packet.ChannelCount, Is.EqualTo(0));

                TracePacketReader reader = new(packet, new DltTraceDecoderFactory());
                DltTraceLineBase line;

                // The order of flushing should be based on the channel number. Channel 0 is flushed first.

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("Skipped: 64 bytes; Reason: Flushing packets at end of stream"));
                Assert.That(line.Position, Is.EqualTo(0));

                // Because the packets are incomplete and they're interleaved, we'll get two callbacks in the last call.
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line.Text, Is.EqualTo("Skipped: 60 bytes; Reason: Flushing packets at end of stream"));
                Assert.That(line.Position, Is.EqualTo(0));
                Assert.That(channels, Is.EqualTo(1));

                line = await reader.GetLineAsync();
                Assert.That(line, Is.Null);
                Assert.That(packet.ChannelCount, Is.EqualTo(2));
            }
        }
    }
}
