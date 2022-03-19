namespace RJCP.Diagnostics.Log
{
    using System;
    using Dlt;
    using Dlt.Args;
    using NUnit.Framework;

    [TestFixture]
    public class DltTraceLineTest
    {
        [Test]
        public void DefaultTraceLine()
        {
            DltTraceLine line = new DltTraceLine();
            Assert.That(line.Arguments.Count, Is.EqualTo(0));
            Assert.That(line.ApplicationId, Is.Null);
            Assert.That(line.ContextId, Is.Null);
            Assert.That(line.EcuId, Is.Null);
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.Type, Is.EqualTo(DltType.UNKNOWN));
            Assert.That(line.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Text, Is.EqualTo(string.Empty));

            Assert.That(line.Features.ApplicationId, Is.False);
            Assert.That(line.Features.ContextId, Is.False);
            Assert.That(line.Features.EcuId, Is.False);
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
            Assert.That(line.Features.TimeStamp, Is.False);
            Assert.That(line.Features.MessageType, Is.False);
            Assert.That(line.Features.SessionId, Is.False);
        }

        [Test]
        public void TraceLineWithArgumentsArray()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(line.Arguments.Count, Is.EqualTo(1));
            Assert.That(line.Arguments[0], Is.TypeOf<StringDltArg>());
            Assert.That(line.ApplicationId, Is.Null);
            Assert.That(line.ContextId, Is.Null);
            Assert.That(line.EcuId, Is.Null);
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.Type, Is.EqualTo(DltType.UNKNOWN));
            Assert.That(line.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Text, Is.EqualTo("string"));

            Assert.That(line.Features.ApplicationId, Is.False);
            Assert.That(line.Features.ContextId, Is.False);
            Assert.That(line.Features.EcuId, Is.False);
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
            Assert.That(line.Features.TimeStamp, Is.False);
            Assert.That(line.Features.MessageType, Is.False);
            Assert.That(line.Features.SessionId, Is.False);

            // Ensure that the arguments is read only
            Assert.That(line.Arguments.IsReadOnly, Is.True);
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyAdd()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(() => {
                line.Arguments.Add(new HexIntDltArg(10, 1));
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyClear()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(() => {
                line.Arguments.Clear();
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyInsert()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(() => {
                line.Arguments.Insert(0, new BinaryIntDltArg(10, 1));
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyRemove()
        {
            IDltArg arg = new StringDltArg("string");
            DltTraceLine line = new DltTraceLine(new[] { arg });
            Assert.That(() => {
                line.Arguments.Remove(arg);
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyRemoveAt()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(() => {
                line.Arguments.RemoveAt(0);
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyCopyTo()
        {
            IDltArg arg = new StringDltArg("string");
            DltTraceLine line = new DltTraceLine(new[] { arg });

            IDltArg[] args = new IDltArg[1];
            line.Arguments.CopyTo(args, 0);
            Assert.That(args[0], Is.EqualTo(arg));
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyContainsFalse()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(line.Arguments.Contains(new BinaryIntDltArg(10, 1)), Is.False);
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyContainsTrue()
        {
            IDltArg arg1 = new StringDltArg("string");
            IDltArg arg2 = new BinaryIntDltArg(10, 1);
            DltTraceLine line = new DltTraceLine(new[] { arg1, arg2 });
            Assert.That(line.Arguments.Contains(new BinaryIntDltArg(10, 1)), Is.False);
            Assert.That(line.Arguments.Contains(arg1), Is.True);
            Assert.That(line.Arguments.Contains(arg2), Is.True);
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyIndexOf()
        {
            IDltArg arg1 = new StringDltArg("string");
            IDltArg arg2 = new BinaryIntDltArg(10, 1);
            DltTraceLine line = new DltTraceLine(new[] { arg1, arg2 });
            Assert.That(line.Arguments.IndexOf(new BinaryIntDltArg(10, 1)), Is.EqualTo(-1));
            Assert.That(line.Arguments.IndexOf(arg1), Is.EqualTo(0));
            Assert.That(line.Arguments.IndexOf(arg2), Is.EqualTo(1));
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyIndexGet()
        {
            IDltArg arg1 = new StringDltArg("string");
            IDltArg arg2 = new BinaryIntDltArg(10, 1);
            DltTraceLine line = new DltTraceLine(new[] { arg1, arg2 });
            Assert.That(line.Arguments[0], Is.EqualTo(arg1));
            Assert.That(line.Arguments[1], Is.EqualTo(arg2));
        }

        [Test]
        public void TraceLineWithArgumentsArrayReadOnlyIndexSet()
        {
            DltTraceLine line = new DltTraceLine(new IDltArg[] { new StringDltArg("string") });
            Assert.That(() => {
                line.Arguments[0] = new HexIntDltArg(10, 1);
            }, Throws.TypeOf<NotSupportedException>());
        }
    }
}
