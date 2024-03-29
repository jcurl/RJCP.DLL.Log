﻿namespace RJCP.App.DltDump.Domain.Dlt
{
    /// <summary>
    /// Defines the format for reading the input stream.
    /// </summary>
    public enum InputFormat
    {
        /// <summary>
        /// Try and determine the format automatically.
        /// </summary>
        Automatic,

        /// <summary>
        /// File input stream with a storage header.
        /// </summary>
        File,

        /// <summary>
        /// Serial input stream with a DLS\1 header.
        /// </summary>
        Serial,

        /// <summary>
        /// Network input stream starting with a standard header.
        /// </summary>
        Network,

        /// <summary>
        /// PCAP or PCAP-NG File.
        /// </summary>
        Pcap
    }
}
