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
        /// Various errors that can be simulated when opening a file.
        /// </summary>
        public enum FileOpenError
        {
            None,
            ArgumentNullException,
            ArgumentException,
            NotSupportedException,
            FileNotFoundException,
            IOException,
            SecurityException,
            DirectoryNotFoundException,
            UnauthorizedAccessException,
            PathTooLongException,
            InvalidOperationException
        }

        /// <summary>
        /// Used to simulate an error on opening a file name.
        /// </summary>
        /// <value>The exception that should be simulated when opening a file.</value>
        public FileOpenError OpenError { get; set; } = FileOpenError.None;

        /// <summary>
        /// Gets the collection of lines that can be modified which the factory uses to create a reader.
        /// </summary>
        /// <value>The collection of lines to return.</value>
        public ICollection<DltTraceLineBase> Lines { get; } = new List<DltTraceLineBase>();

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

            switch (OpenError) {
            case FileOpenError.ArgumentNullException:
                throw new ArgumentNullException(nameof(fileName));
            case FileOpenError.ArgumentException:
                throw new ArgumentException("Simulated Argument Exception", nameof(fileName));
            case FileOpenError.NotSupportedException:
                throw new NotSupportedException("Simulated Not Supported Exception");
            case FileOpenError.FileNotFoundException:
                throw new FileNotFoundException("Simulated File Not Found Exception", fileName);
            case FileOpenError.IOException:
                throw new IOException("Simulated I/O Exception");
            case FileOpenError.SecurityException:
                throw new System.Security.SecurityException("Simulated Security Exception");
            case FileOpenError.DirectoryNotFoundException:
                throw new DirectoryNotFoundException("Simulated Directory Exception");
            case FileOpenError.UnauthorizedAccessException:
                throw new UnauthorizedAccessException("Simulated Unauthorized Exception");
            case FileOpenError.PathTooLongException:
                throw new PathTooLongException("Simulated Path Too Long Exception");
            case FileOpenError.InvalidOperationException:
                throw new InvalidOperationException("Simulated Invalid Operation Exception");
            }

            return Task.FromResult(GetTraceReader());
        }

        private ITraceReader<DltTraceLineBase> GetTraceReader()
        {
            return new LineTraceReader<DltTraceLineBase>(Lines);
        }
    }
}
