# Introduction - DLT Format

This document tries to capture at a high level the protocol format. Specific details fo the
DLT format should be obtained by the relevant specifications.

## General Format

The DLT v1 packet consists of:

* A storage header (for files recorded from a DLT talker)
* A standard header
* An extended header
* A payload

The storage header is optional, and contains information on when the data was
recorded. It is the format for on disk DLT logs.

A standard header provides information about the DLT payload. It describes the
features.

An extended header contains optional information, which is typically present for
verbose DLT logging.

The payload consists of structured arguments that describe a type and the data
associated with that type. The entire string is then concatenated.

## Construction of a DLT Packet

The diagram below shows graphically the constituents of a DLT packet. This
diagram is useful when reading DLT binary (even if it is in the AutoSAR
documentation, a condensed form makes it easier to parse the packets by hand,
should there be corruption of the data stream)

![DLT Format](out/diagrams/DLT.FormatCheatSheet/DLT.FormatCheatSheet.svg)

If the Extended Header is not provided, it is assumed that the packet is
non-verbose. Therefore, all verbose DLT messages must have the extended header
(but having the extended header alone is not enough for a verbose packet, it
must also be explicitly marked as verbose).
