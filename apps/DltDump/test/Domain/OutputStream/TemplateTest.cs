namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using System.Globalization;
    using NUnit.Framework;

    [TestFixture]
    public class TemplateTest
    {
        [Test]
        public void NullTemplate()
        {
            Assert.That(() => {
                _ = new Template(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyTemplate()
        {
            Template template = new(string.Empty);
            Assert.That(template.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NoSubstitution()
        {
            Template template = new("foo");
            Assert.That(template.ToString(), Is.EqualTo("foo"));
        }

        [Test]
        public void SelfEscape()
        {
            Template template = new("fo%%o");
            Assert.That(template.ToString(), Is.EqualTo("fo%o"));
        }

        [TestCase("%TEMPLATETEST%", "testVar")]
        [TestCase("_%TEMPLATETEST%", "_testVar")]
        [TestCase("%TEMPLATETEST%_var", "testVar_var")]
        public void EnvironmentSubstitution(string templateStr, string expected)
        {
            Environment.SetEnvironmentVariable("TEMPLATETEST", "testVar");

            Template template = new(templateStr);
            Assert.That(template.ToString(), Is.EqualTo(expected));
            Assert.That(template.ContainsVariable("TEMPLATETEST"), Is.True);
        }

        [TestCase("%INPUT%", "input")]
        [TestCase("_%INPUT%", "_input")]
        [TestCase("%INPUT%_var", "input_var")]
        public void VariableSubstitution(string templateStr, string expected)
        {
            Template template = new(templateStr);
            template.Variables["INPUT"] = "input";
            Assert.That(template.ToString(), Is.EqualTo(expected));
            Assert.That(template.ContainsVariable("INPUT"), Is.True);
        }

        [Test]
        public void VariableEnvOrder()
        {
            Environment.SetEnvironmentVariable("TEMPLATETEST", "envVar");

            Template template = new("%TEMPLATETEST%");
            template.Variables["TEMPLATETEST"] = "input";
            Assert.That(template.ToString(), Is.EqualTo("input"));
        }

        [Test]
        public void VariableEnvInString()
        {
            Environment.SetEnvironmentVariable("TEMPLATETEST", "envVar");

            Template template = new("%TEMPLATETEST%_%VARTEST%.txt");
            template.Variables["VARTEST"] = "input";
            Assert.That(template.ToString(), Is.EqualTo("envVar_input.txt"));
        }

        [Test]
        public void VariableSubstitutionUpdate()
        {
            Template template = new("%INPUT%.dlt");

            template.Variables["INPUT"] = "input";
            Assert.That(template.ToString(), Is.EqualTo("input.dlt"));

            template.Variables["INPUT"] = "var2";
            Assert.That(template.ToString(), Is.EqualTo("var2.dlt"));
        }

        [TestCase("%NONEXISTENTx%.dlt", ".dlt")]
        [TestCase("_%NONEXISTENTx%.dlt", "_.dlt")]
        [TestCase("_%NONEXISTENTx%", "_")]
        public void NonExistentEnvOrVar(string templateStr, string expected)
        {
            Template template = new(templateStr);
            Assert.That(template.ToString(), Is.EqualTo(expected));
        }

        [TestCase("%", "%", false)]
        [TestCase("%VAR", "%VAR", false)]
        [TestCase("VAR%", "VAR%", false)]
        [TestCase("%%VAR%", "%VAR%", false)]
        [TestCase("%VAR%%", "%", true)]
        public void IncompleteVar(string templateStr, string expected, bool containsVar)
        {
            Template template = new(templateStr);
            Assert.That(template.ToString(), Is.EqualTo(expected));
            Assert.That(template.ContainsVariable("VAR"), Is.EqualTo(containsVar));
        }

        [Test]
        public void ContainsNullVariable()
        {
            Template template = new("%FILE%.txt");
            template.Variables["FILE"] = "input";

            Assert.That(() => {
                template.ContainsVariable(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ContainsEmptyVariable()
        {
            Template template = new("%FILE%.txt");
            template.Variables["FILE"] = "input";

            Assert.That(() => {
                template.ContainsVariable(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConcatenationAllowed()
        {
            Template template = new("%FILE%.txt");
            template.Variables["FILE"] = "input";
            template.Variables["CTR"] = 1.ToString(CultureInfo.InvariantCulture);

            Assert.That(template.AllowConcatenation, Is.False);
            Assert.That(template.SupportsSplit, Is.False);
        }

        [Test]
        public void ConcatenationNotAllowed()
        {
            Template template = new("%CTR%.txt");
            template.Variables["FILE"] = "input";
            template.Variables["CTR"] = 1.ToString(CultureInfo.InvariantCulture);

            Assert.That(template.AllowConcatenation, Is.True);
        }

        [Test]
        public void SupportsSplitDate()
        {
            Template template = new("file_%CDATE%.txt");
            template.Variables["FILE"] = "input";
            template.Variables["CTR"] = 1.ToString(CultureInfo.InvariantCulture);

            Assert.That(template.AllowConcatenation, Is.True);
            Assert.That(template.SupportsSplit, Is.True);
        }

        [Test]
        public void SupportsSplitTime()
        {
            Template template = new("file_%CTIME%.txt");
            template.Variables["FILE"] = "input";
            template.Variables["CTR"] = 1.ToString(CultureInfo.InvariantCulture);

            Assert.That(template.AllowConcatenation, Is.True);
            Assert.That(template.SupportsSplit, Is.True);
        }

        [Test]
        public void SupportsSplitDateTime()
        {
            Template template = new("file_%CDATETIME%.txt");
            template.Variables["FILE"] = "input";
            template.Variables["CTR"] = 1.ToString(CultureInfo.InvariantCulture);

            Assert.That(template.AllowConcatenation, Is.True);
            Assert.That(template.SupportsSplit, Is.True);
        }

        [Test]
        public void SupportsSplitCounter()
        {
            Template template = new("file_%CTR%.txt");
            template.Variables["FILE"] = "input";
            template.Variables["CTR"] = 1.ToString(CultureInfo.InvariantCulture);

            Assert.That(template.AllowConcatenation, Is.True);
            Assert.That(template.SupportsSplit, Is.True);
        }
    }
}
