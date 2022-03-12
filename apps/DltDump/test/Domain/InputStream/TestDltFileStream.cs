namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;

    public class TestDltFileStream : NullInputStream
    {
        /// <summary>
        /// Used to simulate an error on opening a file name.
        /// </summary>
        /// <value>The exception that should be simulated when opening a file.</value>
        public FileOpenError OpenError { get; set; } = FileOpenError.None;

        /// <summary>
        /// Opens this instance.
        /// </summary>
        /// <returns>Stream.</returns>
        /// <exception cref="InputStreamException"></exception>
        public override void Open()
        {
            Exception ex;
            switch (OpenError) {
            case FileOpenError.ArgumentNullException:
                ex = new ArgumentNullException(nameof(Connection));
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.ArgumentException:
                ex = new ArgumentException("Simulated Argument Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.NotSupportedException:
                ex = new NotSupportedException("Simulated Not Supported Exception");
                throw new InputStreamException(ex.Message, ex);
            case FileOpenError.FileNotFoundException:
                ex = new FileNotFoundException("Simulated File Not Found Exception", Connection);
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

            base.Open();
        }
    }
}
