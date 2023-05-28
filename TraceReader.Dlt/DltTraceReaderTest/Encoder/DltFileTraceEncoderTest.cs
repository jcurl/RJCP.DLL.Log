namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using Dlt;
    using Dlt.Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class DltFileTraceEncoderTest
    {
        public static readonly byte[] StorageHeader = new byte[] {
            0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x45, 0x43, 0x55, 0x31
        };

        [Test]
        public void EncodeNoEcuIdNoTimeStamp()
        {
            byte[] buffer = new byte[65535];
            IDltLineBuilder builder = new DltLineBuilder()
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            byte[] expected = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x21, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
            };

            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Result<int> result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result.Value, Is.EqualTo(expected.Length));
            Assert.That(buffer[..result.Value], Is.EqualTo(expected));
        }

        [Test]
        public void EncodeTimeStampLocal()
        {
            byte[] buffer = new byte[65535];
            IDltLineBuilder builder = new DltLineBuilder()
                .SetTimeStamp(new DateTime(2023, 05, 16, 12, 24, 22, 55, DateTimeKind.Local))
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            byte[] expected = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x21, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
            };

            // Because we're using a local time zone, we need to calculate the UTC version, which is stored in the
            // storage header.
            DateTime utcTime = builder.TimeStamp.ToUniversalTime();
            long seconds = new DateTimeOffset(utcTime).ToUnixTimeSeconds();
            int fractionalSecondTicks = (int)(utcTime.Ticks % TimeSpan.TicksPerSecond);
            int microseconds = fractionalSecondTicks / ((int)TimeSpan.TicksPerMillisecond / 1000);
            BitOperations.Copy32ShiftLittleEndian(seconds, expected.AsSpan(4..8));
            BitOperations.Copy32ShiftLittleEndian(microseconds, expected.AsSpan(8..12));

            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Result<int> result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result.Value, Is.EqualTo(expected.Length));
            Assert.That(buffer[..result.Value], Is.EqualTo(expected));
        }

        [Test]
        public void EncodeTimeStampUtc()
        {
            byte[] buffer = new byte[65535];
            IDltLineBuilder builder = new DltLineBuilder()
                .SetTimeStamp(new DateTime(2023, 05, 16, 12, 24, 22, 55, DateTimeKind.Utc))
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            byte[] expected = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0xF6, 0x75, 0x63, 0x64, 0xD8, 0xD6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x21, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
            };

            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Result<int> result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result.Value, Is.EqualTo(expected.Length));
            Assert.That(buffer[..result.Value], Is.EqualTo(expected));
        }

        [Test]
        public void EncodeEcuIdStorageHeaderOnly()
        {
            byte[] buffer = new byte[65535];
            IDltLineBuilder builder = new DltLineBuilder()
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            byte[] expected = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x45, 0x43, 0x55, 0x31,
                0x21, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
            };

            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();

            // The storage header contains the ECU ID, even if the feature bit is not set. This is true in the decoder
            // case also.
            DltTraceLineBase line = builder.GetResult();
            line.EcuId = "ECU1";

            Result<int> result = encoder.Encode(buffer, line);
            Assert.That(result.Value, Is.EqualTo(expected.Length));
            Assert.That(buffer[..result.Value], Is.EqualTo(expected));
        }

        [Test]
        public void EncodeEcuId()
        {
            byte[] buffer = new byte[65535];
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            byte[] expected = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x45, 0x43, 0x55, 0x31,
                0x25, 0x00, 0x00, 0x2E, 0x45, 0x43, 0x55, 0x31, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
            };

            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Result<int> result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result.Value, Is.EqualTo(expected.Length));
            Assert.That(buffer[..result.Value], Is.EqualTo(expected));
        }

        [Test]
        public void EncodeFull()
        {
            byte[] buffer = new byte[65535];
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetTimeStamp(new DateTime(2023, 05, 16, 12, 24, 22, 55, DateTimeKind.Utc))
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            byte[] expected = new byte[] {
                0x44, 0x4C, 0x54, 0x01, 0xF6, 0x75, 0x63, 0x64, 0xD8, 0xD6, 0x00, 0x00, 0x45, 0x43, 0x55, 0x31,
                0x25, 0x00, 0x00, 0x2E, 0x45, 0x43, 0x55, 0x31, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
            };

            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Result<int> result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result.Value, Is.EqualTo(expected.Length));
            Assert.That(buffer[..result.Value], Is.EqualTo(expected));
        }

        [Test]
        public void EncodeNullLine()
        {
            byte[] buffer = new byte[65535];
            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();

            Assert.That(() => {
                _ = encoder.Encode(buffer, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(19)]
        [TestCase(20)]
        public void EncodeInsufficientBuffer(int len)
        {
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetTimeStamp(new DateTime(2023, 05, 16, 12, 24, 22, 55, DateTimeKind.Utc))
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(false)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            byte[] buffer = new byte[len];
            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            ITraceEncoder<DltTraceLineBase> encoder = factory.Create();
            Assert.That(encoder.Encode(buffer, builder.GetResult()).HasValue, Is.False);
        }
    }
}
