namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;
    using System.IO;
    using Resources;

    /// <summary>
    /// An input factory to create a file stream for reading DLT files.
    /// </summary>
    public sealed class DltFileStreamFactory : InputStreamFactoryBase
    {
        /// <summary>
        /// Creates the input stream from a specified URI.
        /// </summary>
        /// <param name="uri">The URI to create the input stream from.</param>
        /// <returns>The input stream that can be used for creating a decoder.</returns>
        /// <exception cref="InputStreamException">
        /// There was a problem instantiating the stream. The message and inner exception provide more information on
        /// the fault.
        /// </exception>
        public override IInputStream Create(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            try {
                string fileName = uri.LocalPath;
                return new DltFileStream(fileName);
            } catch (InvalidOperationException ex) {
                throw new InputStreamException("", ex);
            } catch (FileNotFoundException ex) {
                string message = string.Format(AppResources.FileOpenError_FileNotFound, uri);
                throw new InputStreamException(message, ex);
            } catch (DirectoryNotFoundException ex) {
                string message = string.Format(AppResources.FileOpenError_DirectoryNotFound, uri);
                throw new InputStreamException(message, ex);
            } catch (PathTooLongException ex) {
                string message = string.Format(AppResources.FileOpenError_PathTooLong, uri);
                throw new InputStreamException(message, ex);
            } catch (IOException ex) {
                string message = string.Format(AppResources.FileOpenError_IOException, uri, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (ArgumentException ex) {
                string message = string.Format(AppResources.FileOpenError_InvalidFile, uri, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (NotSupportedException ex) {
                string message = string.Format(AppResources.FileOpenError_InvalidFile, uri, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (System.Security.SecurityException ex) {
                string message = string.Format(AppResources.FileOpenError_Security, uri, ex.Message);
                throw new InputStreamException(message, ex);
            } catch (UnauthorizedAccessException ex) {
                string message = string.Format(AppResources.FileOpenError_Unauthorized, uri);
                throw new InputStreamException(message, ex);
            }
        }
    }
}
