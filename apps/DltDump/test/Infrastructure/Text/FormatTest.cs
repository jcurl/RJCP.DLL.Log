namespace RJCP.App.DltDump.Infrastructure.Text
{
    using NUnit.Framework;

    [TestFixture]
    public class FormatTest
    {
        [Test]
        public void FormatOneLine()
        {
            const string message = "This is a single line.";

            string[] lines = Format.Wrap(80, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo("This is a single line."));
        }

        [Test]
        public void FormatLongLine()
        {
            const string message = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(4));
            Assert.That(lines[0], Is.EqualTo("Sed ut perspiciatis unde omnis iste"));
            Assert.That(lines[1], Is.EqualTo("natus error sit voluptatem accusantium"));
            Assert.That(lines[2], Is.EqualTo("doloremque laudantium, totam rem"));
            Assert.That(lines[3], Is.EqualTo("aperiam, eaque ipsa"));
        }

        [Test]
        public void FormatLongLineMultipleSpaces()
        {
            // Spaces will be compressed.
            const string message = "Sed   ut    perspiciatis  unde  omnis  iste  natus  error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(4));
            Assert.That(lines[0], Is.EqualTo("Sed ut perspiciatis unde omnis iste"));
            Assert.That(lines[1], Is.EqualTo("natus error sit voluptatem accusantium"));
            Assert.That(lines[2], Is.EqualTo("doloremque laudantium, totam rem"));
            Assert.That(lines[3], Is.EqualTo("aperiam, eaque ipsa"));
        }

        [Test]
        public void FormatLongLineExactBoundary()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg hhhhh iiiii jjjjj";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("aaaaa bbbbb ccccc ddddd eeeee fffff gggg"));
            Assert.That(lines[1], Is.EqualTo("hhhhh iiiii jjjjj"));
        }

        [Test]
        public void FormatSingleLineExactBoundary()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo("aaaaa bbbbb ccccc ddddd eeeee fffff gggg"));
        }

        [Test]
        public void FormatSinelLineExactBoundaryWithSpace()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg ";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo("aaaaa bbbbb ccccc ddddd eeeee fffff gggg"));
        }

        [Test]
        public void FormatSinelLineExactBoundaryWithNewLine()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg\n";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo("aaaaa bbbbb ccccc ddddd eeeee fffff gggg"));
        }

        [Test]
        public void FormatSinelLineExactBoundaryWithSpaceAndNewLine()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg \n";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo("aaaaa bbbbb ccccc ddddd eeeee fffff gggg"));
        }

        [Test]
        public void FormatLongLineNoBreak()
        {
            const string message = "abcdefghijklmnopqrstuvwxyz 1234567890";

            string[] lines = Format.Wrap(20, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("abcdefghijklmnopqrstuvwxyz"));
            Assert.That(lines[1], Is.EqualTo("1234567890"));
        }

        [Test]
        public void FormatLongLineNoBreak2()
        {
            const string message = "1234567890 abcdefghijklmnopqrstuvwxyz";

            string[] lines = Format.Wrap(20, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("1234567890"));
            Assert.That(lines[1], Is.EqualTo("abcdefghijklmnopqrstuvwxyz"));
        }

        [Test]
        public void FormatWithNewLineUnix()
        {
            const string message = "aaaaa\nbbbbb";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("aaaaa"));
            Assert.That(lines[1], Is.EqualTo("bbbbb"));
        }

        [Test]
        public void FormatWithNewLineWindows()
        {
            const string message = "aaaaa\r\nbbbbb";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("aaaaa"));
            Assert.That(lines[1], Is.EqualTo("bbbbb"));
        }

        [Test]
        public void FormatWithNewLineMac()
        {
            const string message = "aaaaa\rbbbbb";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("aaaaa"));
            Assert.That(lines[1], Is.EqualTo("bbbbb"));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void FormatNewLineOnlyUnix(string message)
        {
            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo(string.Empty));
        }

        [Test]
        public void FormatMultipleNewLine()
        {
            const string message = "\n\n";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo(string.Empty));
            Assert.That(lines[1], Is.EqualTo(string.Empty));
        }

        [Test]
        public void FormatWithMultipleNewLine()
        {
            const string message = "aaaaa\r\rbbbbb";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(3));
            Assert.That(lines[0], Is.EqualTo("aaaaa"));
            Assert.That(lines[1], Is.EqualTo(string.Empty));
            Assert.That(lines[2], Is.EqualTo("bbbbb"));
        }

        [Test]
        public void FormatWithMultipleNewLineAndSpace()
        {
            const string message = "aaaaa\r\r bbbbb";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(3));
            Assert.That(lines[0], Is.EqualTo("aaaaa"));
            Assert.That(lines[1], Is.EqualTo(string.Empty));
            Assert.That(lines[2], Is.EqualTo("bbbbb"));
        }

        [Test]
        public void FormatWithLeadingSpace()
        {
            const string message = "  aaaaa\r\r bbbbb";

            string[] lines = Format.Wrap(40, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(3));
            Assert.That(lines[0], Is.EqualTo("aaaaa"));
            Assert.That(lines[1], Is.EqualTo(string.Empty));
            Assert.That(lines[2], Is.EqualTo("bbbbb"));
        }

        [Test]
        public void FormatOneLineIndent()
        {
            const string message = "This is a single line.";

            string[] lines = Format.Wrap(80, 10, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo("          This is a single line."));
        }

        [Test]
        public void FormatLongLineIndent()
        {
            const string message = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa";

            string[] lines = Format.Wrap(40, 5, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(4));
            Assert.That(lines[0], Is.EqualTo("     Sed ut perspiciatis unde omnis iste"));
            Assert.That(lines[1], Is.EqualTo("     natus error sit voluptatem"));
            Assert.That(lines[2], Is.EqualTo("     accusantium doloremque laudantium,"));
            Assert.That(lines[3], Is.EqualTo("     totam rem aperiam, eaque ipsa"));
        }

        [Test]
        public void FormatLongLineHangingIndent()
        {
            const string message = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa";

            string[] lines = Format.Wrap(40, 5, 2, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(4));
            Assert.That(lines[0], Is.EqualTo("     Sed ut perspiciatis unde omnis iste"));
            Assert.That(lines[1], Is.EqualTo("  natus error sit voluptatem accusantium"));
            Assert.That(lines[2], Is.EqualTo("  doloremque laudantium, totam rem"));
            Assert.That(lines[3], Is.EqualTo("  aperiam, eaque ipsa"));
        }

        [Test]
        public void FormatLongLineMultipleSpacesIndent()
        {
            // Spaces will be compressed.
            const string message = "Sed   ut    perspiciatis  unde  omnis  iste  natus  error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa";

            string[] lines = Format.Wrap(40, 5, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(4));
            Assert.That(lines[0], Is.EqualTo("     Sed ut perspiciatis unde omnis iste"));
            Assert.That(lines[1], Is.EqualTo("     natus error sit voluptatem"));
            Assert.That(lines[2], Is.EqualTo("     accusantium doloremque laudantium,"));
            Assert.That(lines[3], Is.EqualTo("     totam rem aperiam, eaque ipsa"));
        }

        [Test]
        public void FormatLongLineMultipleSpacesHangingIndent()
        {
            const string message = "Sed   ut    perspiciatis  unde  omnis  iste  natus  error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa";

            string[] lines = Format.Wrap(40, 5, 2, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(4));
            Assert.That(lines[0], Is.EqualTo("     Sed ut perspiciatis unde omnis iste"));
            Assert.That(lines[1], Is.EqualTo("  natus error sit voluptatem accusantium"));
            Assert.That(lines[2], Is.EqualTo("  doloremque laudantium, totam rem"));
            Assert.That(lines[3], Is.EqualTo("  aperiam, eaque ipsa"));
        }

        [Test]
        public void FormatLongLineExactBoundaryIndent()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg hhhhh iiiii jjjjj";

            string[] lines = Format.Wrap(40, 5, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa bbbbb ccccc ddddd eeeee fffff"));
            Assert.That(lines[1], Is.EqualTo("     gggg hhhhh iiiii jjjjj"));
        }

        [Test]
        public void FormatLongLineExactBoundaryHangingIndent()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee fffff gggg hhhhh iiiii jjjjj kkkkk lllll mmm";

            string[] lines = Format.Wrap(40, 5, 2, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa bbbbb ccccc ddddd eeeee fffff"));
            Assert.That(lines[1], Is.EqualTo("  gggg hhhhh iiiii jjjjj kkkkk lllll mmm"));
        }

        [Test]
        public void FormatLongLineExactBoundaryHangingIndent2()
        {
            const string message = "aaaaa bbbbb ccccc ddddd eeeee    fffff gggg hhhhh iiiii jjjjj kkkkk lllll";

            string[] lines = Format.Wrap(40, 5, 6, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa bbbbb ccccc ddddd eeeee fffff"));
            Assert.That(lines[1], Is.EqualTo("      gggg hhhhh iiiii jjjjj kkkkk lllll"));
        }

        [Test]
        public void FormatWithNewLineUnixIndent()
        {
            const string message = "aaaaa\nbbbbb";

            string[] lines = Format.Wrap(40, 5, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa"));
            Assert.That(lines[1], Is.EqualTo("     bbbbb"));
        }

        [Test]
        public void FormatWithNewLineWindowsIndent()
        {
            const string message = "aaaaa\r\nbbbbb";

            string[] lines = Format.Wrap(40, 5, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa"));
            Assert.That(lines[1], Is.EqualTo("     bbbbb"));
        }

        [Test]
        public void FormatWithNewLineMacIndent()
        {
            const string message = "aaaaa\rbbbbb";

            string[] lines = Format.Wrap(40, 5, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa"));
            Assert.That(lines[1], Is.EqualTo("     bbbbb"));
        }

        [Test]
        public void FormatWithNewLineUnixHangingIndent()
        {
            const string message = "aaaaa\nbbbbb";

            string[] lines = Format.Wrap(40, 5, 2, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa"));
            Assert.That(lines[1], Is.EqualTo("     bbbbb"));
        }

        [Test]
        public void FormatWithNewLineWindowsHangingIndent()
        {
            const string message = "aaaaa\r\nbbbbb";

            string[] lines = Format.Wrap(40, 5, 2, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa"));
            Assert.That(lines[1], Is.EqualTo("     bbbbb"));
        }

        [Test]
        public void FormatWithNewLineMacHangingIndent()
        {
            const string message = "aaaaa\rbbbbb";

            string[] lines = Format.Wrap(40, 5, 2, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("     aaaaa"));
            Assert.That(lines[1], Is.EqualTo("     bbbbb"));
        }

        [Test]
        public void FormatBulletPointsLongerMessage()
        {
            const string message =
                "* This is an example of bullet points, that can be formatted with hanging indents;\n" +
                "* And then the second bullet point; followed by\n" +
                "* The final longer line, which should look like a nice markdown bullet point list.\n";

            string[] lines = Format.Wrap(40, 2, 4, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(8));
            Assert.That(lines[0], Is.EqualTo("  * This is an example of bullet points,"));
            Assert.That(lines[1], Is.EqualTo("    that can be formatted with hanging"));
            Assert.That(lines[2], Is.EqualTo("    indents;"));
            Assert.That(lines[3], Is.EqualTo("  * And then the second bullet point;"));
            Assert.That(lines[4], Is.EqualTo("    followed by"));
            Assert.That(lines[5], Is.EqualTo("  * The final longer line, which should"));
            Assert.That(lines[6], Is.EqualTo("    look like a nice markdown bullet"));
            Assert.That(lines[7], Is.EqualTo("    point list."));
        }

        [Test]
        public void FormatNewLineWithHangingIndent()
        {
            const string message = "Line1\n Line2";

            string[] lines = Format.Wrap(40, 2, 4, message);
            Assert.That(lines, Is.Not.Null);
            Assert.That(lines, Has.Length.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("  Line1"));
            Assert.That(lines[1], Is.EqualTo("    Line2"));
        }
    }
}
