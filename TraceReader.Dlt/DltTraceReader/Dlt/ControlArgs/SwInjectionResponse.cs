namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;

    /// <summary>
    /// Represents arbitrary data for a software injection response.
    /// </summary>
    public sealed class SwInjectionResponse : ControlResponse
    {
        /// <summary>
        /// Pending code.
        /// </summary>
        public const int StatusPending = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwInjectionResponse"/> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="status">The status.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serviceId"/> is outside valid range of 0xFFF..0xFFFFFFFF.
        /// </exception>
        public SwInjectionResponse(int serviceId, int status) : base(status)
        {
            if (serviceId >= 0 && serviceId < 0xFFF)
                throw new ArgumentOutOfRangeException(nameof(serviceId), "ServiceID outside valid range of 0xFFF..0xFFFFFFFF");

            ServiceId = serviceId;
        }

        /// <summary>
        /// Gets the service identifier of the control message.
        /// </summary>
        /// <value>The service identifier of the control message.</value>
        public override int ServiceId { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[ {0}]", ToString(Status));
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>
        /// Shall return <see cref="string.Empty"/> if the <paramref name="status"/> code is not recognized.
        /// </remarks>
        /// /
        protected override string ToString(int status)
        {
            if (Status == StatusPending) return "pending";

            return base.ToString(status);
        }
    }
}
