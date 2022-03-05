namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public class InvalidStreamFactory : InputStreamFactoryBase
    {
        public override IInputStream Create(Uri uri)
        {
            return null;
        }
    }
}
