# Design Requirements for DltDump <!-- omit in toc -->

This document covers the design requirements and the actual design of the
DltDump tool.

- [1. User Interface](#1-user-interface)
  - [1.1. General Usage](#11-general-usage)
  - [1.2. Command Line Options](#12-command-line-options)
  - [1.3. Help](#13-help)
  - [1.4. Input](#14-input)
  - [1.5. Input Format](#15-input-format)
    - [1.5.1. Online Mode](#151-online-mode)
  - [1.6. Output Format](#16-output-format)
    - [1.6.1. Position printed to the Console Output](#161-position-printed-to-the-console-output)
    - [1.6.2. Splitting Files](#162-splitting-files)
    - [1.6.3. Output File Name Template](#163-output-file-name-template)
    - [1.6.4. Overwriting Files](#164-overwriting-files)
  - [1.7. Search Filters](#17-search-filters)
- [2. Configuration File](#2-configuration-file)
  - [2.1. Logging](#21-logging)
- [3. Handling Crashes](#3-handling-crashes)

## 1. User Interface

### 1.1. General Usage

The `dltdump` tool shall be a command line tool. It's main purpose is to read
DLT files according to AutoSAR R20-11 or earlier (version 1), apply filters, and
write the output in various formats.

```cmd
dltdump [options] <input>
```

### 1.2. Command Line Options

This library shall use by
[RJCP.DLL.CommandLine](https://github.com/jcurl/RJCP.DLL.CommandLine). The style
of options should be the same as the operating system on which it is used.

* Windows: `/option:value`
* Linux: `-o value` or `--option value`

This document will use the Linux style for command line options.

### 1.3. Help

If the option `-?` or `--help` is provided, a help string is printed to the
console.

If the option `--version` is given, the version of the tool is printed to the
console. This is included in the `-?` option by default.

### 1.4. Input

The input can be:

* One more more files; or
* A URI

Files are those that are not a URI and are handled by the Operating System.

The following URI formats shall be supported:

* Serial - `ser:COM,baud,databit,parity,stopbit`
  * Opens the serial port with the
    [RJCP.DLL.SerialPortStream](https://github.com/jcurl/RJCP.DLL.SerialPortStream)
    library.
* TCP - `tcp://address:port`
  * A client TCP socket that connects to the address specified
* File - `file://filename`
  * Equivalent to a file

Other URIs might be supported in the future. But it is likely that a port of my
RemoteTerminalStream (a rewrite that is smaller, uses less buffering,
concentrates more on mapping URIs to streams than fancy features) will need to
be done first.

Only a single `ser:` or a single `tcp:` URI is allowed.

### 1.5. Input Format

The input format shall be obtained by the URI

* File name, or `file://`
  * Extension with `.dlt` indicates a file with a storage header.
  * Extension with `.pcap` or `.pcapng` indicates a PCAP file.
* `tcp://` - a DLT stream with only the standard header
* `ser:` - a DLT stream with the `DLS\1` marker

It should be possible to override the format with the `/format` option.

Globs on the command line should be provided by the shell. That means, globbing
on Linux will work, but Windows will need some scripting.

Files are read in the order they are provided on the command line. There is no
multiplexing of the files to mix time stamps or different ECUs.

#### 1.5.1. Online Mode

If the format is not using a storage header, and is reading from a live input,
such as the serial port or TCP stream, time stamps shall be applied to the
incoming messages based on the time stamp of the computer at that time.

### 1.6. Output Format

Output can be printed to the console (the default).

If the user provides the option `--output` or `-o`, then output will be
redirected to a file and contain a storage header. If no time stamp information
is available, the default time stamp of 1/1/1970 00:00 (UTC) should be used.

This allows reading an input file, and writing it again potentially rewriting
the format from DLT to text, or a new DLT file that can be loaded with other DLT
tools.

#### 1.6.1. Position printed to the Console Output

The option `--position` shall be supported to print the position of the packet
at the start of each line. This allows to debug potentially corrupted files by
knowing the offset of each packet in the file.

#### 1.6.2. Splitting Files

The option `-s` or `--split` shall be used to split the output file by size. The
output file size output will not be exact, as it should not be possible to split
a DLT line across two files.

#### 1.6.3. Output File Name Template

The output file name string can contain special characters, which will be
converted when writing.

* `%FILE%` - The input file name without the extension.
* `%CTR%` - The counter for the split file.
* `%CDATETIME%` - The current date/time for the first packet in this file as in
  the storage header. When reading from an input file, this is the contents of
  the storage header, else for online mode, this is the current time stamp. This
  will help to split files based on size and rename based on time stamp.
  * `%CDATE%` - The current date. The format is in `YYYYMMDD`.
  * `%CTIME%` - The current time. The format is 24h in `HHMMSS`.

#### 1.6.4. Overwriting Files

If the option `-f` or `--force` is provided, the output is overwritten, else the
command exits and preserves the existing file on the file system.

### 1.7. Search Filters

Various search filters shall be supported

* String filters for the payload
  * `-s` or `--string`: Filter for a partial string match
  * `-r` or `--regex`: Filter for a regular expression match
  * `-i` or `--ignorecase`: Ignore the case for `-s` or `-r`
* These provide filters on fields and are a list. The list will select any in
  the list, but anything not in the list for that field is filtered out.
  * `--ecuid=A`: Filter for one or more ECU IDs.
  * `--appid=A`: Filter for one or more APP IDs.
  * `--ctxid=A`: Filter for one or more Context IDs.
  * `--session=n`: Filter for one or more Session IDs.
  * `--type=T`: Filter for one or more DLT message types.
* These may be combined
  * `--verbose`: Filter for verbose messages only.
  * `--control`: Filter for control messages only.
  * `--nonverbose`: Filter for non-verbose messages only.

Some "grep" features should be supported

* `-A=n` or `--after-context`: Provide context for `n` lines after the match
* `-B=n` or `--before-context`: Provide context for `n` lines before the match

## 2. Configuration File

The tool shall check if there is a `dltdump.json` file present along side of the
binary. If so, it shall use that to load the options.

### 2.1. Logging

Logging shall be part of the JSON file. This can help to find unsupported
features, corruption within the DLT file for debugging. Logging is to use the
[RJCP.DLL.Trace](https://github.com/jcurl/RJCP.DLL.Trace).

## 3. Handling Crashes

The library
[RJCP.DLL.CrashReporter](https://github.com/jcurl/RJCP.DLL.CrashReporter) shall
be used, so that if the software crashes, a crash log can be provided.
