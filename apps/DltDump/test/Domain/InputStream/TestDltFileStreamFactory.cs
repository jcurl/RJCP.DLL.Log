namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;

    /// <summary>
    /// A test file stream factory that can simulate errors opening files. No files are accessed.
    /// </summary>
    public class TestDltFileStreamFactory : InputStreamFactoryBase
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
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from. This is ignored as it is a mock.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="InputStreamException">Simulated exception that is caught.</exception>
        /// <exception cref="InvalidOperationException">Simulated Invalid Operation Exception.</exception>
        public override IInputStream Create(Uri uri)
        {
            Exception ex;
            switch (OpenError) {
            case FileOpenError.ArgumentNullException:
                ex = new ArgumentNullException(nameof(uri));
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.ArgumentException:
                ex = new ArgumentException("Simulated Argument Exception", nameof(uri));
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.NotSupportedException:
                ex = new NotSupportedException("Simulated Not Supported Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.FileNotFoundException:
                ex = new FileNotFoundException("Simulated File Not Found Exception", uri.LocalPath);
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.IOException:
                ex = new IOException("Simulated I/O Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.SecurityException:
                ex = new System.Security.SecurityException("Simulated Security Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.DirectoryNotFoundException:
                ex = new DirectoryNotFoundException("Simulated Directory Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.UnauthorizedAccessException:
                ex = new UnauthorizedAccessException("Simulated Unauthorized Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.PathTooLongException:
                ex = new PathTooLongException("Simulated Path Too Long Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.InvalidOperationException:
                throw new InvalidOperationException("Simulated Invalid Operation Exception");
            }

            return new NullInputStream();
        }
    }
}
