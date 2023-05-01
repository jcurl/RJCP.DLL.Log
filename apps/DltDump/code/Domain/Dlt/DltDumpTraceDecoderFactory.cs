namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using RJCP.App.DltDump.Resources;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;

    /// <summary>
    /// A decoder factory for all DltDump modes of decoding based on input formats, online mode and output filters.
    /// </summary>
    public class DltDumpTraceDecoderFactory : ITraceDecoderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        public InputFormat InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the frame map used for decoding non-verbose messages.
        /// </summary>
        /// <value>The frame map used for decoding non-verbose messages.</value>
        public IFrameMap FrameMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if in online mode.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> this is in online mode, where time stamps are obtained by the local host, else
        /// <see langword="false"/>. Formats with a storage header ignore this field.
        /// </value>
        public bool OnlineMode { get; set; }

        /// <summary>
        /// Gets or sets the output stream to use when instantiating.
        /// </summary>
        /// <value>The output stream.</value>
        /// <remarks>
        /// When instantiating via <see cref="CreateAsync(string)"/>, the <see cref="IOutputStream.SupportsBinary"/> is
        /// used to determine if this object should be injected or not. If this object is <see langword="null"/>, then
        /// no <see cref="IOutputStream"/> is used.
        /// </remarks>
        public IOutputStream OutputStream { get; set; }

        /// <summary>
        /// Creates a new instance of an <see cref="ITraceDecoder{DltTraceLineBase}"/>.
        /// </summary>
        /// <returns>
        /// A new instance of an <see cref="ITraceDecoder{DltTraceLineBase}"/> object based on the input format, if time
        /// stamp information is in real time through the <see cref="OnlineMode"/> property, and if there are output
        /// stream filters.
        /// </returns>
        /// <exception cref="InvalidOperationException">The input format is unknown.</exception>
        public ITraceDecoder<DltTraceLineBase> Create()
        {
            if (OutputStream == null || !OutputStream.SupportsBinary) {
                switch (InputFormat) {
                case InputFormat.File:
                    return new DltFileTraceDecoderFactory(FrameMap).Create();
                case InputFormat.Serial:
                    return new DltSerialTraceDecoderFactory(OnlineMode, FrameMap).Create();
                case InputFormat.Network:
                    return new DltTraceDecoderFactory(OnlineMode, FrameMap).Create();
                case InputFormat.Pcap:
                    return new DltPcapTraceDecoder(FrameMap);
                default:
                    throw new InvalidOperationException(AppResources.InfraDltInvalidFormat);
                }
            }

            switch (InputFormat) {
            case InputFormat.File:
                return new DltFileTraceFilterDecoder(OutputStream, FrameMap);
            case InputFormat.Serial:
                return new DltSerialTraceFilterDecoder(OutputStream, OnlineMode, FrameMap);
            case InputFormat.Network:
                return new DltNetworkTraceFilterDecoder(OutputStream, OnlineMode, FrameMap);
            case InputFormat.Pcap:
                return new DltPcapTraceDecoder(OutputStream, FrameMap);
            default:
                throw new InvalidOperationException(AppResources.InfraDltInvalidFormat);
            }
        }
    }
}
