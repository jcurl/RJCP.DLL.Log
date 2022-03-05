namespace RJCP.App.DltDump.Domain
{
    using InputStream;

    public class TestInputStreamFactory : InputStreamFactory
    {
        public TestInputStreamFactory()
        {
            SetFactory("null", new NullInputStreamFactory());
        }
    }
}
