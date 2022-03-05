namespace RJCP.App.DltDump
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Domain;
    using Infrastructure.Dlt;
    using Infrastructure.Terminal;

    /// <summary>
    /// Provide a context for the global <see cref="Global"/> state.
    /// </summary>
    /// <remarks>
    /// Any test that uses classes which depend on the global state cannot run in parallel with one another. While it's
    /// easy to just call <see cref="Global.Reset"/> in the test case, wrapping it in a <c>using</c> statement makes the
    /// scope clear.
    /// </remarks>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Design calls for an object, no performance impact")]
    public sealed class TestApplication : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestApplication"/> class.
        /// </summary>
        /// <remarks>
        /// The factory elements are set to <see langword="null"/>, so that a test case is forced to properly provide a
        /// mock.
        /// </remarks>
        public TestApplication()
        {
            Global.Instance.CommandFactory = null;
            Global.Instance.Terminal = new VirtualTerminal();
            Global.Instance.InputStreamFactory = new TestInputStreamFactory();
            Global.Instance.DltReaderFactory = new TestDltTraceReaderFactory();
        }

        /// <summary>
        /// Gets the virtualised standard output.
        /// </summary>
        /// <value>The virtualised standard out.</value>
        public VirtualStdOut StdOut
        {
            get
            {
                if (!(Global.Instance.Terminal is VirtualTerminal term)) return null;
                return (VirtualStdOut)term.StdOut;
            }
        }

        /// <summary>
        /// Gets the virtualised standard error.
        /// </summary>
        /// <value>The virtualised standard error.</value>
        public VirtualStdErr StdErr
        {
            get
            {
                if (!(Global.Instance.Terminal is VirtualTerminal term)) return null;
                return (VirtualStdErr)term.StdErr;
            }
        }

        /// <summary>
        /// Writes the virtual <see cref="StdOut"/> and <see cref="StdErr"/> to the console.
        /// </summary>
        public void WriteStd()
        {
            foreach (string line in StdOut.Lines) {
                Console.WriteLine(line);
            }

            foreach (string line in StdErr.Lines) {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// When this object is disposed, the global state is reset.
        /// </summary>
        public void Dispose()
        {
            Global.Reset();
        }
    }
}
