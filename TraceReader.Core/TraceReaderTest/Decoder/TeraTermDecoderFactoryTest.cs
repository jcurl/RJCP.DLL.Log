namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class TeraTermDecoderFactoryTest
    {
        [Test]
        public void DefaultEncoding()
        {
            TeraTermDecoderFactory factory = new TeraTermDecoderFactory();
            Encoding encoding = factory.Encoding;

            Assert.That(encoding.BodyName, Is.EqualTo("utf-8"));
        }

        [Test]
        public void SetNullEncoding()
        {
            TeraTermDecoderFactory factory = new TeraTermDecoderFactory();
            Assert.That(() => {
                factory.Encoding = null;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsciiEncoding()
        {
            TeraTermDecoderFactory factory = new TeraTermDecoderFactory {
                Encoding = Encoding.GetEncoding("ASCII")
            };
            Assert.That(factory.Encoding.BodyName, Is.EqualTo("us-ascii"));
        }

        [Test]
        public void CreateNoDefaultEncoding()
        {
            TeraTermDecoderFactory factory = new TeraTermDecoderFactory {
                Encoding = Encoding.GetEncoding("ASCII")
            };

            ITraceDecoder<LogTraceLine> decoder = factory.Create();
            Assert.That(decoder, Is.TypeOf<TeraTermDecoder>());

            TeraTermDecoder ttDecoder = (TeraTermDecoder)decoder;
            Assert.That(ttDecoder.Encoding.BodyName, Is.EqualTo("us-ascii"));
        }
    }
}
