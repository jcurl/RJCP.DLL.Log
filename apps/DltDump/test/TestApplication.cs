namespace RJCP.App.DltDump
{
    using System;

    /// <summary>
    /// Provide a context for the global <see cref="Global"/> state.
    /// </summary>
    /// <remarks>
    /// Any test that uses classes which depend on the global state cannot run in parallel with one another. While it's
    /// easy to just call <see cref="Global.Reset"/> in the test case, wrapping it in a <c>using</c> statement
    /// makes the scope clear.
    /// </remarks>
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
            Global.Instance.Terminal = null;
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
