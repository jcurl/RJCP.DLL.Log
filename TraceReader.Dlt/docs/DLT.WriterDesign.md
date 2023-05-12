# DLT Writer Design <!-- omit in toc -->

This document considers the mechanism to take a DLT trace line and write this to
a stream that conforms to the DLTv1 protocol. This design is based on DLTv1 from
R20-11 and earlier.

## Table of Contents <!-- omit in toc -->

- [1. DLT Trace Writer](#1-dlt-trace-writer)
  - [1.1. Purpose of the DLT Trace Writer](#11-purpose-of-the-dlt-trace-writer)
  - [1.2. Use Cases of the DLT Trace Writer](#12-use-cases-of-the-dlt-trace-writer)
- [2. Design for the Writer](#2-design-for-the-writer)
  - [2.1. Factory Creation Model](#21-factory-creation-model)
  - [2.2. Format of a DLT v1 Packet](#22-format-of-a-dlt-v1-packet)
  - [2.3. Argument Encoders (DLT Trace Encoder)](#23-argument-encoders-dlt-trace-encoder)
  - [2.4. Control Encoders (DLT Trace Encoder)](#24-control-encoders-dlt-trace-encoder)

## 1. DLT Trace Writer

There are three trace writers for DLT. A trace writer prepares a packet and
ensures that a write is done as a single packet.

* TCP based streams, where a DLT packet starts with the standard header.
* Storage files, where a header based on the ECU and the recorded time stamp is
  recorded.
* Serial based streams.

### 1.1. Purpose of the DLT Trace Writer

The DLT Trace writer shall write DLT verbose messages based on trace lines. Data
can be sent out via any stream, to a file or a network. Data is sent out as
packets to the stream interfaces.

### 1.2. Use Cases of the DLT Trace Writer

The following use cases are considered:

* Take a decoded trace line (verbose or non-verbose) and write, where possible,
  as a verbose message.
* Data will be taken from the trace line and serialized to a temporary buffer,
  which is then written. The buffer is fixed, up to 64kB. A trace line would
  result in a single write call to the stream.
* Arguments as provided by the DLT Trace Decoeder should be supported.
* It should be possible to extend functionality based on custom arguments.

## 2. Design for the Writer

### 2.1. Factory Creation Model

The design for the writer will use factories, which implement interfaces,
following a similar pattern to [DLT Decoder Design](./DLT.DecoderDesign.md).

![](out/diagrams/DLT.Writer/DLT.Writer.svg)

This will allow a common implementation for writing DLT packets, which can be
extended to prepend appropriate headers. The encoder would be the same for all
the various writers:

* Network Writer (no headers)
* File Writer (with a storage header)
* Serial Writer (with a serial header)

and this is independent of how to write the individual packets, which parses the
trace line, and the arguments in the trace line to serialize the data.

A different DLT encoder factory can be provided to a Dlt writer factory,
especially if the user wishes to extend an encoder for their own types.

### 2.2. Format of a DLT v1 Packet

The data that will be written in this design is limited only to writing DLT
verbose. This implies the DLT non-verbose flag will be ignored. This is useful
for converting non-verbose messages to verbose messages that can be read offline
without needing the original FIBEX conversion file.

### 2.3. Argument Encoders (DLT Trace Encoder)

The encoder is responsible for writing the data.

![](out/diagrams/Dlt.WriterEncoder/DLT.WriterEncoder.svg)

This can be extended to handle further argument types, e.g. from a Non-Verbose
message that can't be decoded to a RAW argument. The message information would
be lost.

### 2.4. Control Encoders (DLT Trace Encoder)

TBD
