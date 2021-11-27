namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for reading a log file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITraceReader<T> : IDisposable where T : ITraceLine
    {
        /// <summary>
        /// Gets the next line from the stream asynchronously.
        /// </summary>
        /// <returns>The next line from the stream.</returns>
        Task<T> GetLineAsync();
    }
}
