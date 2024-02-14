namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Globalization;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class Float32DltArgTest
    {
        [TestCase(0f, "0")]
        [TestCase(float.NaN, "nan")]
        [TestCase(float.NegativeInfinity, "-inf")]
        [TestCase(float.PositiveInfinity, "inf")]
        public void FloatToString(float value, string output)
        {
            Float32DltArg floatArg = new(value);
            Assert.That(floatArg.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(floatArg.Append(sb).ToString(), Is.EqualTo(output));
        }

        [TestCase(-10.123f)]
        [TestCase(23.7650f)]
        public void FloatWithSeparator(float number)
        {
            string output = string.Format(CultureInfo.InvariantCulture, "{0:0.000}", number);
            FloatToString(number, output);
        }
    }
}
