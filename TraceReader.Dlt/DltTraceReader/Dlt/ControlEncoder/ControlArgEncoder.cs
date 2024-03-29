﻿namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using System.Collections.Generic;
    using ControlArgs;
    using RJCP.Core;

    /// <summary>
    /// Class to encode the contents of a control message.
    /// </summary>
    /// <remarks>
    /// The control message is encoded based on the type of the control message and the service identifier. This class
    /// has limited extensibility to add/remove encoders for specific control message service identifiers.
    /// </remarks>
    public class ControlArgEncoder : IControlArgEncoder
    {
        private readonly IControlArgEncoder[] m_RequestEncodersStandard = new IControlArgEncoder[128];
        private readonly IControlArgEncoder[] m_ResponseEncodersStandard = new IControlArgEncoder[128];
        private readonly IControlArgEncoder[] m_RequestEncodersCustom = new IControlArgEncoder[128];
        private readonly IControlArgEncoder[] m_ResponseEncodersCustom = new IControlArgEncoder[128];
        private readonly Dictionary<int, IControlArgEncoder> m_RequestSwInjection = new();
        private readonly Dictionary<int, IControlArgEncoder> m_ResponseSwInjection = new();

        /// <summary>
        /// A default Empty Request encoder, for when there is nothing, other than the service id, to encode.
        /// </summary>
        protected static readonly IControlArgEncoder EmptyRequestEncoder = new EmptyControlArgEncoder();

        /// <summary>
        /// The empty response encoder, for when there is nothing, other than the service id and the status, to encode.
        /// </summary>
        protected static readonly IControlArgEncoder EmptyResponseEncoder = new ControlArgResponseEncoder();

        private static readonly IControlArgEncoder DefaultRequestSwInjection = new SwInjectionRequestEncoder();

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlArgEncoder"/> class.
        /// </summary>
        public ControlArgEncoder()
        {
            m_RequestEncodersStandard[0x01] = new SetLogLevelRequestEncoder();
            m_RequestEncodersStandard[0x02] = new SetTraceStatusRequestEncoder();
            m_RequestEncodersStandard[0x03] = new GetLogInfoRequestEncoder();
            m_RequestEncodersStandard[0x04] = EmptyRequestEncoder;   // GetDefaultLogLevel request
            m_RequestEncodersStandard[0x05] = EmptyRequestEncoder;   // StoreConfiguration request
            m_RequestEncodersStandard[0x06] = EmptyRequestEncoder;   // ResetFactoryDefault request
            m_RequestEncodersStandard[0x09] = new SetVerboseModeRequestEncoder();
            m_RequestEncodersStandard[0x0A] = new SetMessageFilteringRequestEncoder();
            m_RequestEncodersStandard[0x0B] = new SetTimingPacketsRequestEncoder();
            m_RequestEncodersStandard[0x0C] = EmptyRequestEncoder;   // GetLocalTime request
            m_RequestEncodersStandard[0x0D] = new SetUseEcuIdRequestEncoder();
            m_RequestEncodersStandard[0x0E] = new SetUseSessionIdRequestEncoder();
            m_RequestEncodersStandard[0x0F] = new SetUseTimeStampRequestEncoder();
            m_RequestEncodersStandard[0x10] = new SetUseExtendedHeaderRequestEncoder();
            m_RequestEncodersStandard[0x11] = new SetDefaultLogLevelRequestEncoder();
            m_RequestEncodersStandard[0x12] = new SetDefaultTraceStatusRequestEncoder();
            m_RequestEncodersStandard[0x13] = EmptyRequestEncoder;   // GetSoftwareVersion request
            m_RequestEncodersStandard[0x14] = EmptyRequestEncoder;   // MessageBufferOverflow request
            m_RequestEncodersStandard[0x15] = EmptyRequestEncoder;   // GetDefaultTraceStatus request
            m_RequestEncodersStandard[0x19] = EmptyRequestEncoder;   // GetVerboseModeStatus request
            m_RequestEncodersStandard[0x1A] = EmptyRequestEncoder;   // GetMessageFilteringStatus request
            m_RequestEncodersStandard[0x1B] = EmptyRequestEncoder;   // GetUseECUID request
            m_RequestEncodersStandard[0x1C] = EmptyRequestEncoder;   // GetUseSessionId request
            m_RequestEncodersStandard[0x1D] = EmptyRequestEncoder;   // GetUseTimeStamp request
            m_RequestEncodersStandard[0x1E] = EmptyRequestEncoder;   // GetUseExtendedHeader request
            m_RequestEncodersStandard[0x1F] = new GetTraceStatusRequestEncoder();
            m_RequestEncodersStandard[0x23] = EmptyRequestEncoder;   // BufferOverflowNotification request
            m_RequestEncodersStandard[0x24] = EmptyRequestEncoder;   // SyncTimeStamp request

            m_ResponseEncodersStandard[0x01] = EmptyResponseEncoder; // SetLogLevel response
            m_ResponseEncodersStandard[0x02] = EmptyResponseEncoder; // SetTraceStatus response
            m_ResponseEncodersStandard[0x03] = new GetLogInfoResponseEncoder();
            m_ResponseEncodersStandard[0x04] = new GetDefaultLogLevelResponseEncoder();
            m_ResponseEncodersStandard[0x05] = EmptyResponseEncoder; // StoreConfiguration response
            m_ResponseEncodersStandard[0x06] = EmptyResponseEncoder; // ResetFactoryDefault response
            m_ResponseEncodersStandard[0x09] = EmptyResponseEncoder; // SetVerboseMode response
            m_ResponseEncodersStandard[0x0A] = EmptyResponseEncoder; // SetMessageFiltering response
            m_ResponseEncodersStandard[0x0B] = EmptyResponseEncoder; // SetTimingPackets response
            m_ResponseEncodersStandard[0x0C] = EmptyResponseEncoder; // GetLocalTime response
            m_ResponseEncodersStandard[0x0D] = EmptyResponseEncoder; // SetUseECUID response
            m_ResponseEncodersStandard[0x0E] = EmptyResponseEncoder; // SetUseSessionID response
            m_ResponseEncodersStandard[0x0F] = EmptyResponseEncoder; // UseTimeStamp response
            m_ResponseEncodersStandard[0x10] = EmptyResponseEncoder; // UseExtendedHeader response
            m_ResponseEncodersStandard[0x11] = EmptyResponseEncoder; // SetDefaultLogLevel response
            m_ResponseEncodersStandard[0x12] = EmptyResponseEncoder; // SetDefaultTraceStatus response
            m_ResponseEncodersStandard[0x13] = new GetSoftwareVersionResponseEncoder();
            m_ResponseEncodersStandard[0x14] = new MessageBufferOverflowResponseEncoder();
            m_ResponseEncodersStandard[0x15] = new GetDefaultTraceStatusResponseEncoder();
            m_ResponseEncodersStandard[0x19] = new GetVerboseModeStatusResponseEncoder();
            m_ResponseEncodersStandard[0x1A] = new GetMessageFilteringStatusResponseEncoder();
            m_ResponseEncodersStandard[0x1B] = new GetUseEcuIdResponseEncoder();
            m_ResponseEncodersStandard[0x1C] = new GetUseSessionIdResponseEncoder();
            m_ResponseEncodersStandard[0x1D] = new GetUseTimeStampResponseEncoder();
            m_ResponseEncodersStandard[0x1E] = new GetUseExtendedHeaderResponseEncoder();
            m_ResponseEncodersStandard[0x1F] = new GetTraceStatusResponseEncoder();
            m_ResponseEncodersStandard[0x23] = new BufferOverflowNotificationResponseEncoder();
            m_ResponseEncodersStandard[0x24] = new SyncTimeStampResponseEncoder();

            m_ResponseEncodersCustom[0x01] = new CustomUnregisterContextResponseEncoder();
            m_ResponseEncodersCustom[0x02] = new CustomConnectionInfoResponseEncoder();
            m_ResponseEncodersCustom[0x03] = new CustomTimeZoneResponseEncoder();
            m_ResponseEncodersCustom[0x04] = EmptyResponseEncoder;  // CustomMarker response
        }

        /// <summary>
        /// Registers the specified service identifier encoder.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="type">
        /// The type (only <see cref="DltType.CONTROL_REQUEST"/> and <see cref="DltType.CONTROL_RESPONSE"/> are
        /// supported.
        /// </param>
        /// <param name="encoder">The encoder to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoder"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Unsupported service <paramref name="type"/> with service identifier <paramref name="serviceId"/>.
        /// </exception>
        /// <remarks>
        /// Intended to be called by your own constructor to add new encoders, extending the functionality of this
        /// class. Note that a <see cref="ControlErrorNotSupported"/> response will always be encoded regardless of the
        /// <paramref name="serviceId"/>.
        /// </remarks>
        protected void Register(int serviceId, DltType type, IControlArgEncoder encoder)
        {
            ArgumentNullException.ThrowIfNull(encoder);
            switch (type) {
            case DltType.CONTROL_REQUEST:
                if (serviceId >= 0 && serviceId <= m_RequestEncodersStandard.Length) {
                    m_RequestEncodersStandard[serviceId] = encoder;
                    return;
                } else if (serviceId >= 0xF00 && serviceId <= m_RequestEncodersCustom.Length + 0xF00) {
                    m_RequestEncodersCustom[serviceId - 0xF00] = encoder;
                    return;
                } else if (serviceId is >= 0xFFF or < 0) {
                    m_RequestSwInjection[serviceId] = encoder;
                    return;
                }
                break;
            case DltType.CONTROL_RESPONSE:
                if (serviceId >= 0 && serviceId <= m_ResponseEncodersStandard.Length) {
                    m_ResponseEncodersStandard[serviceId] = encoder;
                    return;
                } else if (serviceId >= 0xF00 && serviceId <= m_ResponseEncodersCustom.Length + 0xF00) {
                    m_ResponseEncodersCustom[serviceId - 0xF00] = encoder;
                    return;
                } else if (serviceId is >= 0xFFF or < 0) {
                    m_ResponseSwInjection[serviceId] = encoder;
                    return;
                }
                break;
            }

            throw new ArgumentException($"Unsupported service {type} with service identifier {serviceId}");
        }

        /// <summary>
        /// Unregisters the specified service identifier encoder.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="type">The type.</param>
        /// <remarks>
        /// Removes the encoder so that the specified <paramref name="serviceId"/> and <paramref name="type"/> will not
        /// be encoded.
        /// </remarks>
        protected void Unregister(int serviceId, DltType type)
        {
            switch (type) {
            case DltType.CONTROL_REQUEST:
                if (serviceId >= 0 && serviceId <= m_RequestEncodersStandard.Length) {
                    m_RequestEncodersStandard[serviceId] = null;
                } else if (serviceId >= 0xF00 && serviceId <= m_RequestEncodersCustom.Length + 0xF00) {
                    m_RequestEncodersCustom[serviceId - 0xF00] = null;
                } else if (serviceId is >= 0xFFF or < 0) {
                    m_RequestSwInjection.Remove(serviceId);
                }
                break;
            case DltType.CONTROL_RESPONSE:
                if (serviceId >= 0 && serviceId <= m_ResponseEncodersStandard.Length) {
                    m_ResponseEncodersStandard[serviceId] = null;
                } else if (serviceId >= 0xF00 && serviceId <= m_ResponseEncodersCustom.Length + 0xF00) {
                    m_ResponseEncodersCustom[serviceId - 0xF00] = null;
                } else if (serviceId is >= 0xFFF or < 0) {
                    m_ResponseSwInjection.Remove(serviceId);
                }
                break;
            }
        }

        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the control argument to.</param>
        /// <param name="msbf">If <see langword="true"/> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        public virtual Result<int> Encode(Span<byte> buffer, bool msbf, IControlArg arg)
        {
            switch (arg.DefaultType) {
            case DltType.CONTROL_REQUEST:
                return EncodeRequest(buffer, msbf, (ControlRequest)arg);
            case DltType.CONTROL_RESPONSE:
                return EncodeResponse(buffer, msbf, (ControlResponse)arg);
            case DltType.CONTROL_TIME:
                // We don't care about the `IControlArg` (e.g. `DltTimeMarker`).
                return 0;
            default:
                return Result.FromException<int>(new DltEncodeException($"Unknown request service {arg.DefaultType} with service {arg.ServiceId:x}"));
            }
        }

        private Result<int> EncodeRequest(Span<byte> buffer, bool msbf, ControlRequest arg)
        {
            IControlArgEncoder encoder = null;
            if (arg.ServiceId >= 0 && arg.ServiceId <= m_RequestEncodersStandard.Length) {
                encoder = m_RequestEncodersStandard[arg.ServiceId];
            } else if (arg.ServiceId >= 0xF00 && arg.ServiceId <= m_RequestEncodersCustom.Length + 0xF00) {
                encoder = m_RequestEncodersCustom[arg.ServiceId - 0xF00];
            } else if (arg.ServiceId is >= 0xFFF or < 0) {
                if (!m_RequestSwInjection.TryGetValue(arg.ServiceId, out encoder))
                    encoder = DefaultRequestSwInjection;
            }

            if (encoder is null)
                return Result.FromException<int>(new DltEncodeException($"Unknown request service {arg.DefaultType} with service {arg.ServiceId:x}"));
            return encoder.Encode(buffer, msbf, arg);
        }

        private Result<int> EncodeResponse(Span<byte> buffer, bool msbf, ControlResponse arg)
        {
            if (arg is ControlErrorNotSupported)
                return EmptyResponseEncoder.Encode(buffer, msbf, arg);

            IControlArgEncoder encoder = null;
            if (arg.ServiceId >= 0 && arg.ServiceId <= m_ResponseEncodersStandard.Length) {
                encoder = m_ResponseEncodersStandard[arg.ServiceId];
            } else if (arg.ServiceId >= 0xF00 && arg.ServiceId <= m_ResponseEncodersCustom.Length + 0xF00) {
                encoder = m_ResponseEncodersCustom[arg.ServiceId - 0xF00];
            } else if (arg.ServiceId is >= 0xFFF or < 0) {
                if (!m_ResponseSwInjection.TryGetValue(arg.ServiceId, out encoder))
                    encoder = EmptyResponseEncoder;
            }

            if (encoder is null)
                return Result.FromException<int>(new DltEncodeException($"Unknown response service {arg.DefaultType} with service {arg.ServiceId:x}"));
            return encoder.Encode(buffer, msbf, arg);
        }
    }
}
