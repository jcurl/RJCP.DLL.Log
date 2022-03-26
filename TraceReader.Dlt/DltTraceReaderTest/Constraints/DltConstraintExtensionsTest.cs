namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;
    using Dlt.ControlArgs;
    using NUnit.Framework;

    [TestFixture(Category = "TraceReader.Constraints")]
    public class DltConstraintExtensionsTest
    {
        [TestCase("ECUX", true)]
        [TestCase("ECUx", false)]
        public void DltEcuIdMatch(string constraint, bool match)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .GetResult();
            Constraint c = new Constraint().DltEcuId(constraint);
            Assert.That(c.Check(line), Is.EqualTo(match));
        }

        [Test]
        public void DltEcuIdNotPresent()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetApplicationId("APP1")
                .GetResult();
            Constraint c = new Constraint().DltEcuId(string.Empty);
            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void DltEcuIdNull()
        {
            Assert.That(() => {
                _ = new Constraint().DltEcuId(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("CTX1", true)]
        [TestCase("CTX2", false)]
        public void DltCtxIdMatch(string constraint, bool match)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .GetResult();
            Constraint c = new Constraint().DltCtxId(constraint);
            Assert.That(c.Check(line), Is.EqualTo(match));
        }

        [Test]
        public void DltCtxIdNotPresent()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECU1")
                .GetResult();
            Constraint c = new Constraint().DltCtxId(string.Empty);
            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void DltCtxIdNull()
        {
            Assert.That(() => {
                _ = new Constraint().DltCtxId(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("APP1", true)]
        [TestCase("app2", false)]
        public void DltAppIdMatch(string constraint, bool match)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .GetResult();
            Constraint c = new Constraint().DltAppId(constraint);
            Assert.That(c.Check(line), Is.EqualTo(match));
        }

        [Test]
        public void DltAppIdNotPresent()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECU1")
                .GetResult();
            Constraint c = new Constraint().DltAppId(string.Empty);
            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void DltAppIdNull()
        {
            Assert.That(() => {
                _ = new Constraint().DltAppId(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(DltType.LOG_FATAL, true)]
        [TestCase(DltType.LOG_INFO, false)]
        [TestCase(DltType.APP_TRACE_FUNCTION_IN, false)]
        public void DltTypeMatch(DltType type, bool match)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_FATAL)
                .GetResult();
            Constraint c = new Constraint().DltType(type);
            Assert.That(c.Check(line), Is.EqualTo(match));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void DltIsVerbose(bool isVerbose, bool match)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_FATAL)
                .SetIsVerbose(true)
                .GetResult();
            Constraint c = new Constraint().DltIsVerbose(isVerbose);
            Assert.That(c.Check(line), Is.EqualTo(match));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void DltIsVerboseControl(bool isVerbose, bool match)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.CONTROL_RESPONSE)
                .SetControlPayload(new GetSoftwareVersionResponse(ControlResponse.StatusOk, "20w34.1"))
                .GetResult();
            Constraint c = new Constraint().DltIsVerbose(isVerbose);
            Assert.That(c.Check(line), Is.EqualTo(match));
        }

        [Test]
        public void DltIsControl()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.CONTROL_RESPONSE)
                .SetControlPayload(new GetSoftwareVersionResponse(ControlResponse.StatusOk, "20w34.1"))
                .GetResult();
            Constraint c = new Constraint().DltIsControl();
            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void DltIsControlFalse()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetDltType(DltType.LOG_FATAL)
                .SetIsVerbose(true)
                .GetResult();
            Constraint c = new Constraint().DltIsControl();
            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void DltSessionIdMatchZero()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetSessionId(0)
                .GetResult();
            Constraint c = new Constraint().DltSessionId(0);
            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void DltSessionIdNoMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetSessionId(10)
                .GetResult();
            Constraint c = new Constraint().DltSessionId(0);
            Assert.That(c.Check(line), Is.False);
        }

        [Test]
        public void DltSessionIdNotPresent()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetApplicationId("APP1")
                .GetResult();
            Constraint c = new Constraint().DltSessionId(0);
            Assert.That(c.Check(line), Is.False);
        }

        [TestCase(0, 100, false)]
        [TestCase(99, 100, false)]
        [TestCase(100, 100, true)]
        [TestCase(101, 100, true)]
        public void Awake(int alive, int match, bool result)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetApplicationId("APP1")
                .SetDeviceTimeStamp(alive * TimeSpan.TicksPerMillisecond)
                .GetResult();

            Constraint c = new Constraint().Awake(match);
            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [Test]
        public void AwakeNegative()
        {
            Assert.That(() => {
                _ = new Constraint().Awake(-1);
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0, 100, false)]
        [TestCase(99, 100, false)]
        [TestCase(100, 100, true)]
        [TestCase(101, 100, true)]
        public void AwakeTimeSpan(int alive, int match, bool result)
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetApplicationId("APP1")
                .SetDeviceTimeStamp(alive * TimeSpan.TicksPerMillisecond)
                .GetResult();

            Constraint c = new Constraint().Awake(new TimeSpan(match * TimeSpan.TicksPerMillisecond));
            Assert.That(c.Check(line), Is.EqualTo(result));
        }

        [Test]
        public void AwakeNegativeTimeStamp()
        {
            Assert.That(() => {
                _ = new Constraint().Awake(new TimeSpan(-10000));
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void AwakeNegativeTimeStampLarge()
        {
            Assert.That(() => {
                _ = new Constraint().Awake(new TimeSpan(int.MaxValue * TimeSpan.TicksPerMillisecond + 1));
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
