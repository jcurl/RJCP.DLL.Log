namespace RJCP.App.DltDump.Infrastructure.Constraints
{
    using System;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class DltNotBeforeDateTest
    {
        [Test]
        public void NotBeforeMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetTimeStamp(new DateTime(2022, 06, 18, 10, 10, 0, DateTimeKind.Utc))
                .GetResult();
            Constraint c = new Constraint().Expr(new DltNotBeforeDate(new DateTime(2022, 6, 18, 10, 0, 5, DateTimeKind.Utc)));
            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void NotBeforeExactMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetTimeStamp(new DateTime(2022, 06, 18, 10, 0, 5, DateTimeKind.Utc))
                .GetResult();
            Constraint c = new Constraint().Expr(new DltNotBeforeDate(new DateTime(2022, 6, 18, 10, 0, 5, DateTimeKind.Utc)));
            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void NotBeforeNoMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetTimeStamp(new DateTime(2022, 06, 18, 9, 55, 59, DateTimeKind.Utc))
                .GetResult();
            Constraint c = new Constraint().Expr(new DltNotBeforeDate(new DateTime(2022, 6, 18, 10, 0, 5, DateTimeKind.Utc)));
            Assert.That(c.Check(line), Is.False);
        }
    }
}
