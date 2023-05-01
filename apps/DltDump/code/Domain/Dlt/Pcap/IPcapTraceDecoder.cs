namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A PCAP Trace Decoder that allows setting time stamps from other sources.
    /// </summary>
    public interface IPcapTraceDecoder : ITraceDecoder<DltTraceLineBase>
    {
        /// <summary>
        /// Gets or sets the packet time stamp.
        /// </summary>
        /// <value>The packet time stamp.</value>
        DateTime PacketTimeStamp { get; set; }
    }
}
