namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Globalization;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class Float64DltArgTest
    {
        [TestCase(.00000000000000001, "1e-17")]
        [TestCase(0, "0")]
        [TestCase(double.NaN, "nan")]
        [TestCase(double.NegativeInfinity, "-inf")]
        [TestCase(double.PositiveInfinity, "inf")]
        public void FloatToString(double value, string output)
        {
            Float64DltArg floatArg = new(value);
            Assert.That(floatArg.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(floatArg.Append(sb).ToString(), Is.EqualTo(output));
        }

        [Test]
        public void FloatToStringLarge()
        {
            double number = -4.42330604244772E-305;
            string output = string.Format(CultureInfo.InvariantCulture, "{0:0.00000e-000}", number);

            Float64DltArg floatArg = new(number);
            Assert.That(floatArg.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(floatArg.Append(sb).ToString(), Is.EqualTo(output));
        }

        [TestCase(new byte[] { 0xEC, 0x34, 0xD2, 0x52, 0x79, 0x83, 0x44, 0x40 }, "41.0271")]
        [TestCase(new byte[] { 0xD0, 0xE5, 0xE0, 0xAC, 0x9B, 0x84, 0x52, 0xC0 }, "-74.072")]
        public void Float_LargeNumber(byte[] array, string output)
        {
            if (!BitConverter.IsLittleEndian) Array.Reverse(array);
            double number = BitConverter.ToDouble(array, 0);

            Float64DltArg floatArg = new(number);
            Assert.That(floatArg.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(floatArg.Append(sb).ToString(), Is.EqualTo(output));
        }
    }
}
