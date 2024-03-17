namespace RJCP.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dlt;
    using Dlt.NonVerbose;
    using Dlt.Packet;
    using RJCP.CodeQuality.NUnitExtensions;

    public class DltFactory
    {
        public DltFactory(DltFactoryType factoryType)
        {
            FactoryType = factoryType;
        }

        public DltFactory(DltFactoryType factoryType, IFrameMap map)
        {
            FactoryType = factoryType;
            Map = map;
        }

        public DltFactory(DltFactoryType factoryType, TraceReaderFactory<DltTraceLineBase> factory)
        {
            FactoryType = factoryType;
            Factory = factory;
        }

        public DltFactoryType FactoryType { get; }

        private TraceReaderFactory<DltTraceLineBase> Factory { get; }

        private IFrameMap Map { get; }

        public Task<ITraceReader<DltTraceLineBase>> DltReaderFactory(Stream stream)
        {
            if (Factory is not null) {
                return Factory.CreateAsync(stream);
            }

            if (Map is not null) {
                switch (FactoryType) {
                case DltFactoryType.Standard:
                    return new DltTraceReaderFactory(Map).CreateAsync(stream);
                case DltFactoryType.File:
                    return new DltFileTraceReaderFactory(Map).CreateAsync(stream);
                case DltFactoryType.Serial:
                    return new DltSerialTraceReaderFactory(Map).CreateAsync(stream);
                default:
                    throw new InvalidOperationException($"Unknown Factory {FactoryType}");
                }
            }

            switch (FactoryType) {
            case DltFactoryType.Standard:
                return new DltTraceReaderFactory().CreateAsync(stream);
            case DltFactoryType.File:
                return new DltFileTraceReaderFactory().CreateAsync(stream);
            case DltFactoryType.Serial:
                return new DltSerialTraceReaderFactory().CreateAsync(stream);
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }
        }

        public DltPacketWriter.DltPacketBuilder Verbose(DltPacketWriter writer, DateTime storageTime, TimeSpan deviceTime, DltType msgType, string text)
        {
            switch (FactoryType) {
            case DltFactoryType.Standard:
                return writer.NewPacket().Line(deviceTime, msgType, text);
            case DltFactoryType.File:
                return writer.NewPacket().Line(deviceTime, msgType, text).StorageHeader(storageTime);
            case DltFactoryType.Serial:
                return writer.NewPacket().Line(deviceTime, msgType, text).SerialMarker();
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }
        }

        public DltPacketWriter.DltPacketBuilder Verbose(DltPacketWriter writer, DateTime storageTime, TimeSpan deviceTime, DltType msgType, int noar, byte[] payload)
        {
            switch (FactoryType) {
            case DltFactoryType.Standard:
                return writer.NewPacket().Line(deviceTime, msgType, noar, payload);
            case DltFactoryType.File:
                return writer.NewPacket().Line(deviceTime, msgType, noar, payload).StorageHeader(storageTime);
            case DltFactoryType.Serial:
                return writer.NewPacket().Line(deviceTime, msgType, noar, payload).SerialMarker();
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }
        }

        public DltPacketWriter.DltPacketBuilder Control(DltPacketWriter writer, DateTime storageTime, TimeSpan deviceTime, DltType msgType, byte[] payload)
        {
            switch (FactoryType) {
            case DltFactoryType.Standard:
                return writer.NewPacket().Control(deviceTime, msgType, payload);
            case DltFactoryType.File:
                return writer.NewPacket().Control(deviceTime, msgType, payload).StorageHeader(storageTime);
            case DltFactoryType.Serial:
                return writer.NewPacket().Control(deviceTime, msgType, payload).SerialMarker();
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }
        }

        public DltPacketWriter.DltPacketBuilder NonVerbose(DltPacketWriter writer, DateTime storageTime, TimeSpan deviceTime, byte[] payload)
        {
            switch (FactoryType) {
            case DltFactoryType.Standard:
                return writer.NewPacket().NonVerbose(deviceTime, payload);
            case DltFactoryType.File:
                return writer.NewPacket().NonVerbose(deviceTime, payload).StorageHeader(storageTime);
            case DltFactoryType.Serial:
                return writer.NewPacket().NonVerbose(deviceTime, payload).SerialMarker();
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }
        }

        public DltPacketWriter.DltPacketBuilder NonVerboseExt(DltPacketWriter writer, DateTime storageTime, TimeSpan deviceTime, byte[] payload)
        {
            switch (FactoryType) {
            case DltFactoryType.Standard:
                return writer.NewPacket().NonVerboseExt(deviceTime, payload);
            case DltFactoryType.File:
                return writer.NewPacket().NonVerboseExt(deviceTime, payload).StorageHeader(storageTime);
            case DltFactoryType.Serial:
                return writer.NewPacket().NonVerboseExt(deviceTime, payload).SerialMarker();
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }
        }

        public async Task WriteAsync(DltPacketWriter writer, string testName)
        {
            string ext;
            switch (FactoryType) {
            case DltFactoryType.Standard:
                ext = ".tcp.dlt";
                break;
            case DltFactoryType.Serial:
                ext = ".dls.dlt";
                break;
            case DltFactoryType.File:
                ext = ".dlt";
                break;
            default:
                throw new InvalidOperationException($"Unknown Factory {FactoryType}");
            }

            string dir = Path.Combine(Deploy.WorkDirectory, "dltout");
            string path = Path.Combine(dir, $"{testName}{ext}");
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            await writer.WriteAsync(path);
        }
    }
}
