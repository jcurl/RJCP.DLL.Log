﻿namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Legacy
{
    using System;
    using Infrastructure;
    using NUnit.Framework;
    using RJCP.Core;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Args;

    internal static class PcapLegacyLines
    {
        public static readonly DltTraceLine Line1 =
            new(new[] { new StringDltArg("DLT Argument test string..") }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                Position = 0,
                Line = 0,
                Count = 11,
                Type = DltType.LOG_INFO,
                TimeStamp = new DateTime(2020, 6, 26, 13, 55, 48).AddNanoSeconds(802045000),
                SessionId = 910,
                DeviceTimeStamp = new TimeSpan(0, 0, 8).AddNanoSeconds(711400000),
                Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                           DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                           DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                           DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
            };

        public static readonly DltTraceLine Line2 =
            new(new[] { new StringDltArg("DLT Argument test string 2") }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                Position = 0,
                Line = 0,
                Count = 12,
                Type = DltType.LOG_INFO,
                TimeStamp = new DateTime(2020, 6, 26, 13, 55, 48).AddNanoSeconds(802045000),
                SessionId = 910,
                DeviceTimeStamp = new TimeSpan(0, 0, 8).AddNanoSeconds(711500000),
                Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                           DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                           DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                           DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
            };

        public static readonly DltTraceLine Line3 =
            new(new[] { new StringDltArg("DLT Argument test string..") }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                Position = 0,
                Line = 0,
                Count = 13,
                Type = DltType.LOG_INFO,
                TimeStamp = new DateTime(2020, 6, 26, 13, 55, 50).AddNanoSeconds(806141000),
                SessionId = 910,
                DeviceTimeStamp = new TimeSpan(0, 0, 8).AddNanoSeconds(711400000),
                Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                           DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                           DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                           DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
            };

        public static readonly DltTraceLine Line4 =
            new(new[] { new StringDltArg("DLT Argument test string 2") }) {
                EcuId = "ECU1",
                ApplicationId = "APP1",
                ContextId = "CTX1",
                Position = 0,
                Line = 0,
                Count = 14,
                Type = DltType.LOG_INFO,
                TimeStamp = new DateTime(2020, 6, 26, 13, 55, 50).AddNanoSeconds(806141000),
                SessionId = 910,
                DeviceTimeStamp = new TimeSpan(0, 0, 8).AddNanoSeconds(711500000),
                Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                           DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                           DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                           DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
            };
    }

    /// <summary>
    /// Test decoding legacy PCAP formats.
    /// </summary>
    /// <typeparam name="TDec">The type of the decoder.</typeparam>
    /// <remarks>
    /// Both decoders should behave the same and transparent for the same input. The <see cref="DltPcapTraceDecoder"/>
    /// tries to determine the right format based on the first 32-bits.
    /// </remarks>
    [TestFixture(typeof(DltPcapLegacyDecoder), true)]
    [TestFixture(typeof(DltPcapLegacyDecoder), false)]
    [TestFixture(typeof(DltPcapTraceDecoder), true)]
    [TestFixture(typeof(DltPcapTraceDecoder), false)]
    public class PcapLegacyDecoderTest<TDec> : PcapDecoderTestBase where TDec : class, ITraceDecoder<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcapLegacyDecoderTest{TDec}"/> class.
        /// </summary>
        /// <param name="outputStream">
        /// If set to <see langword="true"/>, test with an <see cref="IOutputStream"/>.
        /// </param>
        public PcapLegacyDecoderTest(bool outputStream) : base(outputStream) { }

        private TDec Create()
        {
            return Create(false);
        }

        private TDec Create(bool nullOutput)
        {
            ITraceDecoderFactory<DltTraceLineBase> factory;
            if (nullOutput || MemOutputStream is null) {
                factory = new PcapTraceDecoderFactory(null, null);
                if (typeof(TDec) == typeof(DltPcapLegacyDecoder)) return new DltPcapLegacyDecoder(factory) as TDec;
                if (typeof(TDec) == typeof(DltPcapTraceDecoder)) return new DltPcapTraceDecoder() as TDec;
            } else {
                factory = new PcapTraceDecoderFactory(MemOutputStream, null);
                if (typeof(TDec) == typeof(DltPcapLegacyDecoder)) return new DltPcapLegacyDecoder(factory) as TDec;
                if (typeof(TDec) == typeof(DltPcapTraceDecoder)) return new DltPcapTraceDecoder(MemOutputStream, null) as TDec;
            }

            throw new NotImplementedException();
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ValidHeaderLittleEndianMicroseconds(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, // Magic
                0x02, 0x00, 0x04, 0x00, // Major, Minor version
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, // Snap Length
                0x01, 0x00, 0x00, 0x00 // Link Type
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.True);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Microseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));

                    DateTime expected = DateTime.UnixEpoch.AddNanoSeconds(100000);
                    DateTime converted = legacy.Format.GetTimeStamp(0, 100);
                    Assert.That(converted, Is.EqualTo(expected));
                    Assert.That(converted.Kind, Is.EqualTo(DateTimeKind.Utc));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ValidHeaderLittleEndianNanoseconds(int chunk)
        {
            byte[] header = new byte[] {
                0x4D, 0x3C, 0xB2, 0xA1, // Magic
                0x02, 0x00, 0x04, 0x00, // Major, Minor version
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, // Snap Length
                0x01, 0x00, 0x00, 0x00 // Link Type
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.True);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Nanoseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));

                    DateTime expected = DateTime.UnixEpoch.AddNanoSeconds(100);
                    DateTime converted = legacy.Format.GetTimeStamp(0, 100);
                    Assert.That(converted, Is.EqualTo(expected));
                    Assert.That(converted.Kind, Is.EqualTo(DateTimeKind.Utc));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ValidHeaderBigEndianMicoseconds(int chunk)
        {
            byte[] header = new byte[] {
                0xA1, 0xB2, 0xC3, 0xD4, // Magic
                0x00, 0x02, 0x00, 0x04, // Major, Minor version
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x04, 0x00, 0x00, // Snap Length
                0x00, 0x00, 0x00, 0x01 // Link Type
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.False);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Microseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ValidHeaderBigEndianNanoseconds(int chunk)
        {
            byte[] header = new byte[] {
                0xA1, 0xB2, 0x3C, 0x4D, // Magic
                0x00, 0x02, 0x00, 0x04, // Major, Minor version
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x04, 0x00, 0x00, // Snap Length
                0x00, 0x00, 0x00, 0x01 // Link Type
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.False);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Nanoseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ValidHeaderEthernetType(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ValidHeaderLinuxLlcType(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x71, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_LINUX_SLL));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void FcsLength(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x24
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(4));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void FcsLengthPNotSet(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x20
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, header, chunk);

                Assert.That(lines, Is.Empty);
                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidHeader(int chunk)
        {
            byte[] header = new byte[] {
                0xD5, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidMajorVersionSmall(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x01, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidMajorVersionLarge(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x03, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidMinorVersionSmall(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x03, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidMinorVersionLarge(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x05, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void InvalidSnapLength(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void UnknownLinkType(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x02, 0x00, 0x00, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ReservedSet1(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x10, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ReservedSet2(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x08, 0x00
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ReservedSet3(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x02
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [TestCaseSource(nameof(ReadChunks))]
        public void ReservedSet4(int chunk)
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x08
            };

            using (TDec decoder = Create()) {
                Assert.That(() => {
                    _ = Decode(decoder, header, chunk);
                }, Throws.TypeOf<UnknownPcapFileFormatException>());
            }
        }

        [Test]
        public void DisposeTest()
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, // Magic
                0x02, 0x00, 0x04, 0x00, // Major, Minor version
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, // Snap Length
                0x01, 0x00, 0x00, 0x00 // Link Type
            };

            using (TDec decoder = Create()) {
                decoder.Dispose();
                Assert.That(() => {
                    _ = decoder.Decode(header, 0);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void DisposeTest2()
        {
            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, // Magic
                0x02, 0x00, 0x04, 0x00, // Major, Minor version
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, // Snap Length
                0x01, 0x00, 0x00, 0x00 // Link Type
            };

            using (TDec decoder = Create()) {
                _ = decoder.Decode(header, 0);
                decoder.Dispose();

                Assert.That(() => {
                    _ = decoder.Decode(header, 0);
                }, Throws.TypeOf<ObjectDisposedException>());
            }
        }

        /// <summary>
        /// Decodes two DLT packets in one Ethernet Frame. Checks the PCAP time stamp matches.
        /// </summary>
        [TestCaseSource(nameof(ReadChunks))]
        public void DecodePacket(int chunk)
        {
            byte[] packet = new byte[] {
                // PCAP Header
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //   0 ..  15
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00,                                                 //  16 ..  23

                // Packet
                0x64, 0xFE, 0xF5, 0x5E, 0xFD, 0x3C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00, //  24 ..  39
                0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00, //  40 ..  55
                0x00, 0x92, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x2A, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF, //  56 ..  71
                0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x7E, 0x69, 0x15, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43, //  72 ..  87
                0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, //  88 .. 103
                0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, // 104 .. 119
                0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, // 120 .. 135
                0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x3D, 0x0C, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, // 136 .. 151
                0x8E, 0x00, 0x01, 0x54, 0x4B, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, // 152 .. 167
                0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E, // 168 .. 183
                0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x20, 0x32, 0x00  // 184 .. 199
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, packet, chunk);

                Assert.That(lines, Has.Count.EqualTo(2));
                Assert.That(lines[0].Features.TimeStamp, Is.True);
                Assert.That(lines[0].TimeStamp, Is.EqualTo(PcapLegacyLines.Line1.TimeStamp));
                Assert.That(lines[0].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line1.DeviceTimeStamp));
                Assert.That(lines[0].Text, Is.EqualTo(PcapLegacyLines.Line1.Text));
                Assert.That(lines[0].Count, Is.EqualTo(PcapLegacyLines.Line1.Count));
                Assert.That(lines[0].Line, Is.EqualTo(0));
                Assert.That(lines[0].Position, Is.EqualTo(82));
                Assert.That(lines[1].Features.TimeStamp, Is.True);
                Assert.That(lines[1].TimeStamp, Is.EqualTo(PcapLegacyLines.Line2.TimeStamp));
                Assert.That(lines[1].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line2.DeviceTimeStamp));
                Assert.That(lines[1].Text, Is.EqualTo(PcapLegacyLines.Line2.Text));
                Assert.That(lines[1].Count, Is.EqualTo(PcapLegacyLines.Line2.Count));
                Assert.That(lines[1].Line, Is.EqualTo(1));
                Assert.That(lines[1].Position, Is.EqualTo(141));

                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.True);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Microseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));

                    DateTime expected = DateTime.UnixEpoch.AddNanoSeconds(100000);
                    Assert.That(legacy.Format.GetTimeStamp(0, 100), Is.EqualTo(expected));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        /// <summary>
        /// Decodes two Ethernet frames. Checks the time stamps are correct.
        /// </summary>
        [TestCaseSource(nameof(ReadChunks))]
        public void DecodePackets(int chunk)
        {
            byte[] packet = new byte[] {
                // PCAP Header
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //   0 ..  15
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00,                                                 //  16 ..  23

                // Packet
                0x64, 0xFE, 0xF5, 0x5E, 0xFD, 0x3C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00, //  24 ..  39
                0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00, //  40 ..  55
                0x00, 0x92, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x2A, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF, //  56 ..  71
                0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x7E, 0x69, 0x15, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43, //  72 ..  87
                0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, //  88 .. 103
                0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, // 104 .. 119
                0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, // 120 .. 135
                0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x3D, 0x0C, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, // 136 .. 151
                0x8E, 0x00, 0x01, 0x54, 0x4B, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, // 152 .. 167
                0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E, // 168 .. 183
                0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x20, 0x32, 0x00, // 184 .. 199

                // Packet
                0x66, 0xFE, 0xF5, 0x5E, 0xFD, 0x4C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00, // 200 .. 215
                0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00, // 216 .. 231
                0x00, 0x92, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x2A, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF, // 232 .. 247
                0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x7E, 0x69, 0x15, 0x3D, 0x0D, 0x00, 0x3B, 0x45, 0x43, // 248 .. 263
                0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, // 264 .. 279
                0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, // 280 .. 295
                0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, // 296 .. 311
                0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x3D, 0x0E, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, // 312 .. 327
                0x8E, 0x00, 0x01, 0x54, 0x4B, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, // 328 .. 343
                0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E, // 344 .. 359
                0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x20, 0x32, 0x00  // 360 .. 375
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, packet, chunk);

                Assert.That(lines, Has.Count.EqualTo(4));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(PcapLegacyLines.Line1.TimeStamp));
                Assert.That(lines[0].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line1.DeviceTimeStamp));
                Assert.That(lines[0].Text, Is.EqualTo(PcapLegacyLines.Line1.Text));
                Assert.That(lines[0].Count, Is.EqualTo(PcapLegacyLines.Line1.Count));
                Assert.That(lines[0].Line, Is.EqualTo(0));
                Assert.That(lines[0].Position, Is.EqualTo(82));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(PcapLegacyLines.Line2.TimeStamp));
                Assert.That(lines[1].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line2.DeviceTimeStamp));
                Assert.That(lines[1].Text, Is.EqualTo(PcapLegacyLines.Line2.Text));
                Assert.That(lines[1].Count, Is.EqualTo(PcapLegacyLines.Line2.Count));
                Assert.That(lines[1].Line, Is.EqualTo(1));
                Assert.That(lines[1].Position, Is.EqualTo(141));
                Assert.That(lines[2].TimeStamp, Is.EqualTo(PcapLegacyLines.Line3.TimeStamp));
                Assert.That(lines[2].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line3.DeviceTimeStamp));
                Assert.That(lines[2].Text, Is.EqualTo(PcapLegacyLines.Line3.Text));
                Assert.That(lines[2].Count, Is.EqualTo(PcapLegacyLines.Line3.Count));
                Assert.That(lines[2].Line, Is.EqualTo(2));
                Assert.That(lines[2].Position, Is.EqualTo(258));
                Assert.That(lines[3].TimeStamp, Is.EqualTo(PcapLegacyLines.Line4.TimeStamp));
                Assert.That(lines[3].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line4.DeviceTimeStamp));
                Assert.That(lines[3].Text, Is.EqualTo(PcapLegacyLines.Line4.Text));
                Assert.That(lines[3].Count, Is.EqualTo(PcapLegacyLines.Line4.Count));
                Assert.That(lines[3].Line, Is.EqualTo(3));
                Assert.That(lines[3].Position, Is.EqualTo(317));

                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.True);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Microseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));

                    DateTime expected = DateTime.UnixEpoch.AddNanoSeconds(100000);
                    Assert.That(legacy.Format.GetTimeStamp(0, 100), Is.EqualTo(expected));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        /// <summary>
        /// Decodes two frames, with a "large" frame in between.
        /// </summary>
        /// <remarks>
        /// If we see a valid PCAP frame that is bigger than 2048 bytes, we don't decode it, but "skip" it. This is
        /// because it's bigger than an Ethernet MTU (1500 bytes).
        /// </remarks>
        [TestCaseSource(nameof(ReadChunks))]
        public void DecodeWithDiscard(int chunk)
        {
            // Must be larger than the largest packet the packet decoder expects.
            const int BigFrame = 70000;

            byte[] header = new byte[] {
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00
            };

            byte[] pkt1 = new byte[] {
                0x64, 0xFE, 0xF5, 0x5E, 0xFD, 0x3C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00,
                0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                0x00, 0x92, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x2A, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x7E, 0x69, 0x15, 0x3D, 0x0B, 0x00, 0x3B, 0x45, 0x43,
                0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,
                0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69,
                0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x3D, 0x0C, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03,
                0x8E, 0x00, 0x01, 0x54, 0x4B, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00,
                0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E,
                0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x20, 0x32, 0x00,
            };

            byte[] pkt2 = new byte[] {
                0x66, 0xFE, 0xF5, 0x5E, 0xFD, 0x4C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00,
                0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00,
                0x00, 0x92, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x2A, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF,
                0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x7E, 0x69, 0x15, 0x3D, 0x0D, 0x00, 0x3B, 0x45, 0x43,
                0x55, 0x31, 0x00, 0x00, 0x03, 0x8E, 0x00, 0x01, 0x54, 0x4A, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31,
                0x43, 0x54, 0x58, 0x31, 0x00, 0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72,
                0x67, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69,
                0x6E, 0x67, 0x2E, 0x2E, 0x00, 0x3D, 0x0E, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03,
                0x8E, 0x00, 0x01, 0x54, 0x4B, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00,
                0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E,
                0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x20, 0x32, 0x00
            };

            byte[] bigPktHdr = new byte[] {
                0x65, 0xFE, 0xF5, 0x5E, 0xFD, 0x5C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00
            };
            BitOperations.Copy32ShiftLittleEndian(BigFrame, bigPktHdr, 8);
            BitOperations.Copy32ShiftLittleEndian(BigFrame, bigPktHdr, 12);

            int packetLength = header.Length + pkt1.Length + bigPktHdr.Length + BigFrame + pkt2.Length;
            byte[] packet = new byte[packetLength];
            header.CopyTo(packet, 0);
            pkt1.CopyTo(packet, header.Length);
            bigPktHdr.CopyTo(packet, header.Length + pkt1.Length);
            pkt2.CopyTo(packet, header.Length + pkt1.Length + bigPktHdr.Length + BigFrame);

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, packet, chunk);

                Assert.That(lines, Has.Count.EqualTo(4));
                Assert.That(lines[0].TimeStamp, Is.EqualTo(PcapLegacyLines.Line1.TimeStamp));
                Assert.That(lines[0].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line1.DeviceTimeStamp));
                Assert.That(lines[0].Text, Is.EqualTo(PcapLegacyLines.Line1.Text));
                Assert.That(lines[0].Count, Is.EqualTo(PcapLegacyLines.Line1.Count));
                Assert.That(lines[0].Line, Is.EqualTo(0));
                Assert.That(lines[0].Position, Is.EqualTo(82));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(PcapLegacyLines.Line2.TimeStamp));
                Assert.That(lines[1].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line2.DeviceTimeStamp));
                Assert.That(lines[1].Text, Is.EqualTo(PcapLegacyLines.Line2.Text));
                Assert.That(lines[1].Count, Is.EqualTo(PcapLegacyLines.Line2.Count));
                Assert.That(lines[1].Line, Is.EqualTo(1));
                Assert.That(lines[1].Position, Is.EqualTo(141));
                Assert.That(lines[2].TimeStamp, Is.EqualTo(PcapLegacyLines.Line3.TimeStamp));
                Assert.That(lines[2].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line3.DeviceTimeStamp));
                Assert.That(lines[2].Text, Is.EqualTo(PcapLegacyLines.Line3.Text));
                Assert.That(lines[2].Count, Is.EqualTo(PcapLegacyLines.Line3.Count));
                Assert.That(lines[2].Line, Is.EqualTo(2));
                Assert.That(lines[2].Position, Is.EqualTo(258 + bigPktHdr.Length + BigFrame));
                Assert.That(lines[3].TimeStamp, Is.EqualTo(PcapLegacyLines.Line4.TimeStamp));
                Assert.That(lines[3].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line4.DeviceTimeStamp));
                Assert.That(lines[3].Text, Is.EqualTo(PcapLegacyLines.Line4.Text));
                Assert.That(lines[3].Count, Is.EqualTo(PcapLegacyLines.Line4.Count));
                Assert.That(lines[3].Line, Is.EqualTo(3));
                Assert.That(lines[3].Position, Is.EqualTo(317 + bigPktHdr.Length + BigFrame));

                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.True);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Microseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));

                    DateTime expected = DateTime.UnixEpoch.AddNanoSeconds(100000);
                    Assert.That(legacy.Format.GetTimeStamp(0, 100), Is.EqualTo(expected));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        [Test]
        public void FlushBeforeDecode()
        {
            using (TDec decoder = Create()) {
                var lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }

        /// <summary>
        /// Decodes two DLT packets in one Ethernet Frame. Checks the PCAP time stamp matches.
        /// </summary>
        [TestCaseSource(nameof(ReadChunks))]
        public void DecodePacketSkippedLine(int chunk)
        {
            byte[] packet = new byte[] {
                // PCAP Header
                0xD4, 0xC3, 0xB2, 0xA1, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //   0 ..  15
                0x00, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00,                                                 //  16 ..  23

                // Packet
                0x64, 0xFE, 0xF5, 0x5E, 0xFD, 0x3C, 0x0C, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00, //  24 ..  39
                0x10, 0xDF, 0x23, 0x41, 0xE4, 0xC2, 0x74, 0xE7, 0xB1, 0x14, 0x44, 0x5E, 0x08, 0x00, 0x45, 0x00, //  40 ..  55
                0x00, 0x92, 0x3A, 0x25, 0x00, 0x00, 0x01, 0x11, 0xA3, 0x2A, 0xC0, 0xA8, 0x01, 0x01, 0xEF, 0xFF, //  56 ..  71
                0x2A, 0x63, 0x0D, 0xA2, 0x0D, 0xA2, 0x00, 0x7E, 0x69, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //  72 ..  87
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //  88 .. 103
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 104 .. 119
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 120 .. 135
                0x00, 0x00, 0x00, 0x00, 0x00, 0x3D, 0x0C, 0x00, 0x3B, 0x45, 0x43, 0x55, 0x31, 0x00, 0x00, 0x03, // 136 .. 151
                0x8E, 0x00, 0x01, 0x54, 0x4B, 0x41, 0x01, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31, 0x00, // 152 .. 167
                0x02, 0x00, 0x00, 0x1B, 0x00, 0x44, 0x4C, 0x54, 0x20, 0x41, 0x72, 0x67, 0x75, 0x6D, 0x65, 0x6E, // 168 .. 183
                0x74, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x20, 0x32, 0x00  // 184 .. 199
            };

            using (TDec decoder = Create()) {
                var lines = Decode(decoder, packet, chunk);

                Assert.That(lines, Has.Count.EqualTo(2));
                // Skipped line
                Assert.That(lines[0].TimeStamp, Is.EqualTo(PcapLegacyLines.Line1.TimeStamp));
                Assert.That(lines[0].DeviceTimeStamp.Ticks, Is.EqualTo(0));
                Assert.That(lines[0].Text, Is.EqualTo("Skipped: 59 bytes; Reason: Invalid packet standard header"));
                Assert.That(lines[0].Count, Is.EqualTo(-1));
                Assert.That(lines[0].Line, Is.EqualTo(0));
                Assert.That(lines[0].Position, Is.EqualTo(82));
                Assert.That(lines[1].TimeStamp, Is.EqualTo(PcapLegacyLines.Line2.TimeStamp));
                Assert.That(lines[1].DeviceTimeStamp, Is.EqualTo(PcapLegacyLines.Line2.DeviceTimeStamp));
                Assert.That(lines[1].Text, Is.EqualTo(PcapLegacyLines.Line2.Text));
                Assert.That(lines[1].Count, Is.EqualTo(PcapLegacyLines.Line2.Count));
                Assert.That(lines[1].Line, Is.EqualTo(1));
                Assert.That(lines[1].Position, Is.EqualTo(141));

                if (decoder is DltPcapLegacyDecoder legacy) {
                    Assert.That(legacy.Format, Is.Not.Null);
                    Assert.That(legacy.Format.IsLittleEndian, Is.True);
                    Assert.That(legacy.Format.Resolution, Is.EqualTo(PcapFormat.TimeResolution.Microseconds));
                    Assert.That(legacy.Format.MajorVersion, Is.EqualTo(2));
                    Assert.That(legacy.Format.MinorVersion, Is.EqualTo(4));
                    Assert.That(legacy.Format.SnapLen, Is.EqualTo(0x40000));
                    Assert.That(legacy.Format.FcsLen, Is.EqualTo(0));
                    Assert.That(legacy.Format.LinkType, Is.EqualTo(LinkTypes.LINKTYPE_ETHERNET));

                    DateTime expected = DateTime.UnixEpoch.AddNanoSeconds(100000);
                    Assert.That(legacy.Format.GetTimeStamp(0, 100), Is.EqualTo(expected));
                }

                lines = Flush(decoder);
                Assert.That(lines, Is.Empty);
            }
        }
    }
}
