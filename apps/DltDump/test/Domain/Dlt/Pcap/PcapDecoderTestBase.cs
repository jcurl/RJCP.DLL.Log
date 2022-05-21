namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.OutputStream;
    using NUnit.Framework;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    public abstract class PcapDecoderTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcapLegacyDecoderTest{TDec}"/> class.
        /// </summary>
        /// <param name="outputStream">
        /// If set to <see langword="true"/>, test with an <see cref="IOutputStream"/>.
        /// </param>
        protected PcapDecoderTestBase(bool outputStream)
        {
            if (outputStream) MemOutputStream = new MemoryOutput();
        }

        /// <summary>
        /// Gets the memory output stream.
        /// </summary>
        /// <value>
        /// The memory output stream.
        /// </value>
        protected MemoryOutput MemOutputStream { get; }

        protected static readonly int[] ReadChunks = { 0, 1, 2, 3, 5, 8, 13, 21, 50, 100 };

        /// <summary>
        /// Decodes the data using the given trace decoder splitting data in chunks.
        /// </summary>
        /// <param name="traceDecoder">The trace decoder.</param>
        /// <param name="data">The data.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns>A list of liens that are decoded.</returns>
        protected IList<DltTraceLineBase> Decode(ITraceDecoder<DltTraceLineBase> traceDecoder, ReadOnlySpan<byte> data, int chunkSize)
        {
            if (MemOutputStream != null) MemOutputStream.Clear();

            List<DltTraceLineBase> lines;
            if (chunkSize == 0) {
                lines = new List<DltTraceLineBase>(traceDecoder.Decode(data, 0));
            } else {
                lines = new List<DltTraceLineBase>();
                int offset = 0;
                while (offset < data.Length) {
                    int chunkLength = Math.Min(chunkSize, data.Length - offset);
                    IEnumerable<DltTraceLineBase> decodedLines = traceDecoder.Decode(data[offset..(offset + chunkLength)], offset);
                    lines.AddRange(decodedLines);
                    offset += chunkSize;
                }
            }

            if (MemOutputStream != null) {
                Assert.That(lines.Count, Is.EqualTo(0));
                return (from line in MemOutputStream.Lines select line.Line).ToList();
            }
            return lines;
        }

        /// <summary>
        /// Decodes the data using the given trace decoder splitting data in chunks.
        /// </summary>
        /// <param name="traceDecoder">The trace decoder.</param>
        /// <param name="data">The data blocks as a list.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns>A list of liens that are decoded.</returns>
        /// <remarks>
        /// This method takes the data chunks, and copies it into a larger array.
        /// </remarks>
        protected IList<DltTraceLineBase> Decode(ITraceDecoder<DltTraceLineBase> traceDecoder, IEnumerable<byte[]> data, int chunkSize)
        {
            int dataSize = 0;
            foreach (byte[] block in data) {
                dataSize += block.Length;
            }

            byte[] fullData = new byte[dataSize];
            int cursor = 0;
            foreach (byte[] block in data) {
                Array.Copy(block, 0, fullData, cursor, block.Length);
                cursor += block.Length;
            }

            return Decode(traceDecoder, fullData, chunkSize);
        }

        /// <summary>
        /// Flushes the specified trace decoder.
        /// </summary>
        /// <param name="traceDecoder">The trace decoder.</param>
        /// <returns>A list of liens that are decoded.</returns>
        protected IList<DltTraceLineBase> Flush(ITraceDecoder<DltTraceLineBase> traceDecoder)
        {
            if (MemOutputStream != null) MemOutputStream.Clear();

            List<DltTraceLineBase> lines = new List<DltTraceLineBase>(traceDecoder.Flush());
            if (MemOutputStream != null) {
                Assert.That(lines.Count, Is.EqualTo(0));
                return (from line in MemOutputStream.Lines select line.Line).ToList();
            }

            return lines;
        }
    }
}
