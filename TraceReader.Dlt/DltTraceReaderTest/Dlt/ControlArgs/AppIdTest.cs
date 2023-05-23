namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class AppIdTest
    {
        [Test]
        public void AppIdName()
        {
            AppId appId = new AppId("name");
            Assert.That(appId.Name, Is.EqualTo("name"));
            Assert.That(appId.Description, Is.Empty);
            Assert.That(appId.ContextIds, Is.Empty);
            Assert.That(appId.ToString(), Is.EqualTo("name ()"));
        }

        [Test]
        public void AppIdEmptyName([Values(null, "")] string name)
        {
            AppId appId = new AppId(name);
            Assert.That(appId.Name, Is.EqualTo(string.Empty));
            Assert.That(appId.Description, Is.Empty);
            Assert.That(appId.ContextIds, Is.Empty);
            Assert.That(appId.ToString(), Is.EqualTo("()"));
        }

        [Test]
        public void AppIdDescription()
        {
            AppId appId = new AppId("name", "description");
            Assert.That(appId.Name, Is.EqualTo("name"));
            Assert.That(appId.Description, Is.EqualTo("description"));
            Assert.That(appId.ContextIds, Is.Empty);
            Assert.That(appId.ToString(), Is.EqualTo("name ()"));
        }

        [Test]
        public void AppIdEmptyDescription([Values(null, "")] string description)
        {
            AppId appId = new AppId("name", description);
            Assert.That(appId.Name, Is.EqualTo("name"));
            Assert.That(appId.Description, Is.Empty);
            Assert.That(appId.ContextIds, Is.Empty);
            Assert.That(appId.ToString(), Is.EqualTo("name ()"));
        }

        [Test]
        public void ContextIdName()
        {
            ContextId ctxId = new ContextId("ctx1");
            Assert.That(ctxId.Name, Is.EqualTo("ctx1"));
            Assert.That(ctxId.Description, Is.Empty);
            Assert.That(ctxId.LogLevel, Is.EqualTo(LogLevel.Block));
            Assert.That(ctxId.TraceStatus, Is.EqualTo(ContextId.StatusUndefined));
            Assert.That(ctxId.ToString(), Is.EqualTo("ctx1 Block -128"));
        }

        [Test]
        public void ContextIdEmptyName([Values(null, "")] string name)
        {
            ContextId ctxId = new ContextId(name);
            Assert.That(ctxId.Name, Is.Empty);
            Assert.That(ctxId.Description, Is.Empty);
            Assert.That(ctxId.LogLevel, Is.EqualTo(LogLevel.Block));
            Assert.That(ctxId.TraceStatus, Is.EqualTo(ContextId.StatusUndefined));
            Assert.That(ctxId.ToString(), Is.EqualTo(" Block -128"));
        }

        [Test]
        public void ContextIdDescription()
        {
            ContextId ctxId = new ContextId("ctx1", LogLevel.Info, ContextId.StatusOn, "description");
            Assert.That(ctxId.Name, Is.EqualTo("ctx1"));
            Assert.That(ctxId.Description, Is.EqualTo("description"));
            Assert.That(ctxId.LogLevel, Is.EqualTo(LogLevel.Info));
            Assert.That(ctxId.TraceStatus, Is.EqualTo(ContextId.StatusOn));
            Assert.That(ctxId.ToString(), Is.EqualTo("ctx1 Info 1"));
        }

        [Test]
        public void ContextIdEmptyDescription([Values(null, "")] string description)
        {
            ContextId ctxId = new ContextId("ctx1", LogLevel.Info, ContextId.StatusOn, description);
            Assert.That(ctxId.Name, Is.EqualTo("ctx1"));
            Assert.That(ctxId.Description, Is.Empty);
            Assert.That(ctxId.LogLevel, Is.EqualTo(LogLevel.Info));
            Assert.That(ctxId.TraceStatus, Is.EqualTo(ContextId.StatusOn));
            Assert.That(ctxId.ToString(), Is.EqualTo("ctx1 Info 1"));
        }

        [Test]
        public void FullList()
        {
            ContextId app1c1 = new ContextId("CTX1", LogLevel.Info, 1, "App1 Context 1");
            ContextId app1c2 = new ContextId("CTX2", LogLevel.Debug, 0, "App1 Context 2");
            AppId app1 = new AppId("APP1", "Application 1");
            app1.ContextIds.Add(app1c1);
            app1.ContextIds.Add(app1c2);

            Assert.That(app1.Name, Is.EqualTo("APP1"));
            Assert.That(app1.Description, Is.EqualTo("Application 1"));
            Assert.That(app1.ContextIds, Has.Count.EqualTo(2));
            Assert.That(app1.ContextIds[0].Name, Is.EqualTo("CTX1"));
            Assert.That(app1.ContextIds[0].Description, Is.EqualTo("App1 Context 1"));
            Assert.That(app1.ContextIds[0].LogLevel, Is.EqualTo(LogLevel.Info));
            Assert.That(app1.ContextIds[0].TraceStatus, Is.EqualTo(ContextId.StatusOn));
            Assert.That(app1.ContextIds[1].Name, Is.EqualTo("CTX2"));
            Assert.That(app1.ContextIds[1].Description, Is.EqualTo("App1 Context 2"));
            Assert.That(app1.ContextIds[1].LogLevel, Is.EqualTo(LogLevel.Debug));
            Assert.That(app1.ContextIds[1].TraceStatus, Is.EqualTo(ContextId.StatusOff));
            Assert.That(app1.ToString(), Is.EqualTo("APP1 (CTX1 info on, CTX2 debug off)"));
        }
    }
}
