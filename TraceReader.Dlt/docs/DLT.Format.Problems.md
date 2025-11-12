# DLT Protocol Issues <!-- omit in toc -->

The DLT format is not a well structured protocol, it does not follow the
guidelines of OSI layering. This paper tries to explain why this is a problem,
and what needs to change in the protocol (which makes it incompatible to
existing implementations).

## Table of Contents <!-- omit in toc -->

- [1. Weaknesses in the Robustness of the Protocol](#1-weaknesses-in-the-robustness-of-the-protocol)
  - [1.1. The Structure over Serial or Ethernet - Unique Packet Identification](#11-the-structure-over-serial-or-ethernet---unique-packet-identification)
  - [1.2. Extensions to the Standard/Base Header Option Bits](#12-extensions-to-the-standardbase-header-option-bits)
  - [1.3. Reserved or Unimplemented Fields (Verbose Argument Types)](#13-reserved-or-unimplemented-fields-verbose-argument-types)
- [2. Causes for Corruption](#2-causes-for-corruption)
- [3. Possible Decoder Implementations](#3-possible-decoder-implementations)
  - [3.1. Assume the Length of the Packet is Correct](#31-assume-the-length-of-the-packet-is-correct)
  - [3.2. Protocol Knowledge and Parsing - The Standard Header](#32-protocol-knowledge-and-parsing---the-standard-header)
  - [3.3. Protocol Knowledge - Using More Fields](#33-protocol-knowledge---using-more-fields)
  - [3.4. Control Messages](#34-control-messages)
  - [3.5. Non-Verbose Messages](#35-non-verbose-messages)
- [4. Fixing the Protocol](#4-fixing-the-protocol)
  - [4.1. Use a Marker is not Enough](#41-use-a-marker-is-not-enough)
  - [4.2. Proposed Extension - Extend with a FCS](#42-proposed-extension---extend-with-a-fcs)
  - [4.3. Allow Extensions Compatible with Older Decoders](#43-allow-extensions-compatible-with-older-decoders)
  - [4.4. Allow Types to be Ignored](#44-allow-types-to-be-ignored)
  - [4.5. Layer 4 Timeout Detection](#45-layer-4-timeout-detection)
  - [4.6. NonVerbose Format](#46-nonverbose-format)

## 1. Weaknesses in the Robustness of the Protocol

### 1.1. The Structure over Serial or Ethernet - Unique Packet Identification

Take the protocol format:

![DLT Format](out/diagrams/DLT.FormatCheatSheet/DLT.FormatCheatSheet.svg)

Ignore the Storage Header, as this is recorded by the logging equipment and is
not transmitted by the device doing the logging, and as such, can be considered
outside of the scope of the protocol.

If it can be guaranteed that each packet is perfectly written, so that the
length of each packet is correct according to the wire, an implementation can be
optimized to assume no errors and be able to reconstruct the boundaries of each
packet.

Otherwise, non-standard heuristics must be applied to check if the length of the
packet is correct. These heuristics are problematic on their own as they may not
be accurate, or an extension of the standard at a future date renders the
heuristics problematic or invalid.

Such an example of heuristics implemented are:

- Checking the calculated length matches the said length. There is no easy way
  to calculate the length of the protocol.
- Fields that are reserved might need to be considered. Some implementations
  expect NOAR for non-verbose messages are zero, but many implementations do not
  correctly implement the standard.
- Applying heuristics, then looking for the start of the next packet, is itself
  a heuristic. Except for the storage header, there is no standard frame marker
  to identify the start of a next packet, except for assuming every byte is a
  valid start and requiring the application of the heuristic again. Such an
  example where a heuristic can fail is the embedded application of a DLT line
  in the payload of another line. This could occur, for example, when uploading
  a DLT file from the ECU using a file transfer, and be mistakenly attributed as
  a line emitted intentionally from the ECU at the top level, not wrapped.

### 1.2. Extensions to the Standard/Base Header Option Bits

This applies for the DLTv2 Base Header (because all bits in DLTv1 Standard
Header are defined).

If a new option field is defined in the Base Header, that requires an appendage
of data in the Base Header, it would no longer be possible to decode the Base
Header to determine the start location of the Extended Header or the Payload.
Should the standard ever be extended, tooling must be updated to avoid data
corruption.

The Options fields in PPP Link Layer, or the Options fields in a PCAPng file
provide a consistent example where this is not a problem.

### 1.3. Reserved or Unimplemented Fields (Verbose Argument Types)

The protocol has no standard mechanism to determine the structure of arguments.
If a data stream contains a type which is not implemented, then it is impossible
to know the length of that type and skip over to start processing other types.

This in practice prevents ECUs from implementing arrays and structs when their
tools for reading those streams don't implement arrays and structs. There is no
length field in a standard location that allows ignoring that particular
argument and identifying the start of the next argument.

If a standard defines a new type in addition, then decoders cannot interpret any
arguments occurring afterwards, effectively corrupting the encoded DLT line.

## 2. Causes for Corruption

In reality, one cannot assume that each packet is perfectly written. The DLT
standard does not discuss the transport protocol.

The following issues are well known:

- Serial port transport, may result in bit flips, bytes lost, or occasionally
  bytes are injected (typically 0xFF or 0x00). This changes the length or the
  meaning of the content of the packet. The length of the packet is no longer
  reliable.
- Encapsulation within UDP packets may result in larger chunks of data being
  lost, up to 1472 bytes. If a UDP packet is lost, and the DLT packets are
  fragmented over multiple UDP packets (as opposed to IPv4 fragmentation),
  resynchronisation must occur.
  - Valid implementations may use IPv4 fragmentation (which the reassembly of
    the packet would discard all fragmented packets) and thus there is no need
    for a FCS in DLT. However, there is motivation to not use IPv4 fragmentation
    and such implementations simply split their DLT payload at arbitrary points
    and send the data over multiple UDP packets. Given that each UDP packet is
    valid with valid checksums at L4 (UDP), the loss of a single UDP packet
    corrupts the DLT stream
- Buffer handling errors in implementations when sending data may result in
  corruption of the DLT packets before the data is even sent. In this case, an
  arbitrary amount of data may be lost anywhere in the stream, even a TCP stream
  which would otherwise be considered reliable. That has been observed in
  released versions of the Covesa dlt-daemon, resulting in storage headers at
  the wrong location in the stream.

Due to the lack of the following in the packet description:

- there being no well defined boundaries between the packets; or
- frame checksums to test the integrity of the data being decoded

a decoder cannot reliably decode data in a stream after an error has occurred.
How it does this is completely implementation dependent.

## 3. Possible Decoder Implementations

### 3.1. Assume the Length of the Packet is Correct

A decoder may decide that the length is always correct. This is a risk, as
described above in the possible (real-world and observed) error conditions. The
likelihood after decoding of resynchronising to the data stream as intended by
the device emitting logs is very small, approximately 1 in 65526 (calculated
that most of the data in the stream is random data, and is interpreted as a
length which happens to perfectly align with a valid next packet).

We can see this is not a good solution to packet parsing as it is expected a
significant amount of data is to be lost.

### 3.2. Protocol Knowledge and Parsing - The Standard Header

The mechanism this decoder uses to improve the likelihood of synchronising with
a new packet in case of an error includes field validation. If a field is
determined as impossible, it is considered that the point in the stream which is
used to start decoding the packet is invalid, and decoding is advanced by one
byte, and is tried again.

The fields that can be used initially are:

- The DLT version is expected to be 1. If it isn't, it is assumed data
  corruption. If a new version is invented, this decoder either shouldn't be
  used, or the chances of recovery is reduced depending on how the new protocol
  version is defined (if it fixes these problems or not by defining at least a
  frame checksum or some other similar marker to determine the validity of a
  packet once it's decoded).
- The minimum length of a packet must be at least the minimum length as defined
  by the standard header, and the presence of an extended header if it should be
  present. If the packet is less than the protocol bits specify, then the packet
  is invalid.

This mechanism is a heuristic only, and is not a foolproof means of determining
that a packet is valid or not.

For example, let's assume that the first byte of this message is lost.

| Bytes                     | Description                               |
| ------------------------- | ----------------------------------------- |
| `3d`                      | Version 1, Session Id, ECU Id, Time Stamp |
| `7f`                      | Counter 127                               |
| `00 28`                   | Length                                    |
| `45 43 55 31`             | ECU1                                      |
| `00 00 00 32`             | SessionId = 50                            |
| `00 00 30 20`             | Time Stamp                                |
| `41`                      | Message Info (Verbose, LOG_INFO)          |
| `01`                      | One Argument                              |
| `41 50 50 31`             | APP1                                      |
| `43 54 58 31`             | CTX1                                      |
| `00 82 00 00`             | Argument #1 String                        |
| `08 00`                   | Argument #1 String length 8 bytes         |
| `4d 65 73 73 61 67 65 00` | "Message"                                 |

Consider now the first byte is lost, as could be on a serial stream.

- `7f` - Version = 3 which is invalid
- `00` - Version = 0 which is invalid
- `28` - Version = 1 which is now valid. The counter is now 0x45, length is 0x4355 bytes.

We've now found a packet that has the correct version, a valid length. We might
now discard a significant amount of data which prevents analysis, simply through
one byte lost.

What happens if we didn't check the length? We'd see a packet length of 0x2845,
again, a significant amount of data loss. So ignoring the fields doesn't work
either.

### 3.3. Protocol Knowledge - Using More Fields

The only solution is to continue parsing the data, validating each field, and
deciding if there is a protocol error and then considering the packet as
invalid.

This means that a parser must have as much knowledge about the protocol as
possible, and assume that every implementation implements this protocol and
doesn't introduce its own errors. Possible additional heuristics include:

- Parse extra fields, such as the Message Info;
- For verbose messages, check the type info for each argument
- Sum the lengths of each argument and ensure that it matches also the packet
  length

If any writer doesn't conform properly to the DLT PRS standard, decoders have
less heuristics they can use for reliable reconstruction in case of a fault in
the input stream.

Further, to know the length of each argument implies implementing a decoder for
each argument, as the argument length is not encoded in a standard way. For
example, a string is the length TYLE as undefined, a 16-bit value for the
length, assuming a NUL terminator.

But if an unknown argument type is found, which may be defined in a newer
version of the standard, no arguments afterwards can be properly decoded as it
is not known where the next argument starts.

### 3.4. Control Messages

It has been observed that some implementations do not follow the standard when
writing control messages (the lengths of the control messages do not match that
defined in the DLT PRS). For these messages to be decoded, control message
lengths must be ignored.

Further, newer standards have removed messages that might still be used by older
implementations and therefore, for practical reasons, should still be decoded.

### 3.5. Non-Verbose Messages

If no Fibex file is provided to provide information on how to decode the dynamic
data, no heuristics can be applied to a non-verbose message to estimate the
correctness of each packet and its boundaries.

If a Fibex file is provided, it may not have any errors, and must also perfectly
match for heuristics to be applied. If the wrong dynamic data is provided in the
Fibex file (or specifically, the length of the dynamic data is different to that
described by the Fibex file), heuristics may discard an actually correct packet.

An implementor of the DLT decoder must decide if the Fibex file is wrong (then
assume the packet length is correct), or there was corruption in the data and
ignore the packet length. One can see that this is impossible for a decoder to
make such an arbitrary choice.

## 4. Fixing the Protocol

### 4.1. Use a Marker is not Enough

The DLT protocol already defines a workaround to the problem of decoding that
can significantly improve the chances of a decoder synchronising data, but it is
still easy to circumvent and in some practical cases, provide unintended (or
maliciously invalid) data possibly confusing (or tricking) the user.

There is the definition of the Storage Header, which has a fixed marker `DLT\1`
present. In case of any problems observed through heuristics described in the
previous section, we can now look for a unique string `DLT\1` and then start
decoding from there.

While not defined in the AutoSAR DLT PRS, some implementations send the marker
`DLS\1` to delineate packets when transmitted over unreliable streams, such as a
serial port. This marker immediately precedes the standard header and can
significantly increase the likelihood of resynchronising packets.

Unfortunately, there is a common use case where this causes problems. If any
packet transmitted happens to have the string marker within its payload, that
may be incorrectly interpreted as a valid DLT packet. Situations where this
commonly occur is where devices upload "DLT" logs with a storage header in
uncompressed or unencrypted form, within the payload of another DLT packet. Then
effectively it's no longer possible to distinguish between the two sets of logs.

Thus, while this is a "cheap" solution, it is not sufficient so long as the
markers in the DLT stream are the same as the markers of the encapsulated
packets.

### 4.2. Proposed Extension - Extend with a FCS

The protocol should be extended so the last two bytes (or four bytes) of a DLT
packet should contain a frame checksum. The protocol should be incremented to
indicate the presence of a new protocol.

By extending the last two bytes of the packet as such, existing implementations
with the DLT version 1 could still be made to work, as they could effectively
ignore the last two bytes, depending on their heuristics (for example, the
Covesa DLT viewer does not check the length of the packet that it matches the
length of all the arguments).

Then for more advanced implementations that recognise the next version, they
can apply the content of the packet against the checksum, and if valid, treat
the message as correct.

In case of an encapsulated DLT packet, the contents of that packet would only be
shown if the outer frame is corrupted (and then the inner frame would be
considered valid). After the encapsulated packet is decoded and shown, the next
valid packet would be found and displayed again, minimizing this impact and
confusion.

To avoid even that problem, one could define the concept of sessions, where each
packet has a value which is the same for a single stream, so that the assumption
an encapsulated packet has a different session can be ignored (or identified as
such, making it clear there are multiple recordings in the log).

Negatively, this proposal necessarily increases the size of a DLT message, which
may not be desirable for small embedded devices that are memory constrained when
sending logs. The smallest possible non-verbose packet is:

- 8 bytes containing only a message identifier that is mapped to a static string
- 10 bytes if a 16-bit integer is transmitted in addition as dynamic data
- add 4 bytes if a timestamp when that message is transmitted is needed (which
  is very common).

Arguably, the minimum useful size of a DLT non-verbose message is 14 bytes,
aligning it to 16 bytes would likely provide a performance optimization for many
embedded ECU's using DMA.

### 4.3. Allow Extensions Compatible with Older Decoders

Options should be of a standard format, such that the structure of a packet is
decoupled from the content of the packet.

There is no reason to treat option bits separately between the Standard
Header/Base Header and the Extended Header. It should consistently apply a
length field, an option type and the option data. A good example of how this is
done is in the PCAPng data format.

In this way, if an option is not supported, it can be skipped, increasing the
likelihood that a packet can be decoded. It is fully extensible without the need
of bit fields (which in DLTv1 has no more room to expand and in DLTv2 can have
at most 29 defined options).

Even in DLTv2, the Extension Header schema inconsistently applies a length
field. It argues that the length isn't needed for types that are fixed in size.
The ordering of the fields matter. It prevents custom extensions such that
standard decoders can no longer interpret the extended header as fields are
added which are not in the standard.

### 4.4. Allow Types to be Ignored

The Verbose type information has encoded length. This is especially difficult to
determine the position in the stream for all type arguments if there are complex
type arguments present.

Each Verbose type argument should have a standard way for determining the length
(not encoded in a bit field). So if an argument is not supported, a decoder can
skip over that specific argument (or even present this in its UI has "raw" data)
and continue decoding the remaining arguments. This allows user extension at any
time for new argument types (e.g. an 80-bit IEEE 754 is a common type).

### 4.5. Layer 4 Timeout Detection

One common transport protocol is TCP. This is a connection oriented protocol and
presents problems not observed in connectionless / packet oriented protocols
(serial or UDP).

The device under test implements the TCP server, and the DLT decoder connects.
If the device under test performs a restart (which is quite common given that
the devices are being tested during their development phase), the client will
not receive notification of the loss of the connection (an absence of a TCP RST
or a TCP FIN). An Operating System might not notice this for extended periods of
time.

As such, the client must implement an application level time out, and usually
must be quite short (the order of 1-5 seconds).

The Covesa implementation provides a CONTROL_TIME implementation (undocumented)
that emits such messages at periodic intervals if no normal debugging message
was sent within this time, to effectively tell the receiver the connection is
alive.

This should be formally documented (which is still missing as of R22-11 in
DLTv2).

One benefit of formally documenting this is reserving the messages such they
aren't reused for custom implementations, and do not need to be recorded in the
final DLT stream.

### 4.6. NonVerbose Format

The Non-Verbose Format is not completely specified in an Open manner. Both the
FIBEX and the ARXML standards are locked behind a paywall and it is impossible
for an open implementation to account for these standards without violating
intellectual property.

The FIBEX implementation can be decoded in a manner compatible with the
dlt-viewer from Covesa. The FIBEX implementation ignores the uniqueness
requirement of ID and ID-REF fields of the XML standard.
