namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using Dlt;
    using Dlt.ArgEncoder;
    using Dlt.Args;
    using NUnit.Framework;

    [TestFixture(EncoderTestType.EncoderDefault, true)]
    [TestFixture(EncoderTestType.EncoderDefault, false)]
    [TestFixture(EncoderTestType.Encoder, true)]
    [TestFixture(EncoderTestType.Encoder, false)]
    [TestFixture(EncoderTestType.Factory, true)]
    [TestFixture(EncoderTestType.Factory, false)]
    public class DltTraceEncoderCommonTest
    {
        public enum EncoderTestType
        {
            EncoderDefault,
            Encoder,
            Factory
        }

        private readonly EncoderTestType m_UseFactory;
        private readonly bool m_BigEndian;

        public DltTraceEncoderCommonTest(EncoderTestType useFactory, bool bigEndian)
        {
            m_UseFactory = useFactory;
            m_BigEndian = bigEndian;
        }

        private ITraceEncoder<DltTraceLineBase> GetEncoder()
        {
            switch (m_UseFactory) {
            case EncoderTestType.Factory:
                ITraceEncoderFactory<DltTraceLineBase> factory = new DltTraceEncoderFactory();
                return factory.Create();
            case EncoderTestType.EncoderDefault:
                return new DltTraceEncoder();
            case EncoderTestType.Encoder:
                return new DltTraceEncoder(new VerboseDltEncoder());
            default:
                throw new NotSupportedException();
            }
        }

        [Test]
        public void StandardHeaderMinimum()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x23, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x21, 0x00, 0x00, 0x2A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void StandardHeaderEcuId()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x27, 0x00, 0x00, 0x2E, 0x45, 0x43, 0x55, 0x31, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x25, 0x00, 0x00, 0x2E, 0x45, 0x43, 0x55, 0x31, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void StandardHeaderTimeStamp()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x33, 0x00, 0x00, 0x2E, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x31, 0x00, 0x00, 0x2E, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void StandardHeaderEcuTimeStamp()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x37, 0x00, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x35, 0x00, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void StandardHeaderFull()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x3F, 0x00, 0x00, 0x36, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x3D, 0x00, 0x00, 0x36, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetSessionId(15)
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void SetCount()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x37, 0x7F, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x35, 0x7F, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .SetCount(127)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));

            // This time, we don't set the count, it should increment.
            expected[1] = 0x80;
            builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));

            // Increment again
            expected[1] = 0x81;
            result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void RolloverCount()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x37, 0xFF, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x35, 0xFF, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .SetCount(255)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));

            // This time, we don't set the count, it should increment.
            expected[1] = 0x00;
            builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });
            result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [Test]
        public void NoAppId()
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x37, 0x00, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x35, 0x00, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void PartialAppId(int len)
        {
            byte[] buffer = new byte[65535];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();

            byte[] expected = m_BigEndian ?
                new byte[] {
                    0x37, 0x00, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x00, 0x82, 0x00, 0x00, 0x10, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x00, 0x00, 0x00, 0x22, 0x00, 0x2D
                } :
                new byte[] {
                    0x35, 0x00, 0x00, 0x32, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x00, 0x0A, 0x41, 0x02, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31,  // StdHdr + ExtHdr
                    0x00, 0x82, 0x00, 0x00, 0x10, 0x00, 0x54, 0x65, 0x6D, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x69, 0x73, 0x3A, 0x00,
                    0x22, 0x00, 0x00, 0x00, 0x2D, 0x00
                };

            for (int i = len; i < 4; i++) {
                expected[14 + i] = 0;
                expected[18 + i] = 0;
            }

            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1"[..len])
                .SetContextId("CTX1"[..len])
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(expected.Length));
            Assert.That(buffer[0..result], Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(41)]
        public void InsufficientBufferMinimal(int bufferLen)
        {
            byte[] buffer = new byte[bufferLen];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();
            IDltLineBuilder builder = new DltLineBuilder()
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(-1));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(19)]
        [TestCase(45)]
        public void InsufficientBufferEcuId(int bufferLen)
        {
            byte[] buffer = new byte[bufferLen];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(-1));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(21)]
        [TestCase(22)]
        [TestCase(23)]
        [TestCase(53)]
        public void InsufficientBufferFull(int bufferLen)
        {
            byte[] buffer = new byte[bufferLen];
            using ITraceEncoder<DltTraceLineBase> encoder = GetEncoder();
            IDltLineBuilder builder = new DltLineBuilder()
                .SetEcuId("ECU1")
                .SetDeviceTimeStamp(TimeSpan.TicksPerMillisecond)
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetSessionId(15)
                .SetDltType(DltType.LOG_INFO)
                .SetBigEndian(m_BigEndian)
                .AddArguments(new IDltArg[] {
                    new StringDltArg("Temperature is:"),
                    new SignedIntDltArg(45, 2)
                });

            int result = encoder.Encode(buffer, builder.GetResult());
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
