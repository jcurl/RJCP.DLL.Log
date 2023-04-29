namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Text;
    using Args;
    using Control;
    using Decoder;
    using NUnit.Framework;

    #region Nee Decoder for S_FLOA16, extending existing decoders
    // To create your own handler for your own decoder, you should extend the existing classes and instantiate them.
    // * MyFloat16Arg: New implementation. This is the argument in the ITraceLineDlt message stored.
    // * MyFloat16ArgDecoder: Knows how to convert bytes in a stream into a MyFloat16Arg
    // * MyNonVerboseArgDecoder: Registers the new SIGNAL-REF in the base class
    // * MyDltTraceDecoder: Instantiates the NonVerboseDltDecoder with MyNonVerboseArgDecoder
    // * MyDltTraceDecoderFactory: Create the MyDeltTraceDecoder
    // * MyDltTraceReaderFactory: Creates the ITraceReader using MyDltTraceDecoderFactory

    public class MyFloat16Arg : DltArgBase<byte[]>
    {
        public MyFloat16Arg(byte[] data) : base(data) { }

        public override string ToString()
        {
            if (Data.Length == 0) return string.Empty;
            return string.Format("{0:x2}{1:x2}", Data[0], Data[1]);
        }

        public override StringBuilder Append(StringBuilder strBuilder)
        {
            if (Data.Length == 0) return strBuilder;
            return strBuilder.Append($"{Data[0]:x2}{Data[1]:x2}");
        }
    }

    public class MyFloat16ArgDecoder : INonVerboseArgDecoder
    {
        public int Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg)
        {
            if (buffer.Length < pdu.PduLength) {
                arg = new DltArgError("Insufficient payload buffer {0} for float16 argument", pdu.PduLength);
                return -1;
            }
            if (pdu.PduLength < 2) {
                arg = new DltArgError("S_FLOA16 invalid length in PDU");
                return -1;
            }

            arg = new MyFloat16Arg(buffer[0..2].ToArray());
            return pdu.PduLength;
        }
    }

    public class MyNonVerboseArgDecoder : NonVerboseArgDecoder
    {
        public MyNonVerboseArgDecoder()
        {
            Register("S_FLOA16", new MyFloat16ArgDecoder());
            Unregister("S_BIN32");
            Unregister("S_BIN64");
        }
    }

    public class MyDltTraceDecoder : DltTraceDecoder
    {
        private static INonVerboseDltDecoder GetMyNonVerboseDecoder(IFrameMap map)
        {
            return new NonVerboseDltDecoder(map, new MyNonVerboseArgDecoder());
        }

        public MyDltTraceDecoder(IFrameMap map)
            : base(GetVerboseDecoder(), GetMyNonVerboseDecoder(map), new ControlDltDecoder(), new DltLineBuilder()) { }
    }

    public class MyDltTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        private readonly IFrameMap m_Map;

        public MyDltTraceDecoderFactory(IFrameMap map)
        {
            m_Map = map;
        }

        public ITraceDecoder<DltTraceLineBase> Create()
        {
            return new MyDltTraceDecoder(m_Map);
        }
    }

    public class MyDltTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        public MyDltTraceReaderFactory(IFrameMap map) : base(new MyDltTraceDecoderFactory(map)) { }
    }
    #endregion

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class NonVerboseArgRegisterTest : NonVerboseDecoderTestBase<MyFloat16ArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(1, new TestPdu("S_FLOA16", 2));

        public NonVerboseArgRegisterTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override DltFactory CustomFactory
        {
            get { return new DltFactory(DltFactoryType.Standard, new MyDltTraceReaderFactory(Map)); }
        }

        protected override INonVerboseDltDecoder CustomDecoder
        {
            get { return new NonVerboseDltDecoder(Map, new MyNonVerboseArgDecoder()); }
        }

        [Test]
        public void RegisterFloa16()
        {
            byte[] payload = new byte[] { 0xAA, 0x55 };

            Decode(1, payload, "nv_RegisterFloat16", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<MyFloat16Arg>());

            MyFloat16Arg arg = (MyFloat16Arg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("aa55"));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0xAA, 0x55 }));
        }
    }

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class NonVerboseArgUnregisterDecoderTest : NonVerboseDecoderTestBase<NonVerboseUnknownArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(2, new TestPdu("S_BIN32", 4));

        public NonVerboseArgUnregisterDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override DltFactory CustomFactory
        {
            get { return new DltFactory(DltFactoryType.Standard, new MyDltTraceReaderFactory(Map)); }
        }

        protected override INonVerboseDltDecoder CustomDecoder
        {
            get { return new NonVerboseDltDecoder(Map, new MyNonVerboseArgDecoder()); }
        }

        [Test]
        public void UnregisterBin32()
        {
            byte[] payload = new byte[] { 0xAB, 0xCD, 0xEF, 0x01 };

            Decode(2, payload, "nv_Bin_32bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnknownNonVerboseDltArg>());

            UnknownNonVerboseDltArg arg = (UnknownNonVerboseDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("(04) ab cd ef 01"));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0xAB, 0xCD, 0xEF, 0x01 }));
        }
    }

    [TestFixture]
    public class NonVerboseArgDecoderTest
    {
        private class MyNonVerboseArgDecoderUnregisterNullPduType : NonVerboseArgDecoder
        {
            public MyNonVerboseArgDecoderUnregisterNullPduType()
            {
                Unregister(null);
            }
        }

        [Test]
        public void UnregisterNullPduType()
        {
            Assert.That(() => {
                _ = new MyNonVerboseArgDecoderUnregisterNullPduType();
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private class MyNonVerboseArgDecoderRegisterNullPduType : NonVerboseArgDecoder
        {
            public MyNonVerboseArgDecoderRegisterNullPduType()
            {
                Register(null, new MyFloat16ArgDecoder());
            }
        }

        [Test]
        public void RegisterNullPduType()
        {
            Assert.That(() => {
                _ = new MyNonVerboseArgDecoderRegisterNullPduType();
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private class MyNonVerboseArgDecoderRegisterNullPduDecoder : NonVerboseArgDecoder
        {
            public MyNonVerboseArgDecoderRegisterNullPduDecoder()
            {
                Register("S_FOO", null);
            }
        }

        [Test]
        public void RegisterNullPduArgDecoder()
        {
            Assert.That(() => {
                _ = new MyNonVerboseArgDecoderRegisterNullPduDecoder();
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
