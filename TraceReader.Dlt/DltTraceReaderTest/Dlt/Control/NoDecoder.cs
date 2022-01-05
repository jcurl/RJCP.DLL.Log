namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using ControlArgs;

    public sealed class NoDecoder : IControlArgDecoder
    {
        public int Decode(int serviceId, ReadOnlySpan<byte> buffer, bool msbf, out IControlArg service)
        {
            service = null;
            return -1;
        }
    }
}
