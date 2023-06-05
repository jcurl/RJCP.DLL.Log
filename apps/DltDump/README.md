# DltDump <!-- omit in toc -->

DltDump is a tool that reads DLT Version 1 formatted files.

- [1. Introduction](#1-introduction)
  - [1.1. License](#11-license)
  - [1.2. Help](#12-help)
  - [1.3. Version Information](#13-version-information)
  - [1.4. Using Later .NET Core Versions](#14-using-later-net-core-versions)
  - [1.5. Reading](#15-reading)
  - [1.6. Writing](#16-writing)
  - [1.7. Filtering](#17-filtering)
- [2. Common Use Cases](#2-common-use-cases)
  - [2.1. Recording Data from a TCP Stream](#21-recording-data-from-a-tcp-stream)
  - [2.2. Recording data sent via UDP](#22-recording-data-sent-via-udp)
  - [2.3. Recording data from a Serial Stream](#23-recording-data-from-a-serial-stream)
  - [2.4. Recording data from a Serial Stream in the Network Format](#24-recording-data-from-a-serial-stream-in-the-network-format)
  - [2.5. Converting a Recorded File to Text](#25-converting-a-recorded-file-to-text)
  - [2.6. Converting a PCAP File to DLT](#26-converting-a-pcap-file-to-dlt)
  - [2.7. Interpreting DLT Non-Verbose](#27-interpreting-dlt-non-verbose)
    - [2.7.1. Ignoring the Extended Header (Application and Context Identifier)](#271-ignoring-the-extended-header-application-and-context-identifier)
    - [2.7.2. Loading Multiple Fibex files with Various ECU identifiers](#272-loading-multiple-fibex-files-with-various-ecu-identifiers)
    - [2.7.3. Check all Fibex Files for Correctness](#273-check-all-fibex-files-for-correctness)
    - [2.7.4. Writing to Verbose Mode](#274-writing-to-verbose-mode)
  - [2.8. Search for a String](#28-search-for-a-string)
  - [2.9. Search for a Regular Expression](#29-search-for-a-regular-expression)
  - [2.10. Filter for a Range of Dates](#210-filter-for-a-range-of-dates)
  - [2.11. Case Insensitive Search](#211-case-insensitive-search)
  - [2.12. Printing Context (like 'grep')](#212-printing-context-like-grep)
  - [2.13. Filtering for an Application Identifier](#213-filtering-for-an-application-identifier)
  - [2.14. Filter Summary](#214-filter-summary)
  - [2.15. Concatenating files](#215-concatenating-files)
  - [2.16. Splitting files](#216-splitting-files)
  - [2.17. Splitting files on Input based on Time Stamp](#217-splitting-files-on-input-based-on-time-stamp)
  - [2.18. Searching for Corruption on the Input DLT File](#218-searching-for-corruption-on-the-input-dlt-file)
- [3. Detailed Usage](#3-detailed-usage)
  - [3.1. Input Formats](#31-input-formats)
  - [3.2. Time Stamps](#32-time-stamps)
  - [3.3. Live Streams, Retries and Time outs](#33-live-streams-retries-and-time-outs)
  - [3.4. Reading Non-Verbose](#34-reading-non-verbose)
    - [3.4.1. Default Behaviour When Reading a Fibex File](#341-default-behaviour-when-reading-a-fibex-file)
    - [3.4.2. Read Multiple Fibex Files from Different ECUs](#342-read-multiple-fibex-files-from-different-ecus)
    - [3.4.3. Read Multiple Fibex Files Merging for a Single ECU](#343-read-multiple-fibex-files-merging-for-a-single-ecu)
    - [3.4.4. Ignore the Extended Header in the Fibex File](#344-ignore-the-extended-header-in-the-fibex-file)
    - [3.4.5. Summary of Message Id Mapping](#345-summary-of-message-id-mapping)
    - [3.4.6. Check the FIBEX Files for Correctness Without a DLT File](#346-check-the-fibex-files-for-correctness-without-a-dlt-file)
    - [3.4.7. Filter for a Specific Message Identifier](#347-filter-for-a-specific-message-identifier)
    - [3.4.8. Convert from Non-Verbose to Verbose](#348-convert-from-non-verbose-to-verbose)
  - [3.5. Output Files](#35-output-files)
  - [3.6. Output Formats](#36-output-formats)
  - [3.7. Filters](#37-filters)
  - [3.8. Context](#38-context)
  - [3.9. Splitting Output Files and Output Templates](#39-splitting-output-files-and-output-templates)
  - [3.10. Analysing Errors](#310-analysing-errors)
    - [3.10.1. Using Extended Logging](#3101-using-extended-logging)
    - [3.10.2. Getting the Position per Line](#3102-getting-the-position-per-line)
- [4. Further Work](#4-further-work)
  - [4.1. Contributing](#41-contributing)
  - [4.2. Reporting Issues](#42-reporting-issues)
- [5. References](#5-references)

## 1. Introduction

### 1.1. License

See the license file at the top of this repository [LICENSE](../../LICENSE.md).

### 1.2. Help

For detailed information on how to use this tool, run on the command line:

For Windows:

```cmd
dltdump.exe /?
```

For Linux:

```sh
dotnet dltdump.dll -?
```

The command line options given in this manual are using the Linux format, and
assuming the alias `dltdump` is the same as running `dotnet dltdump.dll`.

### 1.3. Version Information

To print the version:

```sh
dltdump --version
DltDump Version: 1.0.0-alpha.v7.20230605T062448+gfc5aab8, (C) 2022-2023, Jason Curl
  Runtime: 3.1.32
  TraceReader: 0.8.0-alpha.20230528T114734+gf9ead1e
  TraceReader.Dlt: 0.8.0-alpha.v7.20230605T062448+gfc5aab8
```

### 1.4. Using Later .NET Core Versions

The software is compiled with a specific version of .NET Core, which you can
obtain using the `--version` command. If you don't have this version installed,
or wish to get performance benefits from a later version:

On Windows:
```sh
dltdump --roll-forward LatestMajor /version
DltDump Version: 1.0.0-alpha.v7.20230605T062448+gfc5aab8, (C) 2022-2023, Jason Curl
  Runtime: 7.0.3
  TraceReader: 0.8.0-alpha.20230528T114734+gf9ead1e
  TraceReader.Dlt: 0.8.0-alpha.v7.20230605T062448+gfc5aab8
```

On Linux:
```
dotnet exec --roll-forward ./dltdump.dll --version
```

### 1.5. Reading

DltDump can read from:

* A file on disk;
  * As recorded by a logger, containing a storage header; or
  * binary data, recorded from a TCP stream (e.g. with `netcat`), or serial.
  * Recorded with a PCAP or PCAP-NG file
    * From UDP packets in IPv4
    * From TECMP packets, as UDP in IPv4
* a TCP server; or
* a Serial port.

### 1.6. Writing

DltDump can write the output to disk in the formats:

* Text format; or
* DLT format

### 1.7. Filtering

When reading the DLT file, the input can be filtered for specific properties,
searching for:

* ECU identifiers;
* Application identifiers;
* Context identifiers;
* Session identifiers;
* Verbose, non-verbose, control messages;
* Message types (e.g. `LOG_INFO`, etc.);
* substrings; and
* regular expressions (in .NET format).

## 2. Common Use Cases

With the functionality provided some common use cases are:

### 2.1. Recording Data from a TCP Stream

To record data from a DLT server listening on port `192.168.1.10:3490`:

```sh
dltdump --output record.dlt tcp://192.168.1.10
```

The input format is assumed to be in the network format. Data recorded by the
network TCP stream uses the time stamp of the PC at the time the message is
seen.

### 2.2. Recording data sent via UDP

Devices may send data to a unicast or a multicast address. Unicast address must
be the IPv4 address of a local interface. Alternatively join a multicast group
and listen for DLT messages.

Listen on the multicast group 224.0.1.1

```sh
dltdump --output record.dlt udp://239.255.1.1
```

### 2.3. Recording data from a Serial Stream

To record data from `/dev/ttyUSB0`:

```sh
dltdump --output record.dlt ser:/dev/ttyUSB0,115200,8,n,1
```

On Windows, this would be

```cmd
dltdump /output:record.dlt ser:COM1,115200,8,n,1
```

The packets are assumed to have a serial header `DLS\1`. Data recorded by the
serial port uses the time stamp of the PC at the time the message is seen.

### 2.4. Recording data from a Serial Stream in the Network Format

Some devices may not send the `DLS\1` header, but only send the standard header.
It is recommended to use the serial header on serial streams due to common
errors with packet loss over the serial stream. See [DLT Format
Problems](../../TraceReader.Dlt/docs/DLT.Format.Problems.md).

```sh
dltdump --output record.dlt --format net ser:/dev/ttyUSB0,115200,8,n,1
```

### 2.5. Converting a Recorded File to Text

To read a recorded DLT file and convert it to text. The extension `.txt` is used
to know that the output should be written in text form, instead of a DLT packet.

```sh
dltdump --output convert.txt record.dlt
```

### 2.6. Converting a PCAP File to DLT

To convert an existing PCAP or PCAP-NG file to a DLT, the input extension must
be `.pcap` or `.pcapng`, and the output extension must be `.dlt`.

```sh
dltdump --output out.dlt input.pcapng
```

Invalid packets, including those that have only partial captures, are ignored.
They are not printed on the command line, or put in the log file. Only packets
with the destination port 3490 are captured.

### 2.7. Interpreting DLT Non-Verbose

If you have a FIBEX file, as described in AutoSAR R19-11, you can provide this
to aid in conversion of Non-Verbose messages in a readable format. This is only
for visual conversion and searches on the output, it does not modify the output
DLT message when filtering results to a new DLT.

```sh
dltdump --fibex myfibexfile.xml input.dlt
```

or for a directory:

```sh
dltdump --fibex mydir input.dlt
```

Default behaviour is as of Covesa dlt-viewer, which ignores the ECU identifier
in the Fibex / Standard header if provided, and uses the application and context
identifier message in the DLT to allow multiple messages of the same message
identifier.

#### 2.7.1. Ignoring the Extended Header (Application and Context Identifier)

If you know the source DLT does not have extended headers, you can ensure that
all frame identifiers are unique and skip the lookups by application and context
identifier. This can ensure warnings in case of message identifier overlaps that
wouldn't normally occur.

```sh
dltdump --fibex myfibexfile.xml --nv-noexthdr input.dlt
```

#### 2.7.2. Loading Multiple Fibex files with Various ECU identifiers

In case your log file contains multiple ECUs, you can load multiple Fibex files
and ensure the message identifiers don't overlap, assuming each DLT message
contains a standard header that provides the ECU identifier.

```sh
dltdump --fibex myecu1.xml --fibex myecu2.xml --nv-multiecu input.dlt
```

#### 2.7.3. Check all Fibex Files for Correctness

You can ignore the input file, and just provide the Fibex options. In this case,
the tool will load the Fibex files, output any problems and exit.

```sh
dltdump --fibex myecu1.xml --fibex myecu2.xml --nv-multiecu
```

#### 2.7.4. Writing to Verbose Mode

If you want to share the DLT output with someone else so they don't need the
FIBEX file, convert the non-verbose messages to verbose with the `--nv-verbose`
command.|

```sh
dltdump --fibex myecu1.xml --nv-multiecu --nv-verbose -o out.dlt input.dlt
```

### 2.8. Search for a String

Search for a specific string in the output and print to the console

```sh
dltdump -s substring record.dlt
```

This is conceptually similar to doing a "grep" over the input.

### 2.9. Search for a Regular Expression

Likewise, .NET regular expressions are supported

```sh
dltdump -r "\\d+" record.dlt
```

This searches for the regular expression `\d+`, which matches for all lines that
have a number.

### 2.10. Filter for a Range of Dates

Return all time stamps that are either after, or before a particular date. This
helps create a log file that has only a specific range.

This specific example will only return lines in the range 18th June 2022, time
10:00:00 to 10:05:00.

```sh
dltdump --not-before 2022-06-18T10:00:00 --not-after 2022-06-18T10:05:00
```

### 2.11. Case Insensitive Search

If the string to be sought should be case insensitive, add the option `i`:

```sh
dltdump -i -s substring record.dlt
```

### 2.12. Printing Context (like 'grep')

If you want to see the lines before and after a match, use the context options
`-A` (after) and `-B` (before):

```sh
dltdump -s substring -A 10 -B 2 record.dlt
```

### 2.13. Filtering for an Application Identifier

If you only want to see logs from a particular application on the console:

```sh
dltdump --appid APP1 record.dlt
```

Or if you want to output the result to a new file `filtered.dlt`

```sh
dltdump --appid APP1 --output filtered.dlt record.dlt
```

### 2.14. Filter Summary

The options for filtering are:

* `--ecuid`: Filter for an ECU identifier
* `--appid`: Filter for an application identifier
* `--ctxid`: Filter for a context identifier
* `--sessionid`: Filter for a session identifier
* `--type`: Filter for message types
* `--verbose`: Filter for verbose messages
* `--nonverbose`: Filter for non-verbose messages (excluding control messages)
* `--control`: Filter for control messages
* `--messageid`: Filter for specific non-verbose messages
* `--not-before`: Filter for messages after this time
* `--not-after`: Filter for messages before this time
* `--before-context=N`: On a filter match, show N lines before
* `--after-context=N`: On a filter match, show N lines after

You can provide the filters, they have a logical "and" relationship. Providing
the same filter option (e.g. `appid`) has a logical "or" relationship.

### 2.15. Concatenating files

If you have multiple files, you can parse and join them together. The ordering
is important (no sorting of the input files are made):

```sh
dltdump --output concat.dlt input1.dlt input2.dlt input3.dlt
```

Or join many PCAP files into one DLT file

```sh
dltdump --output result.dlt file001.pcap file002.pcap file003.pcap
```

### 2.16. Splitting files

If you have a large file, you can split it up into many smaller files. This
example takes the input `record.dlt` and splits it up into files named
`split_001.dlt`, `split_002.dlt`, etc. each being approximately 100MB in size.

```sh
dltdump --split 100M --output split_%CTR%.dlt record.dlt
```

### 2.17. Splitting files on Input based on Time Stamp

An extension of splitting the files is to record data, and to split the output
into files of particular sizes, with the file name having the time stamp when
the file was started.

```sh
dltdump --split 50M --output record_%CDATETIME%.dlt tcp://192.158.1.10
```

### 2.18. Searching for Corruption on the Input DLT File

If the "dlt-viewer" isn't showing data as you'd expect from recorded input, you
can filter for SKIP data, and show the position of the input file. Let's say
you've recorded the data to a file called `dlt.raw` which is directly from a TCP
stream.

You can convert this to file that can be read via the dlt-viewer:

```sh
dltdump --output convert.dlt --format net dlt.raw
```

You can search for errors in the DLT stream with

```sh
dltdump --position --format net --appid SKIP --ctxid SKIP dlt.raw
```

This will show all errors that `dltdump` found when decoding (it generates a new
line with an application identifier of SKIP and a context identifier also of
SKIP) to the console. Prefixed with each line that is printed is the byte offset
into the original stream where data was first found as invalid. This allows you
to view with a hex editor the raw content on what might have gone wrong.

## 3. Detailed Usage

The `dltdump` tool is a command line tool for Windows and Linux using
Microsoft's .NET Core runtime and SDK. It is compiled and tested against .NET
Core 3.1 LTS.

Using .NET Core 5 and later, one can request to roll forward. It is tested to
work with at least .NET Core 7.0.

```sh
$ dotnet --roll-forward LatestMajor dltdump.dll --version
DltDump Version: 1.0.0-alpha.v7.20230502T045541+g376601d, (C) 2022-2023, Jason
Curl
  Runtime: 7.0.3
  TraceReader: 0.8.0-alpha.20230409T115810+ge8901e3
  TraceReader.Dlt: 0.8.0-alpha.v7.20230502T045541+g376601d
```

### 3.1. Input Formats

The `dltdump` only supports DLT Version 1, as provided by [AutoSAR PRS Log and
Trace
R20-11](https://www.autosar.org/fileadmin/user_upload/standards/foundation/20-11/AUTOSAR_PRS_LogAndTraceProtocol.pdf).

It can read files in three different formats:

* As files already stored on the filesystem, that contains a storage header, as
  added by loggers; or as PCAP or PCAP-NG files stored on the file system
  encapsulated in IPv4 UDP packets;
* As a network stream, described in the PRS, starting with the standard header;
  and
* As a serial stream, which is the fixed 4-byte marker `DLS\1` followed by the
  standard header. This is not specified in the PRS, but implemented in the
  Covesa DLT Viewer application.

If the format is written to disk but doesn't contain a storage header (as it was
recorded by a device that stores raw bytes), then the option `--format` can be
used to specify the format.

By default:

| Input                           | Default Format    |
| ------------------------------- | ----------------- |
| DLT file with storage header    | `--format=file`   |
| PCAP or PCAP-NG file            | `--format=pcapng` |
| `tcp://host:port`               | `--format=net`    |
| `ser:/dev/ttyUSB0,115200,8,n,1` | `--format=ser`    |

### 3.2. Time Stamps

If the input format is `--format=file`, then the storage header is used to
extract the time stamp at which the message was recorded. Otherwise, if the
recording is made directly from TCP or Serial, the time stamp of the PC at the
time the message is decoded is used. If the file is on disk and has no storage
header, the default Unix time stamp of 1/Jan/1970 is used.

If the input format is `--format=pcap` or `--format=pcapng`, the time stamp
associated with the packet is used when the message was recorded.

### 3.3. Live Streams, Retries and Time outs

The input `tcp://` and `ser:` are considered live streams. As such, the time
stamp for a live stream is the current PC time stamp. But in addition, time outs
are applied, as there is usually no other natural way for a stream to close.

In case that there is a 5-second delay with no data received (the stream may
still be active), the `dltdump` tool will close the stream. This is useful for:

* Serial streams, the device powered off
* TCP streams, the device reset, and no TCP RST or TCP FIN was sent.

Using scripting, one can restart the `dltdump` tool to start a new session.

The option `--retries` can be used to perform multiple connect attempts. The
default is zero, which is no retries.

### 3.4. Reading Non-Verbose

DltDump supports decoding non-verbose messages in your input stream using FIBEX
files, as described in AutoSAR R19-11. Note, at this time, ARXML files are not
supported.

It uses a fast, in place mechanism for reading XML, so reading say 180MB, takes
about 3.5 seconds extra start time. XML is a verbose description language, the
in memory format is much smaller.

There are multiple modes for interpreting Fibex Files

* Default behaviour reading a FIBEX
* Read multiple FIBEX files from different ECUs
* Read multiple FIBEX files from the same ECU
* Ignore the Application and Context Identifier in the FIBEX files
* Check the FIBEX files for correctness without a DLT file

#### 3.4.1. Default Behaviour When Reading a Fibex File

The default behaviour of the dlt-viewer is to load a FIBEX file. The ECU
identifier in the FIBEX file is ignored.

When a DLT message is read, and it has no extended header (no application or
context information), it maps the message identifier to the first frame found.
If multiple frames exist with the same message identifier, those other frames
are ignored.

If the DLT message has an extended header, the application and context
identifier are used to find a matching frame with the message, application and
context identifier. This allows DLT logs that emit extended header information
to have multiple frames having the same identifier, so long as the application
and context information don't match.

The dlt-viewer allows a single XML file to have the same message identifier for
more than one frame. The XML standard disallows this (the ID attribute shall
have a unique value for the entire XML file). The DltDump tool enforces that the
identifier must always be unique for a single XML file. But multiple XML files
can be loaded that result in overlapping message identifiers, so long as the
application and context identifiers differ.

#### 3.4.2. Read Multiple Fibex Files from Different ECUs

Read multiple FIBEX files, belonging to multiple different ECUs, so that message
identifiers between the ECUs may overlap.

Provide the option `--nv-multiecu`.

This ensures separate internal differentiate between different ECUs having the
same message identifiers. This is an extension from the dlt-viewer. Provide
multiple Fibex files for the different ECUs on the command line, or provide the
directory containing all ECU Fibex files.

#### 3.4.3. Read Multiple Fibex Files Merging for a Single ECU

Read multiple FIBEX files, all belonging to the same ECU. This is useful for
build systems that compile components separately and therefore generate multiple
FIBEX files for the same product. Instead of processing all steps to merge the
FIBEX files into a single monolithic file, load in a directory of FIBEX Files,
so long as each self contains all PDUs and Frames.

Provide the multiple FIBEX files on the command line, or provide the directory
containing all the FIBEX files ending with the extension `.xml`.

#### 3.4.4. Ignore the Extended Header in the Fibex File

Ignore the Application and Context identifiers when reading in the FIBEX file.
This is especially useful if you know in advance that the DLT file being
interpreted does not provide an extended header (this is easy to see, when using
the DLT Viewer, the Application and Context identifiers are empty when no FIBEX
file is loaded).

Provide the option `--nv-noexthdr`.

This ensures that when loading in all FIBEX files, that each message identifier
is unique, irrespective of the manufacturer data that provides additionally the
application and context information.

The default mode, works fine so long as you know there are no overlapping
identifiers. If there are, but they have different application and context
identifiers. the dlt-viewer would not treat this as a warning, believing later
that the information can be resolved (which is not possible if the messages do
not contain an extended header). This is an additional safety mechanism to
ensure correctness.

#### 3.4.5. Summary of Message Id Mapping

Always assume that the Fibex file contains an application and context
identifier.

If the DLT message identifier has one or more frames having the same identifier,
they are grouped by the application and context identifier. Thus if the decoded
message has no application or context identifier, then the same is as the "flat"
in the next table.

| nv-multiecu | nv-noexthdr | DLT ECUID | DLT App/Ctx Id       | Result from Fibexes     | Notes                      |
| :---------: | :---------: | --------- | -------------------- | ----------------------- | -------------------------- |
|      -      |      -      | Ignored   | No Extended Header   | First Frame¹            | Same as `nv-noexthdr`      |
|      -      |      -      | Ignored   | Present in Fibex     | Match first occurrence² | Can decode                 |
|      -      |      -      | Ignored   | Not Present in Fibex | Not Found³              | Fallback to binary decoder |

For "flat" DLT files, where it's expected DLT non-verbose messages have no
extended header information. If two message frames have the same identifier,
only the first one is ever used.

| nv-multiecu | nv-noexthdr | DLT ECUID | DLT App/Ctx Id | Result       |
| :---------: | :---------: | --------- | -------------- | ------------ |
|      -      |      X      | Ignored   | Ignored        | First Frame¹ |

Providing the `nv-multiecu` option allows additional grouping if the ECU ID is
present in the DLT logs, and a matching FIBEX is loaded. If the DLT log contains
no ECU ID, or it contains an ECU ID but no loaded FIBEX, the behaviour is the
same as the default behaviour.

If you want to ensure decoding only for supported ECU IDs, provide the `--ecuid`
option to additionally filter the ECUID.

| nv-multiecu | nv-noexthdr | DLT ECUID         | DLT App/Ctx Id       | Result from Fibexes      | Notes                      |
| :---------: | :---------: | ----------------- | -------------------- | ------------------------ | -------------------------- |
|      X      |      -      | No ECU ID present | No Extended Header   | First Frame¹             | Same as "default"          |
|      X      |      -      | No ECU ID present | Present in Fibex     | Match first occurrence²  | Same as "default"          |
|      X      |      -      | No ECU ID present | Not Present in Fibex | Not Found³               | Same as "default"          |
|      X      |      -      | Matches a Fibex   | No Extended Header   | First for matching Fibex |                            |
|      X      |      -      | Matches a Fibex   | Present in Fibex     | Match for matching Fibex |                            |
|      X      |      -      | Matches a Fibex   | Not Present in Fibex | Not Found³               |                            |
|      X      |      -      | Not in a Fibex    | No Extended Header   | Not Found³               | Fallback to binary decoder |
|      X      |      -      | Not in a Fibex    | Present in Fibex     | Not Found³               | Fallback to binary decoder |
|      X      |      -      | Not in a Fibex    | Not Present in Fibex | Not Found³               | Fallback to binary decoder |

| nv-multiecu | nv-noexthdr | DLT ECUID         | DLT App/Ctx Id | Result from Fibexes      | Notes                      |
| :---------: | :---------: | ----------------- | -------------- | ------------------------ | -------------------------- |
|      X      |      X      | No ECU ID present | Ignored        | First Frame¹             | Same as `nv-noexthdr`      |
|      X      |      X      | Matches a Fibex   | Ignored        | First for matching Fibex |                            |
|      X      |      X      | Not in a Fibex    | Ignored        | Not Found³               | Fallback to binary decoder |

Notes

¹ First Frame means the first found frame with a matching message identifier
when loading all FIBEX files in order. If there is only one FIBEX file loaded,
then it is the only frame in that FIBEX file.

² Match first occurrence means the first found frame with a matching message,
application and context identifier. If there is only one FIBEX file loaded, then
the message identifier must be unique anyway according to the XML standard. The
most flexible case is if there are multiple FIBEX files loaded for the same ECU
where message identifiers may now be reused, so long as the application and
context identifiers are still unique in this combination.

³ Not found indicates that even though there is a frame in the FIBEX with a
matching identifier, no decoding is possible because the other fields don't
match.

#### 3.4.6. Check the FIBEX Files for Correctness Without a DLT File

It might be a useful step to check the correctness of the FIBEX files that there
is no overlap in message identifiers. The DltDump tool has a Non-Verbose check
mode, which is enabled when no input files are provided.

```sh
dltdump --nv-noexthdr --fibex file.xml
```

Or check by merging multiple files

```sh
dltdump --nv-noexthdr --fibex dir
```

You can ensure a directory of Fibex files from multiple ECUs can be checked at
once:

```sh
dltdump --nv-noexthdr --nv-multiecu --fibex dir
```

Note that the options are identical as described above, have the same behaviour,
only that immediately after loading and checking the FIBEX files, the program
exits.

#### 3.4.7. Filter for a Specific Message Identifier

With, or without the FIBEX file, it is possible to search and filter for
specific non-verbose message(s) in the DLT file.

```sh
dltdump --messageids=60,61,62 input.dlt
```

The output will show (using the default binary decoder for Non-Verbose without a
FIBEX file) DLT messages that have a non-verbose message identifier of 61, 61 or
62. The same can be provided with a FIBEX file.

```sh
dltdump --fibex myfibex.xml --messageids=60,61,62 input.dlt
```

Now the messages will also be shown decoded using the non-verbose decoder.

#### 3.4.8. Convert from Non-Verbose to Verbose

In some cases, you may wish to export a DLT file that is not dependent on the
original verbose message. The `--nv-verbose` command will read and interpret the
DLT non-verbose messages, and then write the message as verbose. You should
ensure you are using the correct FIBEX file:

```sh
dltdump --fibex myfibex.xml --nv-multiecu --nv-verbose -o out.dlt input.dlt
```

On the output, the verbose messages contain the same arguments as the original
non-verbose message with the following exceptions:

* There is no message identifier any more associated with the verbose message.
  This information is lost. The DLT standard does not account for this use case.

### 3.5. Output Files

Writing to a file is done with the `--output` option. If the file already exists
on disk, it will by default, not be overwritten. To override this, provide the
option `--force` which forcibly overwrites the output file that already exists.

Note, if the file being written is one of the inputs, or was written by this
specific instance of `dltdump`, the `--force` option has no effect, and the file
will never be overwritten.

On Linux, the file names are compared between the inputs and the outputs, so it
may be possible to overwrite if you "trick" `dltdump` with soft links and the
such. On Windows, it queries the file system if files are the same.

### 3.6. Output Formats

There are three outputs:

* The console;
* Text file;
* DLT (version 1) file.

If the `--output` option is not used, the console is assumed. It is not
recommended to console pipe the output from Windows to disk, as the shell in use
can modify the output unexpectedly (e.g. PowerShell writes in UCS2, Msys in
UTF8, Command Console in the current code page).

If the extension of the file being written ends with `.dlt`, the output is
written as a DLT file, and a storage header is added if one is not already
present.

Otherwise, the file is written in text format using UTF-8 encoding.

### 3.7. Filters

Filters are given on the command line option:

* `--ecuid`: Filter for an ECU identifier
* `--appid`: Filter for an application identifier
* `--ctxid`: Filter for a context identifier
* `--sessionid`: Filter for a session identifier, often used as the process
  identifier using `dlt-daemon`.
* `--type`: Filter for message types
* `--verbose`: Filter for verbose messages
* `--nonverbose`: Filter for non-verbose messages (excluding control messages)
* `--control`: Filter for control messages
* `--not-before`: Filter for messages that occur on, or after this date
* `--not-after`: Filter for messages that occur on, or before this date
* `--messageid`: Filter for a non-verbose message identifier
* `--string`: Filter for a string
* `--regex`: Filter for a .NET regular expression.

The `string` and `regex` filters can be modified for case insensitive search
with the option `-i` (similar to the unix tool `grep`).

The filter is independent of the output, you can write it as a DLT file, or as
text, as described above.

Multiple entries can be filtered for, by providing a list of the filters for an
option, e.g.

```sh
dltdump --appid=APP1,APP2 --ctxid=CTX1,CTX2 recorded.dlt
```

This will filter for all lines that have either an application identifier of
`APP1` or `APP2` (always case sensitive) *and* having a context identifier of
`CTX1` or `CTX2`.

To filter for specific message types:

```sh
dltdump --type=fatal,error,warn recorded.dlt
```

This will show all log messages of the type `LOG_FATAL`, `LOG_ERROR` and
`LOG_WARN`.

### 3.8. Context

To show context at the time of a match, use the `--before-context` or
`--after-context` options, and give it the number of lines of context. The
context is for the file, without a filter applied. Should you want context for a
specific application identifier for example, you should first filter for the
application identifier on disk, and then run the tool a second time to filter
for a string with context.

### 3.9. Splitting Output Files and Output Templates

Output files (text or DLT) can be split. The `--output` option uses *templates*
which allow the output file name to change dependent on how the file is being
split.

To split the files, provide the `--output` option that provides a template, and
provide the option `--split` with a value indicating the size (in bytes) of when
to split. The output will not split in the middle of a file, the split occurs
immediately after. That makes the output files slightly larger than the split
size.

Modifiers can be used for the split output:

* `--split=2536K`: Split into 2.5MB files
* `--split=10M`: Split into 10MB files
* `--split=1G`: Split into 1GB files

The template describes how to name the file on a split. The following templates
are known:

* `%CTR%`: Provide a counter, starting at `001` and incrementing on each split
* `%CDATE%`, `%CTIME%`, `%CDATETIME%`: Provides the date/time of the first line
  in the file that is being split
* `%FILE%`: Provides the name of the input file being processed without the
  extension. On each new input file, the `%CTR%` is reset to `001`.

So, if you have a very large input file:

```sh
dltdump --output=%FILE%_%CDATETIME%.dlt --split=200M largeinput.dlt
```

will split the large input file `largeinput.dlt` into new files, each being
approximately 200MB in size. The output file will be
`largeinput_<YYYYMMDD>T<HHMMSS>.dlt` and the time stamp is to the current time
zone of the PC.

The same can be used for any input, not just a file. Split recording from a TCP
stream:

```sh
dltdump --output=record_%CDATETIME% --split=50M tcp://192.168.1.10
```

So long as data is being read from the input file, it will be written to
`record_<YYYYMMDD>T<HHMMSS>.dlt`, and be split approximately to 50M chunks.

### 3.10. Analysing Errors

#### 3.10.1. Using Extended Logging

Enable logging by modifying the `dltdump.dll.json` file to extend logging on the
console (other sections can remain, only modify the `Console` section below).

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Error"
    },
    "Console": {
      "IncludeScopes": true,
      "LogLevel": {
        "DltDump": "None",
        "DltDump.Terminal": "None",
        "RJCP.CrashReporter": "Warning",
        "RJCP.IO.Ports": "None",
        "RJCP.Diagnostics.Log": "Information",
        "RJCP.Diagnostics.Log.Dlt": "Information",
        "RJCP.Diagnostics.Log.Dlt.NonVerbose": "Information",
        "RJCP.Diagnostics.Log.Dlt.Pcap": "Information"
      }
    }
  }
}
```

When decoding an error, you'll see a log message (not normally printed by the
tool when processing) that contains an offset in hexadecimal.

#### 3.10.2. Getting the Position per Line

You may have notification of a tool indicating that there missing data. By
enabling logging in the application, and using the `--position` argument, one
can identify the start of each decoded log file in the input stream. For
example, a user complains with some unrelated custom tool a problem (a message
is missing), the same input stream can be given to DltDump with the `--position`
message, which can narrow down where in the log file the data may be corrupt (or
be incorrectly read) allowing to isolate the DLT packet which is the problem.

## 4. Further Work

The following features are not implemented

* Reading ARXML files are not supported
* DLT Version 2, as per AutoSAR DLT PRS R21-11 and later.

### 4.1. Contributing

Please raise an issue in GitHub if you're interested in contributing. I take a
strong focus on unit testing and design.

Design documentation can be found in this repository:

* DLT Dump
  * [Design Requirements](docs/designrequirements.md)
  * [Design Implementation](docs/design.md)
* DLT Decoder
  * [Decoder Design](../../TraceReader.Dlt/docs/DLT.DecoderDesign.md)
  * [Enabling Logging](../../TraceReader.Dlt/docs/DLT.Logging.md)

To build, see [RJCP.base](https://github.com/jcurl/RJCP.base/). Once all
repositories are checked out, you can run `dotnet build` within the base of this
repository (and indeed is a requirement before you can build using Visual
Studio).

### 4.2. Reporting Issues

Please report issues in the GitHub repository
[RJCP.DLL.Log](https://github.com/jcurl/RJCP.DLL.Log/issues).

This project is a best effort, unpaid and done in my spare time. Please take
this in consideration when requesting features and bugs. I am not directly
affiliated with AutoSAR or doing this on behalf of any paid project. This
implementation is done based on already existing open documentation.

In case of a crash of the tool, there will be an output on the command line. You
should include the .zip file that was automatically generated.

If you want to generate such a log file, provide the `--log` option:

```sh
dltdump --log <options>
```

Logging is done in memory, and only dumped in case of an unhandled exception, or
on usage of this command line. As such, very large input files may have logging
truncated. You should reduce the size of the input, attempt to make the problem
reproducible and provide any log files or samples that show the problem

## 5. References

For reference, see:

* [Covesa DLT Daemon](https://github.com/COVESA/dlt-daemon)
* [Covesa DLT Viewer](https://github.com/COVESA/dlt-viewer)
* [AutoSAR DLT PRS
  R20-11](https://www.autosar.org/fileadmin/user_upload/standards/foundation/20-11/AUTOSAR_PRS_LogAndTraceProtocol.pdf)
