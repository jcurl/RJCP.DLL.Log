# RJCP.Diagnostics.Log.Dlt Project <!-- omit in toc -->

- [1. DLT Trace Reader](#1-dlt-trace-reader)
  - [1.1. Reading](#11-reading)
  - [1.2. Constraints](#12-constraints)
  - [1.3. Control Messages](#13-control-messages)
- [2. DLT Trace Writer](#2-dlt-trace-writer)
- [3. Release History](#3-release-history)
  - [3.1. Version 0.8.1](#31-version-081)
  - [3.2. Version 0.8.0](#32-version-080)

## 1. DLT Trace Reader

The DLT Trace Reader implements version 1 of the [AutoSAR Diagnostics Log and
Trace Protocol Specification
R20-11](https://www.autosar.org/fileadmin/standards/R20-11/FO/AUTOSAR_PRS_LogAndTraceProtocol.pdf).

It is used by the
[DltDump](https://github.com/jcurl/RJCP.DLL.Log/tree/master/apps/DltDump)
application.

### 1.1. Reading

It implements reading streams from:

- The serial port (e.g. using
  [RJCP.SerialPortStream](https://github.com/jcurl/RJCP.DLL.SerialPortStream))
- TCP Networks
- From disk

It supports the AutoSAR DLT version 1 packet, in three variants:

- As a network stream, i.e. in the format streamed from logging devices
- Stored on disk, with a storage header
- Sent over serial port with an (undocumented) serial header, in the same format
  as the [Covesa DLT Viewer](https://github.com/COVESA/dlt-viewer).

It can interpret Non-Verbose streams based on the FIBEX file format and convert
them to a verbose string.

It supports most of the data types defined in the AutoSAR standard, including:

- Boolean `BOOL`
- Signed Int `SINT`
- Unsigned Int `UINT`
  - With additional Integer Coding (Hex, Octal, Binary, Decimal)
- Float `FLOA`
- Raw `RAWD`
- Strings `STRG`
  - With String Coding ASCII and UTF-8 (`SCOD`) being stricter than Covesa.

Not implemented are:

- Arrays `ARAY`
- Structures (`STRU`)
- Variable Info (`VARI`)
- Fixed Point (`FIXP`)
- Trace Info (`TRAI`)

It adds extra heuristics to try and determine corrupted data, by checking the
length of verbose payloads with the packet payload (looking for errors in
datastreams), and tries to synchronise where possible.

The output of the string tries to be compatible with the same output given by
the [Covesa DLT Viewer](https://github.com/COVESA/dlt-viewer).

### 1.2. Constraints

Various constraints are added for fast filtering of DLT lines.

- `DltAppId`
- `DltCtxId`
- `DltEcuId`
- `DltIsControl`
- `DltIsVerbose`
- `DltMessageId`
- `DltMessageType`
- `DltSessionId`

These can be used in addition the constraints defined by the
`RJCP.Diagnostics.Log` package.

### 1.3. Control Messages

The reading and interpreting of various control messages over multiple versions
of the standard is supported (so long as the control message identifiers don't
conflict).

The `X` indicates the control message is implemented.

| Service Id            | Name                         | Request | Response | Standard   |
| --------------------- | ---------------------------- | :-----: | :------: | ---------- |
| `0x01`                | SetLogLevel                  |   R/W   |   R/W    | PRS 1.4.0  |
| `0x02`                | SetTraceStatus               |   R/W   |   R/W    | PRS 1.4.0  |
| `0x03`                | GetLogInfo                   |   R/W   |   R/W    | PRS 1.4.0  |
| `0x04`                | GetDefaultLogLevel           |   R/W   |   R/W    | PRS 1.4.0  |
| `0x05`                | StoreConfiguration           |   R/W   |   R/W    | PRS 1.4.0  |
| `0x06`                | ResetToFactoryDefault        |   R/W   |   R/W    | PRS 1.4.0  |
| `0x07`                | SetComInterfaceStatus¹       |         |          | SWS 4.2.2  |
| `0x08`                | SetComInterfaceMaxBandwidth¹ |         |          | SWS 4.2.2  |
| `0x09`                | SetVerboseMode¹              |   R/W   |   R/W    | SWS 4.2.2  |
| `0x0A`                | SetMessageFiltering          |   R/W   |   R/W    | PRS 1.4.0  |
| `0x0B`                | SetTimingPackets             |   R/W   |   R/W    | SWS 4.2.2  |
| `0x0C`                | GetLocalTime¹                |   R/W   |   R/W    | SWS 4.2.2  |
| `0x0D`                | SetUseECUID¹                 |   R/W   |   R/W    | SWS 4.2.2  |
| `0x0E`                | SetUseSessionId¹             |   R/W   |   R/W    | SWS 4.2.2  |
| `0x0F`                | UseTimestamp¹                |   R/W   |   R/W    | SWS 4.2.2  |
| `0x10`                | UseExtendedHeader¹           |   R/W   |   R/W    | SWS 4.2.2  |
| `0x11`                | SetDefaultLogLevel           |   R/W   |   R/W    | PRS 1.4.0  |
| `0x12`                | SetDefaultTraceStatus        |   R/W   |   R/W    | PRS 1.4.0  |
| `0x13`                | GetSoftwareVersion           |   R/W   |   R/W    | PRS 1.4.0  |
| `0x14`                | MessageBufferOverflow¹       |   R/W   |   R/W    | SWS 4.2.2  |
| `0x15`                | GetDefaultTraceStatus        |   R/W   |   R/W    | PRS 1.4.0  |
| `0x17`                | GetLogChannelNames           |         |          | PRS 1.4.0  |
| `0x16`                | GetComInterfaceStatus¹       |         |          | SWS 4.2.2  |
| `0x17`                | GetComInterfaceNames²        |         |          | SWS 4.2.2  |
| `0x18`                | GetComInterfaceMaxBandwidth¹ |         |          | SWS 4.2.2  |
| `0x19`                | GetVerboseModeStatus¹        |   R/W   |   R/W    | SWS 4.2.2  |
| `0x1A`                | GetMessageFilteringStatus¹   |   R/W   |   R/W    | SWS 4.2.2  |
| `0x1B`                | GetUseECUID¹                 |   R/W   |   R/W    | SWS 4.2.2  |
| `0x1C`                | GetUseSessionID¹             |   R/W   |   R/W    | SWS 4.2.2  |
| `0x1D`                | GetUseTimestamp¹             |   R/W   |   R/W    | SWS 4.2.2  |
| `0x1E`                | GetUseExtendedHeader¹        |   R/W   |   R/W    | SWS 4.2.2  |
| `0x1F`                | GetTraceStatus               |   R/W   |   R/W    | PRS 1.4.0  |
| `0x20`                | SetLogChannelAssignment      |         |          | PRS 1.4.0  |
| `0x21`                | SetLogChannelThreshold       |         |          | PRS 1.4.0  |
| `0x22`                | GetLogChannelThreshold       |         |          | PRS 1.4.0  |
| `0x23`                | BufferOverflowNotification   |   R/W   |   R/W    | PRS 1.4.0  |
| `0x24`                | SyncTimeStamp                |   R/W   |   R/W    | PRS R19-11 |
| `0xF01`               | Unregister Context           |   N/A   |   R/W    |            |
| `0xF02`               | Connection Info              |   N/A   |   R/W    |            |
| `0xF03`               | TimeZone Info                |   N/A   |   R/W    |            |
| `0xF04`               | Marker                       |   N/A   |   R/W    |            |
| `0xFFF`..`0xFFFFFFFF` | CallSWCInjection             |    R    |    R     | PRS 1.4.0  |

## 2. DLT Trace Writer

The `DltTraceWriterFactory` allows to create a `DltTraceWriter` that can be used
to write a trace line to disk. This allows one to implement tooling to read log
files (e.g. event logs, journals) and write out as DLT (for example, over the
network).

It supports writing verbose and control messages, as DLTv1 packets.

It does *not* support writing non-verbose messages.

If access to the raw payload is needed, extend the
`DltTraceDecoderBase.CheckLine` in your own class. The payload is not exposed to
a `DltTraceLine` to avoid an unnecessary copy. This could allow reading a packet
that is a non-verbose raw message, and instead of using the DltWriter, write the
original line (for example, this filtering is done by DltDump).

## 3. Release History

### 3.1. Version 0.8.1

Bugfixes:

- Workaround corrupted verbose with no arguments (DOTNET-921)

Quality:

- Add README.md to NuGet package (DOTNET-816, DOTNET-817, DOTNET-932)
- Mark common secondary warnings as Verbose (DOTNET-922)
- Upgrade to .NET 6.0 (DOTNET-936, DOTNET-942, DOTNET-943, DOTNET-945, DOTNET-951)

### 3.2. Version 0.8.0

- Initial Release
