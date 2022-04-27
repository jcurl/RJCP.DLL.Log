namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Collections.Generic;
    using Pcap;
    using Pcap.Legacy;
    using Resources;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A DLT decoder that extracts DLT from UDP packets in a PCAP / PCAP-NG file, determining the format from the
    /// header.
    /// </summary>
    public class DltPcapTraceDecoder : ITraceDecoder<DltTraceLineBase>
    {
        private readonly IOutputStream m_OutputStream;
        private ITraceDecoder<DltTraceLineBase> m_PcapDecoder;
        private readonly byte[] m_Header = new byte[4];
        private int m_HeaderLength;

        public DltPcapTraceDecoder() { }

        public DltPcapTraceDecoder(IOutputStream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            m_OutputStream = outputStream;
        }

        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <remarks>
        /// This method shall accept any number of bytes for decoding. It should also consume all data that is received,
        /// so that data which is not processed is buffered locally by the decoder.
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            if (m_PcapDecoder == null) {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(nameof(DltPcapLegacyDecoder));

                if (m_HeaderLength + buffer.Length < m_Header.Length) {
                    // We don't know what the PCAP type is, so store it and wait for the next write.
                    buffer.CopyTo(m_Header.AsSpan(m_HeaderLength));
                    m_HeaderLength += buffer.Length;
                    return Array.Empty<DltTraceLineBase>();
                } else {
                    if (m_HeaderLength != 0) {
                        int hdr = m_Header.Length - m_HeaderLength;
                        buffer[0..hdr].CopyTo(m_Header.AsSpan(m_HeaderLength));
                        GetPcapDecoder(m_Header);
                        m_PcapDecoder.Decode(m_Header, 0);
                        buffer = buffer[hdr..];
                        position += hdr;
                    } else {
                        GetPcapDecoder(buffer);
                    }
                }
            }

            return m_PcapDecoder.Decode(buffer, position);
        }

        private void GetPcapDecoder(ReadOnlySpan<byte> header)
        {
            int magicBytes = BitOperations.To32ShiftLittleEndian(header);
            switch (magicBytes) {
            case 0x0D0A0A0D:
                // PCAP-NG file
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownMagic);
            case unchecked((int)0xA1B2C3D4):
            case unchecked((int)0xA1B23C4D):
            case unchecked((int)0xD4C3B2A1):
            case 0x4D3CB2A1:
                if (m_OutputStream == null) {
                    m_PcapDecoder = new DltPcapLegacyDecoder();
                } else {
                    m_PcapDecoder = new DltPcapLegacyDecoder(m_OutputStream);
                }
                return;
            }

            throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownMagic);
        }

        /// <summary>
        /// Flushes any data that is locally cached, and returns any pending trace lines.
        /// </summary>
        /// <returns>A read only collection of the decoded lines.</returns>
        /// <remarks>
        /// Flushing a decoder typically happens by the trace reader when the stream is finished, so that any remaining
        /// data the decoder may have can be returned to the user application (including error trace lines).
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Flush()
        {
            if (m_PcapDecoder != null) return m_PcapDecoder.Flush();
            return Array.Empty<DltTraceLineBase>();
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
                if (!m_IsDisposed && m_PcapDecoder != null) {
                    m_PcapDecoder.Dispose();
                }
            }

            m_IsDisposed = true;
        }
    }
}
