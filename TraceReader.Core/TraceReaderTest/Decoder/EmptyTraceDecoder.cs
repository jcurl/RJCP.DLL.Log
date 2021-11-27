namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;

    public sealed class EmptyTraceDecoder : ITraceDecoder<ITraceLine>
    {
        public IEnumerable<ITraceLine> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            return Array.Empty<TraceLine>();
        }

        public IEnumerable<ITraceLine> Flush()
        {
            return Array.Empty<TraceLine>();
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
