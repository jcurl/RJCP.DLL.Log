# DltDump <!-- omit in toc -->

DltDump is a tool that reads DLT Version 1 formatted files.

- [1. Introduction](#1-introduction)
  - [1.1. License](#11-license)
  - [1.2. Help](#12-help)
  - [1.3. Version Information](#13-version-information)
  - [1.4. Reading](#14-reading)
  - [1.5. Writing](#15-writing)
  - [1.6. Filtering](#16-filtering)
- [2. Common Use Cases](#2-common-use-cases)
  - [2.1. Recording Data from a TCP Stream](#21-recording-data-from-a-tcp-stream)
  - [2.2. Recording data sent via UDP](#22-recording-data-sent-via-udp)
  - [2.3. Recording data from a Serial Stream](#23-recording-data-from-a-serial-stream)
  - [2.4. Recording data from a Serial Stream in the Network Format](#24-recording-data-from-a-serial-stream-in-the-network-format)
  - [2.5. Converting a Recorded File to Text](#25-converting-a-recorded-file-to-text)
  - [2.6. Converting a PCAP File to DLT](#26-converting-a-pcap-file-to-dlt)
  - [2.7. Search for a String](#27-search-for-a-string)
  - [2.8. Search for a Regular Expression](#28-search-for-a-regular-expression)
  - [2.9. Filter for a Range of Dates](#29-filter-for-a-range-of-dates)
  - [2.10. Case Insensitive Search](#210-case-insensitive-search)
  - [2.11. Printing Context (like 'grep')](#211-printing-context-like-grep)
  - [2.12. Filtering for an Application Identifier](#212-filtering-for-an-application-identifier)
  - [2.13. Other Filters](#213-other-filters)
  - [2.14. Concatenating files](#214-concatenating-files)
  - [2.15. Splitting files](#215-splitting-files)
  - [2.16. Splitting files on Input based on Time Stamp](#216-splitting-files-on-input-based-on-time-stamp)
  - [2.17. Searching for Corruption on the Input DLT File](#217-searching-for-corruption-on-the-input-dlt-file)
- [3. Detailed Usage](#3-detailed-usage)
  - [3.1. Input Formats](#31-input-formats)
  - [3.2. Time Stamps](#32-time-stamps)
  - [3.3. Live Streams, Retries and Time outs](#33-live-streams-retries-and-time-outs)
  - [3.4. Output Files](#34-output-files)
  - [3.5. Output Formats](#35-output-formats)
  - [3.6. Filters](#36-filters)
  - [3.7. Context](#37-context)
  - [3.8. Splitting Output Files and Output Templates](#38-splitting-output-files-and-output-templates)
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
```

### 1.4. Reading

DltDump can read from:

* A file on disk;
  * As recorded by a logger, containing a storage header; or
  * binary data, recorded from a TCP stream (e.g. with `netcat`), or serial.
  * Recorded with a PCAP or PCAP-NG file
* a TCP server; or
* a Serial port.

### 1.5. Writing

DltDump can write the output to disk in the formats:

* Text format; or
* DLT format

### 1.6. Filtering

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

### 2.7. Search for a String

Search for a specific string in the output and print to the console

```sh
dltdump -s substring record.dlt
```

This is conceptually similar to doing a "grep" over the input.

### 2.8. Search for a Regular Expression

Likewise, .NET regular expressions are supported

```sh
dltdump -r "\\d+" record.dlt
```

This searches for the regular expression `\d+`, which matches for all lines that
have a number.

### 2.9. Filter for a Range of Dates

Return all time stamps that are either after, or before a particular date. This
helps create a log file that has only a specific range.

This specific example will only return lines in the range 18th June 2022, time
10:00:00 to 10:05:00.

```sh
dltdump --not-before 2022-06-18T10:00:00 --not-after 2022-06-18T10:05:00
```

### 2.10. Case Insensitive Search

If the string to be sought should be case insensitive, add the option `i`:

```sh
dltdump -i -s substring record.dlt
```

### 2.11. Printing Context (like 'grep')

If you want to see the lines before and after a match, use the context options
`-A` (after) and `-B` (before):

```sh
dltdump -s substring -A 10 -B 2 record.dlt
```

### 2.12. Filtering for an Application Identifier

If you only want to see logs from a particular application on the console:

```sh
dltdump --appid APP1 record.dlt
```

Or if you want to output the result to a new file `filtered.dlt`

```sh
dltdump --appid APP1 --output filtered.dlt record.dlt
```

### 2.13. Other Filters

The options for filtering are:

* `--ecuid`: Filter for an ECU identifier
* `--appid`: Filter for an application identifier
* `--ctxid`: Filter for a context identifier
* `--sessionid`: Filter for a session identifier
* `--type`: Filter for message types
* `--verbose`: Filter for verbose messages
* `--nonverbose`: Filter for non-verbose messages (excluding control messages)
* `--control`: Filter for control messages

You can provide the filters, they have a logical "and" relationship. Providing
the same filter option (e.g. `appid`) has a logical "or" relationship.

### 2.14. Concatenating files

If you have multiple files, you can parse and join them together. The ordering
is important (no sorting of the input files are made):

```sh
dltdump --output concat.dlt input1.dlt input2.dlt input3.dlt
```

Or join many PCAP files into one DLT file

```sh
dltdump --output result.dlt file001.pcap file002.pcap file003.pcap
```

### 2.15. Splitting files

If you have a large file, you can split it up into many smaller files. This
example takes the input `record.dlt` and splits it up into files named
`split_001.dlt`, `split_002.dlt`, etc. each being approximately 100MB in size.

```sh
dltdump --split 100M --output split_%CTR%.dlt record.dlt
```

### 2.16. Splitting files on Input based on Time Stamp

An extension of splitting the files is to record data, and to split the output
into files of particular sizes, with the file name having the time stamp when
the file was started.

```sh
dltdump --split 50M --output record_%CDATETIME%.dlt tcp://192.158.1.10
```

### 2.17. Searching for Corruption on the Input DLT File

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
  Genivi DLT Viewer application.

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

### 3.4. Output Files

Writing to a file is done with the `--output` option. If the file already exists
on disk, it will by default, not be overwritten. To override this, provide the
option `--force` which forcibly overwrites the output file that already exists.

Note, if the file being written is one of the inputs, or was written by this
specific instance of `dltdump`, the `--force` option has no effect, and the file
will never be overwritten.

On Linux, the file names are compared between the inputs and the outputs, so it
may be possible to overwrite if you "trick" `dltdump` with soft links and the
such. On Windows, it queries the file system if files are the same.

### 3.5. Output Formats

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

### 3.6. Filters

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

### 3.7. Context

To show context at the time of a match, use the `--before-context` or
`--after-context` options, and give it the number of lines of context. The
context is for the file, without a filter applied. Should you want context for a
specific application identifier for example, you should first filter for the
application identifier on disk, and then run the tool a second time to filter
for a string with context.

### 3.8. Splitting Output Files and Output Templates

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

## 4. Further Work

The following features are not implemented

* Reading FIBEX or ARXML files are not supported
* Reading from UDP network streams
* Filtering based on the logger time stamp

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

* [Genivi DLT Daemon](https://github.com/COVESA/dlt-daemon)
* [Genivi DLT Viewer](https://github.com/COVESA/dlt-viewer)
* [AutoSAR DLT PRS
  R20-11](https://www.autosar.org/fileadmin/user_upload/standards/foundation/20-11/AUTOSAR_PRS_LogAndTraceProtocol.pdf)
