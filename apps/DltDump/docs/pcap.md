# PCAP File Description <!-- omit in toc -->

This document contains a summary of the PCAP formats, to help with the
implementation for the decoder when reading PCAP or PCAPNG files.

For reference, the specifications can be found at:

- [PCAP](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcap.html)
- [PCAP-NG](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcapng.html)

## Table of Contents <!-- omit in toc -->

- [1. Notation](#1-notation)
- [2. PCAP Format](#2-pcap-format)
  - [2.1. Header](#21-header)
    - [2.1.1. PCAP Link Type Field](#211-pcap-link-type-field)
  - [2.2. Frame](#22-frame)
- [3. PCAP-NG Format](#3-pcap-ng-format)
  - [3.1. General Block Structure](#31-general-block-structure)
  - [3.2. File Layout](#32-file-layout)
  - [3.3. Options](#33-options)
  - [3.4. Section Header Block (SHB)](#34-section-header-block-shb)
  - [3.5. Interface Description Block (IDB)](#35-interface-description-block-idb)
  - [3.6. Enhanced Packet Block (EPB)](#36-enhanced-packet-block-epb)
- [4. Link Type Frames](#4-link-type-frames)
  - [4.1. Ethernet Frame](#41-ethernet-frame)
  - [4.2. Linux Cooked Frame](#42-linux-cooked-frame)
- [5. Protocol](#5-protocol)
- [6. TECMP Protocol](#6-tecmp-protocol)
  - [6.1. TECMP Header](#61-tecmp-header)
  - [6.2. TECMP Payload](#62-tecmp-payload)
- [7. IPv4 Protocol](#7-ipv4-protocol)
  - [7.1. IP Fragmentation](#71-ip-fragmentation)
- [8. UDP Protocol](#8-udp-protocol)

I wrote this document to help write the software and have a fast reference for
decoding PCAP frames. This might be useful for similar implementations.

## 1. Notation

From left to right, the offset increments, so that:

```text
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |       0       |       1       |       2       |       3       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

byte zero is on the left, byte 3 is on the right. For a 32-bit value, it may be
written as:

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |       0       |       1       |       2       |       3       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

where in this notation (as given in the PCAP documentation, as well as RFQ
documentation), the lower the bit number, the more significant it is. e.g. bit 0
is the MSB of byte 0, and increments from left to right. Bit 15 is the lowest
bit of byte 1.

## 2. PCAP Format

This is the simplest to implement. To read the blocks, the bytes are the offset
into the file or the current pointer in the file.

### 2.1. Header

The header of the packet contains:

```text
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                          Magic Number                         |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |          Major Version        |          Minor Version        |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |                           Reserved1                           |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |                           Reserved2                           |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 |                            SnapLen                            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   20 |               LinkType and additional information             |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

The Magic Number defines the byte ordering and the time resolution:

| Magic Number | Little Endian | Time Resolution |
| ------------ | ------------- | --------------- |
| `0xA1B2C3D4` | False         | microseconds    |
| `0xA1B23C4D` | False         | nanoseconds     |
| `0xD4C3B2A1` | True          | microseconds    |
| `0x4D3CB2A1` | True          | nanoseconds     |

The other fields:

- The Major Version is expected to be 2.
- The Minor Version is expected to be 4.
- Reserved1 and Reserved2 are ignored.
- SnapLen is the maximum length for each packet (the snapshot length). No packet
  in the file is larger than SnapLen.

#### 2.1.1. PCAP Link Type Field

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   20 |FCS len|R|P|     Reserved3     |        Link-layer type        |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

The current implementation doesn't know how to interpret the frame checksum. If
bit `P` is set, then the frame checksum length is calculated, but is not used in
this implementation.

The Link-layer type supported is one of two values:

- `LINKTYPE_ETHERNET` = 1
- `LINKTYPE_LINUX_SLL` = 113

### 2.2. Frame

Each frame follows the Link-layer type. It is formatted as:

```text
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                      Timestamp (Seconds)                      |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |            Timestamp (Microseconds or nanoseconds)            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |                    Captured Packet Length                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |                    Original Packet Length                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 /                                                               /
      /                          Packet Data                          /
      /                  variable length, not padded                  /
      /                                                               /
```

A frame is only decoded if the captured packet length and the original packet
length are the same. Else data is lost and the frame is ignored.

The seconds value is a 32-bit unsigned integer that represents the number of
seconds that have elapsed since 1970-01-01 00:00:00 UTC, and the microseconds or
nanoseconds value represents the number of microseconds or nanoseconds that have
elapsed since that seconds.

## 3. PCAP-NG Format

This format is much more detailed and supports many more recording formats, but
with it comes much more complexity and many more use cases to check for in
implementation.

### 3.1. General Block Structure

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                          Block Type                           |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 /                          Block Body                           /
      /              variable length, padded to 32 bits               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

The fields have the following meaning:

- Block Type (32 bits): a unique unsigned value that identifies the block.
- Block Total Length (32 bits): an unsigned value giving the total size of this
  block in octets.
- Block Body: content of the block.
- Block Total Length: total size of this block, in octets. It's duplicated if
  the file needs to be read in reverse (e.g. implementing windowing, which is
  not required by this project).

The types of Blocks are:

- `0x0A0D0D0A`: Section Header Block
- `0x00000001`: Interface Description Block
- `0x00000006`: Enhanced Packet Block. If this appears in a file, an Interface
  Description Block is also required, before this block.

The length is a signed integer type, in the endianness described by the SHB
Byte-Order Magic. It is the size of the block from byte 0, until the end,
including the length at the end. The length may be signed value -1, which means
to interpret the contents of the block to determine the length. This software
will not support that mode.

### 3.2. File Layout

A typical file consists of:

```text
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
| SHB v1.0  |                      Data                         |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

Multiple Section Header Blocks (SHB) can be defined in a file.

```text
|--   1st Section   --|--   2nd Section   --|--  3rd Section  --|
|                                                               |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
| SHB v1.0  |  Data   | SHB V1.1  |  Data   | SHB V1.0  |  Data |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

The Interface Description Block (IDB) can occur anywhere, but must be before the
packet that uses it.

### 3.3. Options

All the block bodies MAY embed optional fields. each tool can either read the
content of the optional fields (if any), or skip some of them or even all at
once. The DltDump tooling in this project ignores the options.

### 3.4. Section Header Block (SHB)

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                   Block Type = 0x0A0D0D0A                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |                      Byte-Order Magic                         |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |          Major Version        |         Minor Version         |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 |                                                               |
      |                          Section Length                       |
      |                                                               |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   24 /                                                               /
      /                      Options (variable)                       /
      /                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

The section header block is intended to be endianness agnostic. The `Byte-Order
Magic` contains the endianness:

- `0x1A2B3C4D`: Big endian
- `0x4D3C2B1A`: Little endian

The Major Version and Minor Version are expected to be 1 and 0 respectively.

### 3.5. Interface Description Block (IDB)

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                    Block Type = 0x00000001                    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |           LinkType            |           Reserved            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |                            SnapLen                            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 /                                                               /
      /                      Options (variable)                       /
      /                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

The LinkType is described below, and is the same for a PCAP file.

The identifier for the IDB starts at zero and is incremented for each IDB after
a SHB. The EPB uses this identifier.

The following options must be parsed:

- `if_tsreso`. Code `9` and length `1`.
  - If the Most Significant Bit is equal to zero, the remaining bits indicates
    the resolution of the timestamp as a negative power of 10 (e.g. 6 means
    microsecond resolution, timestamps are the number of microseconds since
    1970-01-01 00:00:00 UTC).
  - If the Most Significant Bit is equal to one, the remaining bits indicates
    the resolution as negative power of 2 (e.g. 10 means 1/1024 of second).
  - If this option is not present, a resolution of 10^-6 is assumed (i.e.
    timestamps have the same resolution of the standard 'libpcap' timestamps).

### 3.6. Enhanced Packet Block (EPB)

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                    Block Type = 0x00000006                    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |                         Interface ID                          |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |                        Timestamp (High)                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 |                        Timestamp (Low)                        |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   20 |                    Captured Packet Length                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   24 |                    Original Packet Length                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   28 /                                                               /
      /                          Packet Data                          /
      /              variable length, padded to 32 bits               /
      /                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      /                                                               /
      /                      Options (variable)                       /
      /                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      Block Total Length                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

If the Interface ID is a number which is more than any known IDB since parsing
the SHB, it will be ignored.

To calculate the time stamp, the `Timestamp` field is a 64-bit value, since
1/1/1970. The resolution of the time stamp is defined by the option `if_tsreso`
of the IDB.

## 4. Link Type Frames

The packet data depends on the Link-layer type.

### 4.1. Ethernet Frame

The Ethernet frame starts with 12 bytes. These are typically recorded using
logger devices and WireShark from an interface.

```text
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |                   Destination Mac (6 octets)                  |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |           Dest Mac            |            Src Mac            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |                      Source Mac (6 octets)                    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |            Proto              |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

### 4.2. Linux Cooked Frame

A Linux Cooked Frame (SLL packet) consists of only the MAC which recorded the
frame. See [TCPDump LinkType Linux
SLL](https://www.tcpdump.org/linktypes/LINKTYPE_LINUX_SLL.html).

```text
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |          Packet Type          |         ARPHRD_ type          |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |   Link-layer address length   |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    6 |                   Link-layer address (8 octets)               |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   10 |                                                               |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   14 |            Proto              |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

## 5. Protocol

The supported protocols are:

- 0x88A8 - 802.1ad Q-in-Q header. Expect to be followed by 0x8100.
  - 0x9100 and 0x8100 are also supported as Q-in-Q header
- 0x8100 - 802.1q Virtual LAN
  - If present, the next two bytes represent the VLAN and Priority Code Point.
- 0x99FE - TECMP
- 0x0800 - IPv4

## 6. TECMP Protocol

See [TECMP User Manual
v1.6](https://raw.githubusercontent.com/Technica-Engineering/libtecmp/master/docs/TECMP_User_Manual_V1.6.pdf).

### 6.1. TECMP Header

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |           Device ID           |            Counter            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |    Version    |    Msg Type   |           Data Type           |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |           Reserved            |           CM Flags            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 /                                                               /
      /                         TECMP Payload                         /
```

Values are in big-endian

- Device ID: Unique device sending / recording the packets.
- Counter: 16 bit monotonic, wrap-around counter
- Version: Expected to be 3
- Msg Type: TECMP_MSG_TYPE_LOG_STREAM (0x03)
- Data Type: TECMP_DATA_TYPE_ETH (0x0080)
- CM Flags

  | CMFlags (bits) | Meaning                                                       |
  | -------------- | ------------------------------------------------------------- |
  | [1:0] 10       | Start of a Segmented Message                                  |
  | [1:0] 00       | Within a Segmented Message                                    |
  | [1:0] 01       | End of a Segmented Message                                    |
  | [1:0] 11       | Unsegmented Message                                           |
  | [2] 1          | SPY                                                           |
  | [3] 1          | Multiframe, can send multiple Interface IDs in a single frame |
  | [14:4]         | Reserved                                                      |
  | [15] 1         | Capture Module overflow                                       |

  e.g. 0x000F

  - Unsegmented message (0x03)
  - SPY message (0x04)
  - Multi-frame message (0x08)
  - No capture module overflow

### 6.2. TECMP Payload

There may be multiple sections of this payload immediately after the TECMP
header.

```text
                           1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |                         Interface ID                          |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 |                       Time stamp 63..32                       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   20 |                       Time stamp 31..0                        |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   24 |            Length             |          Data Flags           |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   28 /                                                               /
      /                            Payload                            /
      /                                                               /
```

- Interface ID: uniquely identifies the log data / bus / link
- Timestamp:
  - Bits [61..0] are the number of nanoseconds since 1/1/1970 from a
  PTP clock. Independent of the Wireshark recording timestamp.
  - Bit 62: Timestamp recalculation
  - Bit 63: Valid for TECMP_MSG_TYPE_LOG_STREAM
    - 0 = Synchronised with a PTP clock
    - 1 = Lost synchronisation after 250ms
- Length: Length of the payload (not including this header)
- Data Flags: Values for TECMP_DATA_TYPE_ETH

  | Data Flags Bit | Meaning   |
  | -------------- | --------- |
  | [12:0]         | Reserved  |
  | [13]           | CRC Error |
  | [14]           | TX        |
  | [15]           | Overflow  |

## 7. IPv4 Protocol

See [RFC 791](https://datatracker.ietf.org/doc/html/rfc791).

The Internet Datagram header is:

```text
       0                   1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |Version|  IHL  |Type of Service|          Total Length         |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |         Identification        |Flags|      Fragment Offset    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    8 |  Time to Live |    Protocol   |         Header Checksum       |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   12 |                       Source Address                          |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   16 |                    Destination Address                        |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   20 |                    Options                    |    Padding    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      /                                                               /
      /                          Packet Data                          /
      /                  variable length, not padded                  /
      /                                                               /
```

The options define further definition for the IPv4 header and is optional. The
total length of the Internet header is dependent on the IHL field, e.g. if 0x5,
then the length is 20 bytes.

- Version: Expected to be 0x4
- IHL: Defines the length of the header in the number of 32-bit octets
- Type of Service: Ignored
- Total Length: The length of the entire Internet datagram including the Version
  (but not including the protocol).
- Identification: Used to identify the packet. Useful for packet fragmentation.
- Flags: Defines fragmentation
  - D - Do not Fragment (0x40)
  - M - More Fragments (0x20)
- Fragment Offset: The number of 64-bit octets offset from the *data*. If
  fragmented (and not the last fragment), the fragment must be a multiple of 8
  bytes.
- TTL: The number of router hops before the packet should be dropped. Is
  decremented by each router hop.
- Protocol: The Internet protocol, e.g.
  - 17: UDP
- Header Checksum: The checksum of the IPv4 packet
- Source Address, Destination Address: The 32-bit octet (network order) address
  of the source and destination.
- Options: Optional data, present if the IHL is more than 0x5.

### 7.1. IP Fragmentation

When IPv4 fragmentation occurs, the data is fragmented over multiple IPv4
packets. Each new packet has a new IPv4 header recalculated with a new length
and new checksum. the minimum length of the v4 packet data must be 8 bytes. It
can be up to the MTU (which on Ethernet is typically 1500 packet data bytes, but
may be less if VLAN is used).

The Identification field is used to identify the reconstruction of the fragments
for a given source and destination address and protocol type. Apart from the
packet data from being split, no change to the content of the packet data is
made.

A group of IP fragments are reconstructed into a single IPv4 packet based on the
following fields:

- The source address.
- The destination address.
- The protocol identifier.
- The identification field.

The length of the packet is unknown, until the final packet is received. The
final packet is identified as having the MF bit set to zero. The length is then
calculated from the data length and the fragmentation offset.

The ordering of the fragmented IP packets is undefined, but it is observed often
on Linux that the last packet is transmitted first. However, this is not
guaranteed.

## 8. UDP Protocol

The UDP protocol starts immediately after the IPv4 header, and is described by
[RFC 768](https://datatracker.ietf.org/doc/html/rfc768)

It has the following format:

```text
       0                   1                   2                   3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    0 |          Source Port          |       Destination Port        |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    4 |            Length             |           Checksum            |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      /                                                               /
      /                          Data Octets                          /
      /                                                               /
```

The fields are all stored in network order (big-endian). The length is the
length of the entire UDP packet including the 8-byte header and all other
fragments. The Checksum is over the entire data octets, which may be split
across multipe IPv4 fragments.

In case of IPv4 fragmentation, the source port and destination port are only in
the first fragment, as IP fragmentation is over IP not UDP.
