namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(UnknownVerboseArgEncoder))]
    [TestFixture(typeof(DltArgEncoder))]
    public class UnknownVerboseArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        [TestCase(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x01 }, false, TestName = "Encode_LittleEndian_UnknownVerbose")]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x10, 0x01 }, true, TestName = "Encode_BigEndian_UnknownVerbose")]
        public void EncodeUnknownVerbose(byte[] value, bool msbf)
        {
            byte[] buffer = new byte[value.Length];
            UnknownVerboseDltArg arg = new UnknownVerboseDltArg(value, msbf);
            IArgEncoder encoder = GetEncoder();
            Assert.That(encoder.Encode(buffer, true, msbf, arg), Is.EqualTo(buffer.Length));

            Assert.That(buffer, Is.EqualTo(value));
        }

        [TestCase(false, TestName = "Encode_LittleEndian_UnknownVerboseMax")]
        [TestCase(true, TestName = "Encode_BigEndian_UnknownVerboseMax")]
        public void EncodeUnknownVerboseMax(bool msbf)
        {
            byte[] value = new byte[65535];
            if (msbf) {
                value[0] = 0x00;
                value[1] = 0x00;
                value[2] = 0x04;
                value[3] = 0x00;
            } else {
                value[0] = 0x00;
                value[1] = 0x04;
                value[2] = 0x00;
                value[3] = 0x00;
            }
            Random rnd = new Random();
            rnd.NextBytes(value.AsSpan(4));
            EncodeUnknownVerbose(value, msbf);
        }
    }
}
