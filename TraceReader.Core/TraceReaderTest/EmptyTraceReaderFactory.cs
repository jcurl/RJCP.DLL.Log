namespace RJCP.Diagnostics.Log
{
    using Decoder;

    public class EmptyTracedReaderFactory : TraceReaderFactory<ITraceLine>
    {
        protected override ITraceDecoder<ITraceLine> GetDecoder()
        {
            return new EmptyTraceDecoder();
        }
    }
}
