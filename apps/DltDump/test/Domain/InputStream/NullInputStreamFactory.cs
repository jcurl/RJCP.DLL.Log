namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public class NullInputStreamFactory : InputStreamFactoryBase
    {
        public override IInputStream Create(Uri uri)
        {
            return new NullInputStream();
        }
    }
}
