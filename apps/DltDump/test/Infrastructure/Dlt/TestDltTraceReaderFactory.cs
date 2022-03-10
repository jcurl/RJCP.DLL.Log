namespace RJCP.App.DltDump.Infrastructure.Dlt
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;

    public class TestDltTraceReaderFactory : IDltTraceReaderFactory
    {
        /// <summary>
        /// Gets the collection of lines that can be modified which the factory uses to create a reader.
        /// </summary>
        /// <value>The collection of lines to return.</value>
        public ICollection<DltTraceLineBase> Lines { get; } = new List<DltTraceLineBase>();

        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        /// <remarks>This field is not used for the test mocks, as no files or binary data is interpreted.</remarks>
        public InputFormat InputFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if in online mode.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> this is in online mode, where time stamps are obtained by the local host, else
        /// <see langword="false"/>. Formats with a storage header ignore this field.
        /// </value>
        /// <remarks>This field is not used for the test mocks, as no files or binary data is interpreted.</remarks>
        public bool OnlineMode { get; set; }

        /// <summary>
        /// Creates a mocked <see cref="ITraceReader{DltTraceLineBase}"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return Task.FromResult(GetTraceReader());
        }

        /// <summary>
        /// Creates a mocked <see cref="ITraceReader{DltTraceLineBase}"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object the factory knows how to create.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            return Task.FromResult(GetTraceReader());
        }

        private ITraceReader<DltTraceLineBase> GetTraceReader()
        {
            return new LineTraceReader<DltTraceLineBase>(Lines);
        }
    }
}
