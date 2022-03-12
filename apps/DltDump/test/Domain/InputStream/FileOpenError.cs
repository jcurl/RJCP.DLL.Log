namespace RJCP.App.DltDump.Domain.InputStream
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
}
