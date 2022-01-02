﻿namespace RJCP.Diagnostics.Log.Dlt.Control
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
            m_RequestDecoders.Add(0x04, new GetDefaultLogLevelRequestDecoder());
            m_RequestDecoders.Add(0x13, new GetSoftwareVersionRequestDecoder());
        }

        private void InitializeResponseDecoders()
        {
            m_ResponseDecoders.Add(0x01, new SetLogLevelResponseDecoder());
            m_ResponseDecoders.Add(0x02, new SetTraceStatusResponseDecoder());
            m_ResponseDecoders.Add(0x04, new GetDefaultLogLevelResponseDecoder());
            m_ResponseDecoders.Add(0x13, new GetSoftwareVersionResponseDecoder());
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
                    serviceId = BitOperations.To32ShiftLittleEndian(buffer);
                    if (!m_RequestDecoders.TryGetValue(serviceId, out decoder)) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "No decoder for control request message service 0x{0:x}", serviceId);
                        return -1;
                    }
                    break;
                case DltType.CONTROL_RESPONSE:
                    serviceId = BitOperations.To32ShiftLittleEndian(buffer);
                    if (!m_ResponseDecoders.TryGetValue(serviceId, out decoder)) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "No decoder for control response message service 0x{0:x}", serviceId);
                        return -1;
                    }
                    break;
                case DltType.CONTROL_TIME:
                default:
                    if (Log.Dlt.ShouldTrace(TraceEventType.Warning)) {
                        Log.Dlt.TraceEvent(TraceEventType.Warning, "Invalid control message {0}", lineBuilder.DltType.ToString());
                    }
                    return -1;
                }

                int decoded = decoder.Decode(serviceId, buffer, out IControlArg service);
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
