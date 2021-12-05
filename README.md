# RJCP.Diagnostics.Log Project

This project started in 2011 as a means to read simple text files. It grew into
reading multiple file formats that are log related in Automotive, all with a
packet structure.

This version of the reader is a rewrite based on lessons learned to try and
achieve a better performing library and to take advantage of new features given
in .NET Core.

The old library was intended for automation testing. This version is
considerably simplified with the goal of just reading and writing the files. As
such, speed is of concern, buffering is removed where possible, threading is
reduced to using asynchronous and awaitable methods instead.

## Core Library (RJCP.Diagnostics.Log)

It consists of the core library, which lays down the framework for loading files
and parsing them, and returning each line in the log file as structured data,
which then other applications can group, sort or filter as required to gain
extra knowledge.

## Text Library (RJCP.Diagnostics.Log)

The text reading library is the simplest application of the core library, which
reads a text file and returns each individual line in the text file.

## DLT Library (RJCP.Diagnostics.Log.Dlt)

Implements decoding DLT messages as defined by AutoSAR.
