namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections.Generic;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A DLT decoder that extracts DLT from UDP packets in a PCAP-NG file.
    /// </summary>
    public class DltPcapNgDecoder : ITraceDecoder<DltTraceLineBase>
    {
        private readonly IOutputStream m_OutputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNgDecoder"/> class without an output stream or filter.
        /// </summary>
        public DltPcapNgDecoder() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNgDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputStream"/> is <see langword="null"/>.
        /// </exception>
        public DltPcapNgDecoder(IOutputStream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            m_OutputStream = outputStream;
        }

        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DltTraceLineBase> Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Is <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                if (!m_IsDisposed) {
                    /* Nothing to do yet */
                }
            }

            m_IsDisposed = true;
        }
    }
}
