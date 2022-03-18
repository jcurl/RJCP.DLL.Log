namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;

    /// <summary>
    /// Constraint to check if the device has been awake for a number of milliseconds.
    /// </summary>
    public class Awake : IMatchConstraint
    {
        private readonly int m_AwakeMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="Awake"/> class.
        /// </summary>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Awake <paramref name="timeout"/> must be zero or greater milliseconds.
        /// </exception>
        public Awake(int timeout)
        {
            if (timeout < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Awake time must be zero or greater milliseconds");
            m_AwakeMilliseconds = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Awake"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Awake <paramref name="timeout"/> must be zero or greater milliseconds;
        /// <para>- or -</para>
        /// Awake <paramref name="timeout"/> is too large.
        /// </exception>
        public Awake(TimeSpan timeout)
        {
            if (timeout.TotalMilliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Awake time must be zero or greater milliseconds");
            if (timeout.TotalMilliseconds > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Awake time is too large");
            m_AwakeMilliseconds = (int)timeout.TotalMilliseconds;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.DeviceTimeStamp &&
                   dltLine.DeviceTimeStamp.TotalMilliseconds >= m_AwakeMilliseconds;
        }
    }
}
