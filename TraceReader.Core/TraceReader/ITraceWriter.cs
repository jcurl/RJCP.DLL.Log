namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for writing lines to an output.
    /// </summary>
    /// <typeparam name="T">The base-type that describes the line that is to be written.</typeparam>
    public interface ITraceWriter<in T> : IDisposable where T : ITraceLine
    {
        /// <summary>
        /// Writes the line asynchronously to the output.
        /// </summary>
        /// <param name="line">The line to write to the output.</param>
        /// <returns>A Task that can be waited upon. The result indicates success.</returns>
        Task<bool> WriteLineAsync(T line);
    }
}
