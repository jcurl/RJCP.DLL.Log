# DLT Decoder Design <!-- omit in toc -->

This document covers the design of the decoder. It derives from the [TraceReader
Architecture](../../docs/Architecture.md).

The decoder is based on the [DLT Format](DLT.Format.md).

## Table of Contents <!-- omit in toc -->

- [1. DLT Trace Decoder](#1-dlt-trace-decoder)
  - [1.1. Purpose of the DLT Trace Decoder](#11-purpose-of-the-dlt-trace-decoder)
  - [1.2. Use Cases of the DLT Trace Decoder](#12-use-cases-of-the-dlt-trace-decoder)
  - [1.3. Identifying DLT Packets](#13-identifying-dlt-packets)
    - [1.3.1. Synchronizing Headers](#131-synchronizing-headers)
  - [1.4. The Main Decode Loop](#14-the-main-decode-loop)
    - [1.4.1. Searching for the Packet Start](#141-searching-for-the-packet-start)
    - [1.4.2. Searching for the Standard Header](#142-searching-for-the-standard-header)
    - [1.4.3. Searching for the Packet](#143-searching-for-the-packet)
    - [1.4.4. Discarding Data](#144-discarding-data)
    - [1.4.5. Decoding the Packet](#145-decoding-the-packet)
      - [1.4.5.1. Non-Verbose Messages](#1451-non-verbose-messages)
      - [1.4.5.2. Verbose Messages](#1452-verbose-messages)
      - [1.4.5.3. Control Messages](#1453-control-messages)
  - [1.5. Position within the Stream](#15-position-within-the-stream)
- [2. Trace Lines](#2-trace-lines)

## 1. DLT Trace Decoder

There are three trace decoders for DLT:

* TCP based streams, where a DLT packet starts with the Standard Header
* Storage files, where a DLT packet has a Storage Header describing the time
  stamp of each DLT packet when it was recorded.
* Serial based streams, which are like TCP streams, but each packet is prepended
  with the header `DLS\1`. This format is used by the Genivi DLT Viewer, but it
  is not specified by AutoSAR of itself.

![DltTraceDecoder](out/diagrams/DLT.DecoderClass/DLT.DecoderClass.svg)

### 1.1. Purpose of the DLT Trace Decoder

The DLT trace decoder is responsible for receiving parts of the byte stream and
decoding each individual DLT packet. It must look for the headers if required
(TCP streams have no fixed header structure, a valid packet can only be
determined by analysing the content). As the packet is decoded, the
`IDltLineBuilder` can then be used to construct a line.

### 1.2. Use Cases of the DLT Trace Decoder

The following use cases are considered of the trace decoder, that influences its
design:

* It shall try to be as fast as possible. Decoding packets shall be done in
  place, with copy operations only occurring as necessary (copy operations are
  needed to cache a partial line, ready for the next decode method call).
* Other decoders can be derived from this decoder. One example would be to
  calculate statistics. That means a new trace decoder, derived from
  `DltBaseTraceDecoder` and related classes, may provide its own implementation
  of `IDltLineBuilder` that returns a different object type than `DltTraceLine`
  (not shown in the diagram above, but constructed by the `DltLineBuilder`).
* It may be possible to inject functionality into the `DltBaseTraceDecoder`.
  These are called "filters", which are similar to plugins. The filter may parse
  the contents of the line, write the packet to a new stream, insert or modify
  data. It may take a raw packet and decode it based on a new format such as:
  * Decode file uploads within the DLT file;
  * Decode data streams, such as IPC and byte data;
  * etc.
* Support DLT encapsulated in other protocols, such as PCAP.
* Make it easy to write an encoder later. This impacts the design of the
  `DltTraceLine`.

### 1.3. Identifying DLT Packets

The packet structure is generally described in the diagram:

![DLT Packet Structure](out/diagrams/DLT.FormatCheatSheet/DLT.FormatCheatSheet.svg)

DLT streams sent over the network, known as TCP streams (but may also be part of
UDP packets) consist of the standard header which define the length of the
packet and the remaining data. There is no marker associated with the standard
header. A valid packet can only be detected by parsing the entire packet and
using heuristics (for example, assuming that the headers and the sum of all
arguments match the overall packet size). The DLT protocol implicitly makes the
assumption that data is not lost.

Packets that are stored on disk, usually recorded by a logger, consist
additionally of a storage header, which contains a searchable field `DLT\1`
followed by the standard header.

Packets that are received via unreliable streams, such as the serial port, may
be sent with a marker before each standard header, that can help synchronize in
case of data loss in the stream.

#### 1.3.1. Synchronizing Headers

The raw format assumes there is no data loss. This is generally incorrect, and
is observed that DLT streams can still be corrupted requiring some heuristics to
determine the start of each packet. Reasons for loss of data are:

* Incorrectly written software, which sends the DLT streams. Often buffer
  overflows in the applications that send streams lost data and then lose
  synchronicity with the stream.
* The transport may lose packets. TCP generally doesn't, but there is no such
  guarantee for UDP streams.
* The AutoSAR standard doesn't specify the transport, which may also be the
  UART, which is known to lose (or insert) individual bytes within the stream.

As such, when synchronizing with standard streams, the assumptions are:

* The packet is first assumed to be correct. The length field is obtained, which
  is an unsigned 16-bit value. Parsing can only start when enough bytes have
  been received. Should the stream be closed before all bytes are received, then
  one must start parsing, with the assumption of data corruption (e.g. there may
  be valid packets).
* In practice, the length of the DLT packet should be the same as the length of
  the headers and the arguments as part of the payload.
  * For verbose messages, this means implementing the specification to parse all
    argument types;
  * For non-verbose messages, this means having a valid file (usually a Fibex
    file) that additionally describes the dynamic data which is part of the
    payload.
* One can only know the length of the payload, if the payload is known. If an
  unknown control message, or an unknown argument type is provided, then the
  length of an argument is unknown and heuristics cannot be applied. By
  extension, if the message is non-verbose, the length of the data is unknown if
  no Fibex file, or the wrong Fibex file is provided. Thus checks for
  non-verbose generally should not be made, and an assumption of no data loss is
  required.

Otherwise, for files with a marker, such as those stored on disk, or with a
serial marker, one only needs to search for the next marker after each message.
Parsing the packet data should still apply heuristics, as there are still some
types of data corruption that could cause a significant amount of data to be
lost otherwise. For example, if one byte is lost so that the packet length is
significantly changed (so that the packet being parsed now appears much larger
due to data loss than what it is in reality), the heuristics applied can recover
up to 64kB of data in this case of error, instead of discarding it.

### 1.4. The Main Decode Loop

The `ITraceDecoder.Decode` method can accept any amount of data, and any data
that it doesn't "consume" must be internally cached. This internal cache should
be the maximum size of a DLT packet, which is 64kB plus the storage header of 16
bytes.

![Simple DLT Decode Loop](out/diagrams/DLT.DecoderSequence/DLT.DecoderSequence.svg)

The diagram above is very simplified, and contains no caching or optimizations
for brevity.

#### 1.4.1. Searching for the Packet Start

If when entering the `Decode` method the start of the packet start should be
scanned. This depends on the header:

* *TCP*: There is no header. Thus, no scanning is required, we have to assume
  that we're at the start of the packet.
* *Storage Header*: Scan for `DLT\1` to identify the start of the packet.
* *Serial Header*: Scan for `DLS\1` to identify the start of the packet.

If there is no cached data, then simply scan from the input buffer.

If there is already cached data, scanning is being done because data was
previously discarded. Then scanning must be done of the cached data, until the
start of the packet is identified. If the packet is not identified, then the
input buffer must be scanned further (taking care that the start of the packet
may be split between the two buffers).

#### 1.4.2. Searching for the Standard Header

The standard header has an offset in the frame, depending on the format:

* *TCP*: It is at the start, so the offset is zero
* *Storage Header*: The storage header has a fixed size of 16 bytes, including
  `DLT\1`.
* *Serial Header*: The standard header has an offset of 4 bytes, immediately
  after the `DLS\1`.

If the input has not enough data, i.e. less than the offset plus the length of
the standard header, then data should be cached. The next time the `Decode`
method is called, the check for the Standard Header should be repeated, until
there is at least enough data.

| Storage Format | Minimum Length |
| -------------- | -------------- |
| TCP            | 0 + 4 = 4      |
| Storage        | 16 + 4 = 20    |
| Serial         | 4 + 4 = 8      |

The first term is the extra header, while the constant 4 is the minimum size of
the standard header.

#### 1.4.3. Searching for the Packet

Now that the standard header minimum is available, one can read the length of
the packet and calculate the minimum length of the packet.

The minimum length is expected to be:

* 4 bytes for the standard header, to the length
* 4 bytes for the ECU ID if present (WEID)
* 4 bytes for the Session ID if present (WSID)
* 4 bytes for the Time Stamp if present (WTMS)

The length field in the packet does not include the storage header.

Thus, if the length field is less than the minimum, we can conclude the packet
is not a valid DLT packet, it can be discarded (the minimum amount of data), and
the search starts again.

A second heuristic that can be performed is to ensure the VERS field is of the
correct value. Any value other than `1` is the wrong version and not a valid DLT
packet also.

#### 1.4.4. Discarding Data

The amount of data that can be discarded in case of an invalid packet being
detected depends on the protocol being scanned. There are no repeating
characters in the storage or serial header, so for these formats, it's safe to
discard 4 bytes. For the TCP stream, the minimum bytes scanned should be
discarded, which is 1 byte.

When discarding data, it is not correct to discard the cached data. The next
packet header may already be within the cache.

For this reason, it is recommended that the cache size be double the maximum
packet length. This is an optimization that prevents having to move all bytes in
the cache to the left to discard data. Instead, data can remain as it is, unless
the start after discarding the data is more than the maximum size of a single
packet.

(The cache size doesn't need to be double, it just needs to be more than a
single packet, and it must be made sure every time there's a discard, that a
shift is only done if there is less data to the right of the start than what one
single maximum packet can contain.)

Of course, if we're discarding data from the cache, but it turns out that the
cache is a subset of the input buffer, then the complete cache can be discarded
and the start to the input buffer can be reset. This makes scanning for the
start of the frame simpler.

#### 1.4.5. Decoding the Packet

Once the Standard Header has been identified, the correct number of bytes are
available, the packet can now be decoded:

* The HTYP field of the Standard Header (ECU, SEID, TMSP) if available.
* The extended header if UEH is set. The extended header is always expected to
  be present for verbose messages and control messages
* The arguments if the UEH is set.

##### 1.4.5.1. Non-Verbose Messages

If the extended header is not available, the packet is a non-verbose message.
This implementation does not contain a FIBEX decoder, so the packet is created,
but there are no arguments provided.

##### 1.4.5.2. Verbose Messages

The extended header must be available, and the VERB bit is set (the MSTP value
may not be 3). Each argument shall be parsed and added to the message.

##### 1.4.5.3. Control Messages

The extended header must be available. If the MSTP value is 3, the verbose bit
VERB is ignored. The message is assumed to be a control message, and there are
no arguments.

It has been observed that not all implementations writing DLT strictly conform
to the specification given in the AutoSAR DLT PRS standard.

### 1.5. Position within the Stream

It is important that the position of each decoded packet is made avaialble with
the decoded `DltBaseTraceLine`. This will help debug corrupted packets, that a
user can easily refer to the packet with a hex-editor.

There may be a small penalty when calculating the position of the stream.

Further, when calculating the position of the stream, one cannot assume that
each call to the `Decode` method doesn't skip bytes. This can happen when
decoding encapsulated streams, such as DLT from a PCAP file, where the DLT is
encapsulated within UDP packets.

## 2. Trace Lines

All DLT trace lines are derived from the `DltTraceLineBase`.

![DLT Trace Lines](out/diagrams/DLT.TraceLine/DLT.TraceLine.svg)

Other trace lines may be generated as required from the decoder.

It is important to recognize here that a DLT trace line, and a DLT Control trace
line are different.

* A Control Trace Line doesn't have arguments
* A DLT Trace Line may contain arguments, either decoded from the packet data,
  or from dynamic data and an external file describing how to parse the dynamic
  data.

The `DltTraceDecoder` uses provides the data it parses to its `IDltLineBuilder`
to construct the trace line it will return later. This allows the base
implementation `DltBaseTraceDecoder` to be extended to create other types of
trace lines, without having to encapsulate (wrap) or create temporary copies.
