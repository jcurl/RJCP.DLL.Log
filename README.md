# RJCP.Diagnostics.Log Project <!-- omit in toc -->

This project started in 2011 as a means to read simple text files. It grew into
reading multiple file formats that are log related in Automotive, all with a
packet structure.

This version of the reader is a rewrite based on lessons learned to try and
achieve a better performing library and to take advantage of new features given
in .NET Core.

- [1. Further Documentation](#1-further-documentation)
- [2. Libraries](#2-libraries)
  - [2.1. Core Library (RJCP.Diagnostics.Log)](#21-core-library-rjcpdiagnosticslog)
  - [2.2. Text Library (RJCP.Diagnostics.Log)](#22-text-library-rjcpdiagnosticslog)
  - [2.3. DLT Library (RJCP.Diagnostics.Log.Dlt)](#23-dlt-library-rjcpdiagnosticslogdlt)
- [3. Lessons Learned](#3-lessons-learned)

## 1. Further Documentation

[Introduction to RJCP.Diagnostics.Log](TraceReader.Core/README.md)

[The DLT RJCP.Diagnostics.Log.Dlt Reader](TraceReader.Dlt/README.md)

See [Documentation and Design](docs/README.md).

## 2. Libraries

### 2.1. Core Library (RJCP.Diagnostics.Log)

It consists of the core library, which lays down the framework for loading files
and parsing them, and returning each line in the log file as structured data,
which then other applications can group, sort or filter as required to gain
extra knowledge.

### 2.2. Text Library (RJCP.Diagnostics.Log)

See [TraceReader Text Design](docs/TraceReader.Text.md)

The text reading library is the simplest application of the core library, which
reads a text file and returns each individual line in the text file.

### 2.3. DLT Library (RJCP.Diagnostics.Log.Dlt)

See [TraceReader DLT Design](docs/TraceReader.Dlt.md)

Implements decoding and encoding DLT messages as defined by AutoSAR R20-11 and
earlier.

## 3. Lessons Learned

These lessons learned are taken from the first implementation. The old library
was intended for automation testing. This version is considerably simplified
with the goal of just reading and writing the files. As such, speed is of
concern, buffering is removed where possible, threading is reduced to using
asynchronous and awaitable methods instead.

- The reader and writer are no longer using threads to try and prefetch data
  from streams. The original implementation would enumerate lines on the current
  thread, while a background thread would try to decode. This introduced
  concurrency issues, especially with call backs and made implementation
  difficult.
- Use modern .NET Core, along with C# Asynchronous I/O. Makes the code easier to
  manage and readable.

Some optimisations can certainly be done, but there is more responsibility (and
flexibility) on the calling application.
