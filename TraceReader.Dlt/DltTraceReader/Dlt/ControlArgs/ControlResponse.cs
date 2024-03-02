namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;

    /// <summary>
    /// The base class for control response messages.
    /// </summary>
    public abstract class ControlResponse : IControlArg
    {
        /// <summary>
        /// The response is OK.
        /// </summary>
        public const int StatusOk = 0;

        /// <summary>
        /// The response status is that the request is unsupported.
        /// </summary>
        public const int StatusNotSupported = 1;

        /// <summary>
        /// The response status is that there is a general error.
        /// </summary>
        public const int StatusError = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlResponse"/> class.
        /// </summary>
        /// <param name="status">The status code for the response.</param>
        protected ControlResponse(int status)
        {
            Status = status;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public abstract int ServiceId { get; }

        /// <summary>
        /// Gets the default type.
        /// </summary>
        /// <value>The default type/subtype of the DLT message that is expected to hold this payload.</value>
        /// <remarks>Control responses should have the type of <see cref="DltType.CONTROL_RESPONSE"/>.</remarks>
        public DltType DefaultType
        {
            get { return DltType.CONTROL_RESPONSE; }
        }

        private int m_Status;

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        /// <exception cref="ArgumentOutOfRangeException">The status code is a negative number.</exception>
        public int Status
        {
            get { return m_Status; }

            protected set
            {
                ThrowHelper.ThrowIfNegative(value, nameof(Status));
                m_Status = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString(Status);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>
        /// Shall return status=&lt;status_code&gt; if the <paramref name="status"/> code is not recognized.
        /// </remarks>
        protected virtual string ToString(int status)
        {
            switch (status) {
            case StatusOk:
                return "ok";
            case StatusNotSupported:
                return "not_supported";
            case StatusError:
                return "error";
            default:
                return string.Format("status={0}", status);
            }
        }
    }
}
