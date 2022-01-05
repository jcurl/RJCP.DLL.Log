namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// The DLT decoder for all control messages.
    /// </summary>
    /// <remarks>
    /// This class is used to decode the service identifier, and then use that and the
    /// <see cref="IDltLineBuilder.DltType"/> to know which decoder should be used for generating the control message
    /// object.
    /// </remarks>
    public class ControlDltDecoder : IControlDltDecoder
    {
        private readonly Dictionary<int, IControlArgDecoder> m_RequestDecoders = new Dictionary<int, IControlArgDecoder>();
        private readonly Dictionary<int, IControlArgDecoder> m_ResponseDecoders = new Dictionary<int, IControlArgDecoder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDltDecoder"/> class.
        /// </summary>
        public ControlDltDecoder()
        {
            InitializeRequestDecoders();
            InitializeResponseDecoders();
        }

        private void InitializeRequestDecoders()
        {
            m_RequestDecoders.Add(0x01, new SetLogLevelRequestDecoder());
            m_RequestDecoders.Add(0x02, new SetTraceStatusRequestDecoder());
            m_RequestDecoders.Add(0x03, new GetLogInfoRequestDecoder());
            m_RequestDecoders.Add(0x04, new GetDefaultLogLevelRequestDecoder());
            m_RequestDecoders.Add(0x05, new StoreConfigurationRequestDecoder());
            m_RequestDecoders.Add(0x06, new ResetFactoryDefaultRequestDecoder());
            m_RequestDecoders.Add(0x09, new SetVerboseModeRequestDecoder());
            m_RequestDecoders.Add(0x0A, new SetMessageFilteringRequestDecoder());
            m_RequestDecoders.Add(0x0B, new SetTimingPacketsRequestDecoder());
            m_RequestDecoders.Add(0x0C, new GetLocalTimeRequestDecoder());
            m_RequestDecoders.Add(0x0D, new SetUseEcuIdRequestDecoder());
            m_RequestDecoders.Add(0x0E, new SetUseSessionIdRequestDecoder());
            m_RequestDecoders.Add(0x0F, new SetUseTimeStampRequestDecoder());
            m_RequestDecoders.Add(0x10, new SetUseExtendedHeaderRequestDecoder());
            m_RequestDecoders.Add(0x11, new SetDefaultLogLevelRequestDecoder());
            m_RequestDecoders.Add(0x12, new SetDefaultTraceStatusRequestDecoder());
            m_RequestDecoders.Add(0x13, new GetSoftwareVersionRequestDecoder());
            m_RequestDecoders.Add(0x14, new MessageBufferOverflowRequestDecoder());
            m_RequestDecoders.Add(0x15, new GetDefaultTraceStatusRequestDecoder());
            m_RequestDecoders.Add(0x19, new GetVerboseModeStatusRequestDecoder());
            m_RequestDecoders.Add(0x1A, new GetMessageFilteringStatusRequestDecoder());
            m_RequestDecoders.Add(0x1B, new GetUseEcuIdRequestDecoder());
            m_RequestDecoders.Add(0x1C, new GetUseSessionIdRequestDecoder());
            m_RequestDecoders.Add(0x1D, new GetUseTimeStampRequestDecoder());
            m_RequestDecoders.Add(0x1E, new GetUseExtendedHeaderRequestDecoder());
            m_RequestDecoders.Add(0x1F, new GetTraceStatusRequestDecoder());
            m_RequestDecoders.Add(0x23, new BufferOverflowNotificationRequestDecoder());
            m_RequestDecoders.Add(0x24, new SyncTimeStampRequestDecoder());
        }

        private void InitializeResponseDecoders()
        {
            m_ResponseDecoders.Add(0x01, new SetLogLevelResponseDecoder());
            m_ResponseDecoders.Add(0x02, new SetTraceStatusResponseDecoder());
            m_ResponseDecoders.Add(0x03, new GetLogInfoResponseDecoder());
            m_ResponseDecoders.Add(0x04, new GetDefaultLogLevelResponseDecoder());
            m_ResponseDecoders.Add(0x05, new StoreConfigurationResponseDecoder());
            m_ResponseDecoders.Add(0x06, new ResetFactoryDefaultResponseDecoder());
            m_ResponseDecoders.Add(0x09, new SetVerboseModeResponseDecoder());
            m_ResponseDecoders.Add(0x0A, new SetMessageFilteringResponseDecoder());
            m_ResponseDecoders.Add(0x0B, new SetTimingPacketsResponseDecoder());
            m_ResponseDecoders.Add(0x0C, new GetLocalTimeResponseDecoder());
            m_ResponseDecoders.Add(0x0D, new SetUseEcuIdResponseDecoder());
            m_ResponseDecoders.Add(0x0E, new SetUseSessionIdResponseDecoder());
            m_ResponseDecoders.Add(0x0F, new SetUseTimeStampResponseDecoder());
            m_ResponseDecoders.Add(0x10, new SetUseExtendedHeaderResponseDecoder());
            m_ResponseDecoders.Add(0x11, new SetDefaultLogLevelResponseDecoder());
            m_ResponseDecoders.Add(0x12, new SetDefaultTraceStatusResponseDecoder());
            m_ResponseDecoders.Add(0x13, new GetSoftwareVersionResponseDecoder());
            m_ResponseDecoders.Add(0x14, new MessageBufferOverflowResponseDecoder());
            m_ResponseDecoders.Add(0x15, new GetDefaultTraceStatusResponseDecoder());
            m_ResponseDecoders.Add(0x19, new GetVerboseModeStatusResponseDecoder());
            m_ResponseDecoders.Add(0x1A, new GetMessageFilteringStatusResponseDecoder());
            m_ResponseDecoders.Add(0x1B, new GetUseEcuIdResponseDecoder());
            m_ResponseDecoders.Add(0x1C, new GetUseSessionIdResponseDecoder());
            m_ResponseDecoders.Add(0x1D, new GetUseTimeStampResponseDecoder());
            m_ResponseDecoders.Add(0x1E, new GetUseExtendedHeaderResponseDecoder());
            m_ResponseDecoders.Add(0x1F, new GetTraceStatusResponseDecoder());
            m_ResponseDecoders.Add(0x23, new BufferOverflowNotificationResponseDecoder());
            m_ResponseDecoders.Add(0x24, new SyncTimeStampResponseDecoder());
            m_ResponseDecoders.Add(0xF01, new CustomUnregisterContextResponseDecoder());
            m_ResponseDecoders.Add(0xF02, new CustomConnectionInfoResponseDecoder());
            m_ResponseDecoders.Add(0xF03, new CustomTimeZoneResponseDecoder());
            m_ResponseDecoders.Add(0xF04, new CustomMarkerResponseDecoder());
        }

        /// <summary>
        /// Registers the specified request decoder with the service ID.
        /// </summary>
        /// <param name="serviceId">
        /// The identifier of the service for which a payload <paramref name="decoder"/> is registered.
        /// </param>
        /// <param name="decoder">The decoder of the payload for the given <paramref name="serviceId"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="decoder"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Registering a request decoder which is already registered will overwrite the previously registered decoder.
        /// </remarks>
        protected void RegisterRequest(int serviceId, IControlArgDecoder decoder)
        {
            Register(m_RequestDecoders, serviceId, decoder);
        }

        /// <summary>
        /// Registers the specified response decoder with the service ID.
        /// </summary>
        /// <param name="serviceId">
        /// The identifier of the service for which a payload <paramref name="decoder"/> is registered.
        /// </param>
        /// <param name="decoder">The decoder of the payload for the given <paramref name="serviceId"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="decoder"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Registering a response decoder which is already registered will overwrite the previously registered decoder.
        /// </remarks>
        protected void RegisterResponse(int serviceId, IControlArgDecoder decoder)
        {
            Register(m_ResponseDecoders, serviceId, decoder);
        }

        /// <summary>
        /// Unregisters the request service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the service was previous defined (and now isn't), <see langword="false"/>
        /// otherwise.
        /// </returns>
        protected bool UnregisterRequest(int serviceId)
        {
            return Unregister(m_RequestDecoders, serviceId);
        }

        /// <summary>
        /// Unregisters the response service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the service was previous defined (and now isn't), <see langword="false"/>
        /// otherwise.
        /// </returns>
        protected bool UnregisterResponse(int serviceId)
        {
            return Unregister(m_ResponseDecoders, serviceId);
        }

        private static void Register(Dictionary<int, IControlArgDecoder> decoders, int serviceId, IControlArgDecoder decoder)
        {
            if (decoder == null) throw new ArgumentNullException(nameof(decoder));
            decoders.Remove(serviceId);
            decoders.Add(serviceId, decoder);
        }

        private static bool Unregister(Dictionary<int, IControlArgDecoder> decoders, int serviceId)
        {
            return decoders.Remove(serviceId);
        }

        private readonly IControlArgDecoder m_SwInjectionRequestDecoder = new SwInjectionRequestDecoder();
        private readonly IControlArgDecoder m_SwInjectionResponseDecoder = new SwInjectionResponseDecoder();

        /// <summary>
        /// Decodes the DLT control message payload.
        /// </summary>
        /// <param name="buffer">The buffer where the DLT control message encoded payload can be found.</param>
        /// <param name="lineBuilder">The DLT trace line builder.</param>
        /// <returns>The number of bytes decoded, or -1 upon error.</returns>
        /// <remarks>The result of the decoding is written directly to the <paramref name="lineBuilder"/>.</remarks>
        public int Decode(ReadOnlySpan<byte> buffer, IDltLineBuilder lineBuilder)
        {
            try {
                int serviceId;
                IControlArgDecoder decoder;

                switch (lineBuilder.DltType) {
                case DltType.CONTROL_REQUEST:
                    serviceId = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    if (!m_RequestDecoders.TryGetValue(serviceId, out decoder)) {
                        if (serviceId >= 0 && serviceId < 0xFFF) {
                            Log.Dlt.TraceEvent(TraceEventType.Warning, "No decoder for control request message service 0x{0:x}", serviceId);
                            return -1;
                        }
                        decoder = m_SwInjectionRequestDecoder;
                    }
                    break;
                case DltType.CONTROL_RESPONSE:
                    serviceId = BitOperations.To32Shift(buffer, !lineBuilder.BigEndian);
                    if (!m_ResponseDecoders.TryGetValue(serviceId, out decoder)) {
                        if (serviceId >= 0 && serviceId < 0xFFF) {
                            Log.Dlt.TraceEvent(TraceEventType.Warning, "No decoder for control response message service 0x{0:x}", serviceId);
                            return -1;
                        }
                        decoder = m_SwInjectionResponseDecoder;
                    }
                    break;
                case DltType.CONTROL_TIME:
                    lineBuilder.SetControlPayload(new DltTimeMarker());
                    return 0;
                default:
                    if (Log.Dlt.ShouldTrace(TraceEventType.Warning)) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "Invalid control message {0}", lineBuilder.DltType.ToString());
                    }
                    return -1;
                }

                int decoded = decoder.Decode(serviceId, buffer, lineBuilder.BigEndian, out IControlArg service);
                if (decoded == -1 || service == null) {
                    if (Log.Dlt.ShouldTrace(TraceEventType.Warning)) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "Failed decoding {0} message service 0x{1:x}",
                            lineBuilder.DltType.ToString(), serviceId);
                    }
                    return -1;
                }

                lineBuilder.SetControlPayload(service);
                return decoded;
            } catch (Exception ex) {
                Log.Dlt.TraceException(ex, nameof(Decode), "Exception while decoding control message");
                return -1;
            }
        }
    }
}
