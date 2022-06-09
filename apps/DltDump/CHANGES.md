# Changes

This document shows the changes since last releases.

## Current (1.0.0-alpha.v7.20220609)

Features:

* DOTNET-625: Decode DLT packets split across multiple UDP packets (not IP
  fragmentation). Treat each endpoint as a virtual connection.

Bugs:

* DOTNET-640: Fix out of bounds access which could crash for a corrupted PCAP
  file
* DOTNET-639: Decode PCAP where capture and original size don't match

## 1.0.0-alpha.v7.20220522

Features:

* DOTNET-622: Read PCAP (Legacy) files, decoding UDP packets with destination
  port 3490
* DOTNET-626: Read PCAP-NG files, decoding UDP packets with destination port
  3490
* DOTNET-632: Use `RJCP.Diagnostics.Log.Dlt.Pcap` for logging

## 1.0.0-alpha.v7.20220502

First version that was released for use. See the [README.md](README.md) and the
command line help.
