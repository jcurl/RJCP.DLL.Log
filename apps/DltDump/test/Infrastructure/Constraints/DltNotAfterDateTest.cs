namespace RJCP.App.DltDump.Infrastructure.Constraints
{
    using System;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;

    [TestFixture]
    public class DltNotAfterDateTest
    {
        [Test]
        public void NotAfterMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetTimeStamp(new DateTime(2022, 06, 18, 10, 0, 0, DateTimeKind.Utc))
                .GetResult();
            Constraint c = new Constraint().Expr(new DltNotAfterDate(new DateTime(2022, 6, 18, 10, 0, 5, DateTimeKind.Utc)));
            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void NotAfterExactMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetTimeStamp(new DateTime(2022, 06, 18, 10, 0, 5, DateTimeKind.Utc))
                .GetResult();
            Constraint c = new Constraint().Expr(new DltNotAfterDate(new DateTime(2022, 6, 18, 10, 0, 5, DateTimeKind.Utc)));
            Assert.That(c.Check(line), Is.True);
        }

        [Test]
        public void NotAfterNoMatch()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            DltTraceLineBase line = builder
                .SetEcuId("ECUX")
                .SetApplicationId("APP1")
                .SetContextId("CTX1")
                .SetTimeStamp(new DateTime(2022, 06, 18, 10, 0, 6, DateTimeKind.Utc))
                .GetResult();
            Constraint c = new Constraint().Expr(new DltNotAfterDate(new DateTime(2022, 6, 18, 10, 0, 5, DateTimeKind.Utc)));
            Assert.That(c.Check(line), Is.False);
        }
    }
}
