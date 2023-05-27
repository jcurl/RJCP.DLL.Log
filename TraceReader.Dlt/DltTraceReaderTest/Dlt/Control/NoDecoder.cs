namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;
    using RJCP.Core;

    public sealed class NoDecoder : IControlArgDecoder
    {
        public Result<int> Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            service = null;
            return Result.FromException<int>(new DltDecodeException());
        }
    }
}
