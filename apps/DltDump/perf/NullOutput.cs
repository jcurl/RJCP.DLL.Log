namespace RJCP.App.DltDump
{
    using System;
    using Domain;
    using Infrastructure.Dlt;
    using RJCP.Diagnostics.Log.Dlt;

    public sealed class NullOutput : IOutputStream
    {
        public bool SupportsBinary { get { return false; } }

        public void SetInput(string fileName, InputFormat inputFormat)
        {
            /* Nothing to do */
        }

        public bool Write(DltTraceLineBase line)
        {
            /* Empty Write */
            return true;
        }

        public bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            /* Empty Write */
            return true;
        }

        public void Dispose()
        {
            /* Nothing to dispose */
        }
    }
}
