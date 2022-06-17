# Design and UML for DltDump <!-- omit in toc -->

This document tries to organize the [Design Requirements](designrequirements.md)
for construction of the software (identify the components and features for
implementation in an incremental manner).

- [1. Application Use Cases](#1-application-use-cases)
  - [1.1. Search Flow Model](#11-search-flow-model)
- [2. Layered Design](#2-layered-design)
  - [2.1. Overview of the Layers](#21-overview-of-the-layers)
    - [2.1.1. Root](#211-root)
    - [2.1.2. View](#212-view)
    - [2.1.3. Application](#213-application)
    - [2.1.4. Services](#214-services)
    - [2.1.5. Domain Model](#215-domain-model)
    - [2.1.6. Infrastructure](#216-infrastructure)
    - [2.1.7. Framework](#217-framework)
  - [2.2. View](#22-view)
    - [2.2.1. Command Line Options](#221-command-line-options)
    - [2.2.2. The Command Factory](#222-the-command-factory)
  - [2.3. Application](#23-application)
    - [2.3.1. Help and Version](#231-help-and-version)
    - [2.3.2. Filtering and Output](#232-filtering-and-output)
      - [2.3.2.1. Initialization](#2321-initialization)
      - [2.3.2.2. Instantiating a Stream from the Input Path](#2322-instantiating-a-stream-from-the-input-path)
        - [2.3.2.2.1. Implementing a IInputStreamFactory](#23221-implementing-a-iinputstreamfactory)
      - [2.3.2.3. Decoder Factory](#2323-decoder-factory)
        - [2.3.2.3.1. Adding a new InputFormat](#23231-adding-a-new-inputformat)
      - [2.3.2.4. The Output Stream, Context and Filter](#2324-the-output-stream-context-and-filter)
  - [2.4. Domain](#24-domain)
    - [2.4.1. InputStreamFactory and IInputStream](#241-inputstreamfactory-and-iinputstream)
    - [2.4.2. DLT Trace Decoder (AutoSAR PRS format)](#242-dlt-trace-decoder-autosar-prs-format)
    - [2.4.3. DLT Trace Decoder for PCAP and PCAPNG](#243-dlt-trace-decoder-for-pcap-and-pcapng)
      - [2.4.3.1. PCAP-NG Design](#2431-pcap-ng-design)
      - [2.4.3.2. Supported Link Types for PCAP and PCAPNG](#2432-supported-link-types-for-pcap-and-pcapng)
      - [2.4.3.3. Handling Connections within a PCAP File](#2433-handling-connections-within-a-pcap-file)
        - [2.4.3.3.1. IPv4 Packet Fragmentation by the Network Stack](#24331-ipv4-packet-fragmentation-by-the-network-stack)
        - [2.4.3.3.2. Issues with IP Fragmentation Reassembly](#24332-issues-with-ip-fragmentation-reassembly)
        - [2.4.3.3.3. DLT Packet Fragmentation by the Client](#24333-dlt-packet-fragmentation-by-the-client)
      - [2.4.3.4. Unsupported Features for PCAP / PCAPNG Decoding](#2434-unsupported-features-for-pcap--pcapng-decoding)
    - [2.4.4. Trace Output](#244-trace-output)
      - [2.4.4.1. Console Output](#2441-console-output)
      - [2.4.4.2. Text File Output](#2442-text-file-output)
      - [2.4.4.3. DLT File Output](#2443-dlt-file-output)
      - [2.4.4.4. File Templates for DLT and Text Output](#2444-file-templates-for-dlt-and-text-output)
      - [2.4.4.5. Input File Concatenation](#2445-input-file-concatenation)
      - [2.4.4.6. Splitting Output Files](#2446-splitting-output-files)
      - [2.4.4.7. Detailed Logic for Splitting and Concatenating Files](#2447-detailed-logic-for-splitting-and-concatenating-files)
      - [2.4.4.8. IOutputStream Object Lifetime](#2448-ioutputstream-object-lifetime)
      - [2.4.4.9. On Flush](#2449-on-flush)
    - [2.4.5. The Filter and the Context](#245-the-filter-and-the-context)
      - [2.4.5.1. Output Chaining](#2451-output-chaining)
      - [2.4.5.2. Context Implementation](#2452-context-implementation)
    - [2.4.6. Decoder Extension on Line Decoding](#246-decoder-extension-on-line-decoding)
      - [2.4.6.1. DltTraceDecoder writing to IOutputStream](#2461-dlttracedecoder-writing-to-ioutputstream)
  - [2.5. Infrastructure](#25-infrastructure)
    - [2.5.1. Version Information](#251-version-information)

## 1. Application Use Cases

The main purpose of this application is to read a file, filter the contents, and
output the contents in the format the user chooses.

### 1.1. Search Flow Model

The following diagram shows the high level flow of how searching through a DLT
file should work.

![Search Flow](out/diagrams/searchflow/AppSearch.svg)

The user provides one or more input files, which are read as input streams.
These input streams may be files, TCP streams or serial port streams. In the
future, they could be other streams (e.g. HTTP / HTTPS / FTP or UDP packets).

The input stream is given to a decoder. This decoder knows how to read the input
stream. It could be a DLT stream with or without a header (storage or serial
marker), a PCAP or PCAPNG file, or another decoder in the future for different
encapsulation formats. Each input stream is one continuous DLT log.

If the output is written to the console, the DLT decoder provided by
`RJCP.Diagnostics.Log.Decoder.DltTraceDecoder` produces a trace line, where as
when producing a binary file, it must be captured during the decoding phase to
have access to the original packet data.

The history is used if context before or after a filter is required, and outputs
the lines that should be recorded. The filter is used on the decoded trace line
to identify if it should be recorded or not. The Console Output and File Output
both contain the context and filter, which differs essentially in where it is
instantiated and used, rather than the functionality.

The Console output just writes the output to the console.

The binary writer takes a DLT binary packet (captured from the input stream) and
writes it to an output file. The file could be split and named based on various
rules, not modifying the original input stream. The naming of the file may be
known upfront, or only after the first line is written (if the name is based on
the timestamp of the first line for example).

From this simple idea, it is easy to extend the application later to support
compressed files, different container formats, more complex filters. It is an
extensible (even if simple) design.

## 2. Layered Design

The first part contains a very quick overview of how the layers are defined. The
second part (consisting most of this section) is a detailed breakdown of the
components within the layers.

### 2.1. Overview of the Layers

The software folder structure should have the following namespaces, with `Root`
being in the root namespace.

 ![Layers](out/diagrams/layers/Layers.svg)

The layered design chosen to structure software. A higher layer can access an
element in the lower layer, but not the reverse. There is no strict rule that
only the immediate layer is accessible, so that the application layer might
handle a reference to the infrastructure layer, or the application layer uses
elements from the framework layer.

#### 2.1.1. Root

This is where the program entry points are. It instantiates the View.

#### 2.1.2. View

This is the user interface. This is a command line application, so it gets the
options the user provided and executes the use cases in the application layer.
If a graphical user interface should be created, this layer is modified.

#### 2.1.3. Application

These are classes that represent various application use cases.

#### 2.1.4. Services

Generally thin classes that offer specific services to the View and the
Application layers. It might depend on the infrastructure or framework. It
usually offers thin functionality that the View or the Application uses.

For the command line, there is a special service, called the `Terminal` which
formats strings to print to the terminal. In a GUI application, a ViewModel
would define the classes that the View binds to, but the application updates.

#### 2.1.5. Domain Model

The Domain Model contains the parts which business logic can combine together to
execute use cases. Elements of the domain model can be:

* Input Streams: Mapping files and URIs to streams
* Decoders: Know how to read the input streams
* Filters: Determine if a line matches a user filter
* Output: Knows how to write the output, either console or binary
* Context: Keeps buffers of lines for showing context on a filter match

Each element of the domain model on its own is independent, and has minimal
input on other elements of the domain model. The Application layer ties the
Domain model together. As the domain becomes larger, application services can be
introduced.

#### 2.1.6. Infrastructure

This contains code local to this application. It could be considered reasonably
generic, but for now only used in this application. Importantly, it doesn't
contain any thing related to policy or business logic, but only supporting such.

#### 2.1.7. Framework

This encapsulates reusable library components, abstracting the runtime or the
Operating System.

### 2.2. View

The view is the component that reads the command line and executes the command
as given on the command line. It's purpose is to parse the user input (the user
interface) and instantiate the appropriate application which does the work.

#### 2.2.1. Command Line Options

The command line options are provided by the entry point to the program. Parsing
the command line options to a class is done by the infrastructure component
`RJCP.Core.CommandLine`.

![Command Line](out/diagrams/viewcmdline/View.CommandLine.svg)

The command line options that are supported are:

Help and Information:

* `-?` or `--help`
* `--version`

Input

* This is just the first non-option argument. It can be a file or a URI which is
  parsed

Writing Output Files used by the Binary Writer.

* `-o` or `--output`: Provides a string that might also contain templates
* `-f` or `--force`: Tells the Binary Writer to overwrite the file if it already
  exists.
* `-s` or `--split`: How large to write each file before splitting.

Console Output

* `--position`: Print the position in the input stream for each packet

Filter Options (within the Filter)

* `-s` or `--string`: Searches the text content of a DLT line for a partial
  match of the string.
* `-r` or `--regex`: Searches the text content of a DLT line for a regular
  expression match, using .NET regular expression rules.
* `-i` or `--ignorecase`
* `--ecuid=A`: Filter for one or more ECU IDs.
* `--appid=A`: Filter for one or more APP IDs.
* `--ctxid=A`: Filter for one or more Context IDs.
* `--session=n`: Filter for one or more Session IDs.
* `--type=T`: Filter for one or more DLT types.
* `--verbose`: Filter for verbose messages only.
* `--control`: Filter for control messages only.
* `--nonverbose`: Filter for non-verbose messages only.

Context Options, affecting how lines are printed before and after a filter
match:

* `-A=n` or `--after-context`: Provide context for `n` lines after the match
* `-B=n` or `--before-context`: Provide context for `n` lines before the match

Special options:

* `--log`: Dump the internal memory log to the file system for analysis and
  debugging of the decoder (e.g. of corrupt files, or detection of unsupported
  features).

#### 2.2.2. The Command Factory

The View essentially has two commands from the command line - to print help and
the version, or to filter inputs.

This diagram shows how the command line options are read, and then a factory is
used to create the appropriate command. This can be generated directly from the
options.

![Command Factory](out/diagrams/viewcmdfactory/View.CommandFactory.svg)

If more commands are needed on the command line, the factory can be extended.

### 2.3. Application

#### 2.3.1. Help and Version

To get a simple idea of the minimalistic design to print the version and help
information on the console from the View:

![Help View](out/diagrams/appversionhelp/App.Help.svg)

The Help Command retrieves lines of text that it should print, and then formats
and prints these on the command line. The access to the console should only be
from the View.

#### 2.3.2. Filtering and Output

The filter application takes the inputs; and constructs and configures the
objects necessary for:

* sets up a loop if there are more than one input streams
* the input stream abstracted from the input URI
* the decoder, which is told the input format and the input stream
* a filter for a trace line and also maintains context if required
  * The filter portion handles the `--string`, `--regex`, `--ignorecase` if
    filtering strings or regexes, `--ecuid`, `--appid`, `--ctxid`, `--session`,
    `--type`, `--verbose`, `--control`, `--nonverbose` options
  * The context portion is required for `--after-context` or `--before-context`
  * This must be before the binary writer or the console writer.
* a binary writer that can split and rename files as necessary if writing to a
  file
  * handles the `--output`, `--force`, `--split` options
  * it needs input information about the names of files, time stamp information.
    The module for handling the environment data for the application should be
    separate as there are multiple components to update the state, while the
    binary writer needs the state.
    * The input file name being processed. Known by the loop when opening the
      input stream.
    * The counter is internal, but will need to be reset at the end of the loop
    * The date/time is known only after the first line is decoded
  * must be run from within the decoder thread, where the input stream bytes and
    decoding information is available
* a console writer that prints the line to the console
  * handles the `--position` option
  * Is handled after decoded

The next stages describe the following stages:

* The input stage
* The decoder
* The output stage for console writing
* The output stage for file writing

##### 2.3.2.1. Initialization

The View layer handles the user interface and has the list of inputs. It shall
interpret the input commands and construct a `FilterConfig` object which
describes how the `FilterApp` should configure itself. Most of the options from
the command line will be copied to the `FilterConfig` object.

![Filter Initialization](out/diagrams/appfilterinit/App.Filter.Initialization.svg)

The `FilterApp` initializes all objects it needs from the `Domain` layer.

It will loop over all the inputs given in the `FilterConfig`. The
`InputStreamFactory` knows how to take the input file which is a URI string, and
generates an appropriate stream. This stream could be a file, serial port, or a
TCP stream. The `IInputStream` provides additional methods which can
asynchronously connect the stream if it is required (files don't need to be
connected, where network streams do).

The `IInputStream` provides additional information about how to instantiate a
decoder. It can indicate if the URI is a "live" stream, where time stamps are
generated based on the current system time, or if the time stamp is derived from
the input file. This information is needed when constructing the decoder.

It will also "guess" the input file format based on the URI that was given,
which determines the decoder that should be instantiated.

Because the inputs are looped, each input is decoded one after the other, there
being no multiplexing of input files to generate a single file (e.g. no sorting
of the inputs or their time stamps are considered in this design).

##### 2.3.2.2. Instantiating a Stream from the Input Path

The input path can be a file name or a URI. The diagram in the previous section
is a simplification. To enable testing and extension for reading various input
streams, the following design is taken.

In the diagram below, it shows the `DltFileStreamFactory` and the
`DltFileStream`, but the same is also for the serial and network streams.

![Input Stream](out/diagrams/InputStreamFactory/InputStreamFactory.svg)

The `FilterApp` instantiates only through the `InputStreamFilter`. This has an
internal mapping of a URI scheme to a second factory, e.g.:

* `DltFileStreamFactory`: creates a `DltFileStream`, opening a `FileStream`,
  that is for DLT files with a storage header. The URI is `file://` or just a
  local path. Note, this can extend for different encapsulation formats (e.g.
  for PCAP or PCAPNG files).
* `DltSerialStreamFactory`: creates a `DltSerialStream`, opening a
  `SerialPortStream` (another project of mine), that is for DLT with the the
  `DLS\1` header. The URI starts with `ser:`.
* `DltNetworkStreamFactory`: creates a `DltNetworkStream`, opening a TCP stream
  for streams that have the DLT server. The URI starts with `tcp:`.

Other schemes can be added later, such as `http://`, or `https://` that can use
underlying .NET stream implementations. The decoder just needs a stream and to
know if the timestamps are generated locally, or come from the stream.

###### 2.3.2.2.1. Implementing a IInputStreamFactory

When implementing a new `IInputStreamFactory`:

* Create a new class that implements `IInputStream`.
* Create a new class that implements `IInputStreamFactory`, preferably derived
  instead from `InputStreamFactoryBase`.
* The new factory checks the stream and instantiates the specific
  `IInputStream`.
* Modify the `InputStreamFactory` class to add the scheme and reference the
  newly created factory class.

The sequence of how the `FilterApp` uses the `IInputStreamFactory` is given:

![InputStreamCreate](out/diagrams/inputstream/Domain.InputStream_Sequence.svg)

##### 2.3.2.3. Decoder Factory

The decoder factory is an implementation that is told what kind of decoder to
create based on various inputs given by the `FilterConfig` class. The decoder
factory is a domain object as it is one class that can decide which decoder to
create, where as the framework component factory only creates a decoder for a
specific format.

The decoder factory is given the input file format from the `FilterConfig`,
which it has presumably received from the `IInputStream`.

* Checks the type of URI from `IInputStream`
* Creates a decoder based on TCP, Serial, File. Online mode or not.
* Handles PCAP and UDP streams

###### 2.3.2.3.1. Adding a new InputFormat

Adding a new `InputFormat` is a little more work, e.g. reading a WireShark file,
that contains DLT packets transmitted as UDP. It requires:

* Adding a new element to `InputFormat`.
* If there are aliases to the `InputFormat`, the `CheckInputFormat` method in
  `CmdOptions` should handle this.
* The `FilterApp` should handle the case if the user explicitly specified this
  format on the command line
* The `InputStreamFactory` should know how to create the `InputStream`. When
  reading from a file, you might want to modify `DltFileStream`.
* All the `IOutputStream` objects should handle this input format. It should
  know how to construct the binary version of the output packet, e.g.
  `DltOutput`.
* The `DltDumpTraceReaderFactory` then creates a specific instance of the
  factory for reading the input format. You'll need to implement the decoder as
  well.

##### 2.3.2.4. The Output Stream, Context and Filter

The `IOutputStream` is a simple interface that knows how to write the output DLT
line. It has a `Write` methods, one that takes a `DltTraceLineBase` and
optionally a `ReadOnlySpan<byte>` which describes the packet.

The various output streams may be one of:

* To the console; or
* To a text file; or
* To a binary file.

The output can be prefixed by another object implementing the `IOutputStream`
which can filter the data, and optionally provide context. This allows the
`FilterApp` to abstract and not care about the output stage, while being
flexible about what and how the data is written. This is the Filter and Context
information.

Text output, that doesn't require the original binary packet, can be written
from the `FilterApp` after getting a line from the decoder.

Output that must write data using binary packets must be run in the thread that
is doing the decoding, i.e. as part of the decoder, because a `DltTraceLineBase`
does not maintain the raw binary data. The decoder must call the `IOutputStream`
to pass the information, and so the `FilterApp` must provide the details to the
decoder factory.

### 2.4. Domain

The domain layer contains logic specific to this application or the problem
domain. It implements the business logic specific to reading and writing DLT
data.

#### 2.4.1. InputStreamFactory and IInputStream

This component handles parsing a string input, and determining if this is a file
to read, or a URI for creating a stream.

The output of the `InputStreamFactory` is an object of type `IInputStream` which
returns a stream and other metadata:

* If time stamps are to be obtained from the stream, or from the PC
* If the stream must be "connected", such as a network stream
* The assumed type of stream input

| Input URI            | File Format                       | Connect? | Time Stamp               |
| -------------------- | --------------------------------- | -------- | ------------------------ |
| `file.dlt`           | Storage Header                    | No       | Storage Header from file |
| `file.pcap`          | Wireshark, UDP, no storage header | No       | Wireshark Time stamp     |
| `file.pcapng`        | Wireshark, UDP, no storage header | No       | Wireshark Time stamp     |
| `file://../file.dlt` | Storage Header                    | No       | File                     |
| `ser:..`             | `DLS\1`                           | No       | Local (online mode)      |
| `tcp://...`          | DLT                               | Yes      | Local (online mode)      |

Connecting the stream should be done by the `FilterApp` application in an
asynchronous manner before giving to the decoder.

#### 2.4.2. DLT Trace Decoder (AutoSAR PRS format)

There are three decoders implemented in the DLT package for reading a stream of
bytes and interpreting DLT contents, implemented as per the [AutoSAR R20-11
PRS](https://www.autosar.org/fileadmin/user_upload/standards/foundation/20-11/AUTOSAR_PRS_LogAndTraceProtocol.pdf)
standard (DLT version 1).

If writing to the console, the DLT Trace Decoder used can be instantiated from
the decoders already defined. The output of the decoder is the result of the
decoder `GetLineAsync()` which is then used to apply to a filter and then print
on the console.

If writing to a file, the DLT Trace Decoder must also provide the input buffer
as well as other metadata. The original packet is required so that in case of a
match, that exact buffer can be written to the binary writer.

Further, the input may not have a storage header, and a storage header must be
constructed. Information about the decoded packet is part of the
`IDltLineBuilder` object maintained internally in the decoder and must also be
made present to the binary writer.

#### 2.4.3. DLT Trace Decoder for PCAP and PCAPNG

A decoder can be implemented that can decode either a PCAP or PCAPNG file
format, with detection based on the initial 4 bytes of the file:

* `0x0A0D0D0A`: A
  [PCAP-NG](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcapng.html) file
* `0xA1B2C3D4`: A
  [PCAP](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcap.html) file, big
  endian, with time resolution microseconds
* `0xA1B23C4D`: A
  [PCAP](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcap.html) file, big
  endian, with time resolution nanoseconds
* `0xD4C3B2A1`: A
  [PCAP](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcap.html) file,
  little endian, with time resolution of microseconds
* `0x4D3CB2A1`: A
  [PCAP](https://pcapng.github.io/pcapng/draft-ietf-opsawg-pcap.html) file,
  little endian, with time resolution of nanoseconds

For a quick reference, see [PCAP.md](pcap.md).

The decoder derives from `ITraceReader<DltTraceLineBase>`, the same as the other
DLT decoders. It does not derive from `DltTraceDecoderBase` as the PCAP decoder
does not implement decoding the DLT protocol, but rather uses composition, and
decodes only the PCAP packets, passing the DLT data along.

![PCAP Factory](out/diagrams/dltpcaptracedecoderfactory/DltPcapTraceReaderFactory.svg)

The instantiated `DltPcapTraceDecoder` then reads the first four bytes,
determines the format that should be used to read, instantiating either a
`DltPcapLegacyDecoder` or a `DltPcapNgDecoder`. These two classes are
responsible for extracting the Ethernet frames and obtaining the UDP packet
data.

![PCAP Decoder](out/diagrams/dltpcaptracedecoder/DltPcapTraceDecoder.svg)

The `PacketDecoder` receives a valid PCAP frame. It extracts the time stamp from
the frame, and decodes the frame dependent on the Link Layer type as determined
by the relevant PCAP decoder. It should then determine the offset where the
EtherType starts (e.g. 0x8100 for 802.1q VLAN header, or 0x0800 with the IP
version of 0x04). It checks that the packet is valid (has the right protocol
version, e.g. 17 for UDP, the correct destination port, etc.). It may also
support IP fragmentation, concatenating fragmented frames from the wire and
sending the complete packet to be decoded.

The `PacketDecoder` has a custom `DltPcapNetworkTraceFilterDecoder` (derived by
the `DltTraceDecoder` that actually knows how to decode the DLT frame) which has
the main responsibility setting the logging time stamp on each line (by
overriding the `ParsePrefixHeader`) and possibly writing to an `IOutputStream`.

The implementation assumes that each packet is complete, that is, it contains an
integer number of complete DLT packets. As such, after decoding each Ethernet
frame, the underlying DLT decoder should be flushed.

##### 2.4.3.1. PCAP-NG Design

Writing a PCAP-NG decoder is a little more work, as there may be multiple
section header blocks, and per block, there may be multiple interfaces. This
allows a lot of flexibility. The PCAP-NG decoder shall decode all interfaces.

![PCAP-NG Decoder](out/diagrams/dltpcapngtracedecoder/DltPcapNgTraceDecoder.svg)

The decoding of the blocks are done in `DltPcapNgDecoder`, and the following
blocks are supported:

* `0x0A0D0D0A` - Section Header Block
* `0x00000001` - Interface Description Block
* `0x00000006` - Enhanced Packet Block

The `BlockReader` is stateful, as it decodes blocks, it can maintain state. This
is important as the `SectionHeaderBlock` defines the endianness. The section
header is expected to be the first block. If it is not found, an
`UnknownPcapFileFormatException` will be raised.

An indexed list of `InterfaceDescriptionBlock`s will instantiated and kept by
the decoder, the file format numbers the identifiers from 0 and increasing. The
Interface Description Block defines the interface name, the time resolution and
the link type. If no time resolution is present, a default will be provided.

Decoding an Enhanced Packet Block will be done in place by the `BlockReader`
with the `DecodeBlock()` method. The buffer is the complete Enhanced Packet
Block. It won't create an object of type `IPcapBlock`. Once decoding is
finished, the buffer and packet are discarded. The Enhanced Packet Block
contains the interface that received the packet (and thus which
`InterfaceDescriptionBlock` should decode the DLT packet). The time stamp from
the Enhanced Packet Block is converted based on the options of the interface.

The `InterfaceDescriptionBlock.DecodePacket()` method shall take the Ethernet
frame as an input, the same input as `PacketDecoder`. That allows easier
modification in the future if the "Simple Packet Block" also needs to be
implemented for decoding.

It should not be possible to instantiate the `IPcapBlock` derived objects
outside of the `BlockReader`. They are returned by the respective `BlockReader`
method:

* `GetHeader()`: Returns just the information about the block identifier and the
  length. This allows the decoder to advance to the next block in the file.
* `GetBlock()`: Decodes the block for the following block types:
  * `0x0A0D0D0A` - Section Header Block
  * `0x00000001` - Interface Description Block
  * Others are ignored
* `DecodeBlock()`: Decodes the block and returns the DLT decoded content for the
  following block types:
  * `0x00000006` - Enhanced Packet Block

##### 2.4.3.2. Supported Link Types for PCAP and PCAPNG

The `PacketDecoder` should support the following Link Types defined by [Link
Type](https://www.tcpdump.org/linktypes.html)

* `LINKTYPE_ETHERNET` = 0x01 (1)
* `LINKTYPE_LINUX_SLL` = 0x71 (113)
  * With a 6 byte address, being the MAC of the interface being recorded.

##### 2.4.3.3. Handling Connections within a PCAP File

There are two scenarios in which UDP packets may not fit within a single UDP
frame:

1. The frame is so large, that there is IP fragmentation of the UDP packet. This
   requires the decoder to manage IP fragmentation and to reconstruct the
   original packet as per
   [RFC791](https://datatracker.ietf.org/doc/html/rfc791).
2. The DLT writer application splits the DLT packet into smaller chunks and
   transmits multiple UDP frames (the IP frame is not fragmented, but the
   payload is). This requires treating the input sequence of UDP packets as a
   stream, separating each connection.

The design to handle these two cases are presented:

![PCAP](out/diagrams/dltpacketdecoder/DltPcapPacketDecoder.svg)

###### 2.4.3.3.1. IPv4 Packet Fragmentation by the Network Stack

In the first case of IP fragmentation, the IPv4 source address, destination
address and fragmentation fields (identifier, offset, more fragments) are to be
tracked as per [RFC791](https://datatracker.ietf.org/doc/html/rfc791). The
protocol is always UDP. The reassembled packet is then given to the
`DltPcapNetworkTraceFilterDecoder` for decoding as if it were a single packet.

For each packet received, `PacketDecoder` then checks:

* If the packet is not fragmented, the private method
  `PacketDecoder.DecodeDltPackets` is called with the UDP buffer (the first 8
  bytes contains the ports, length and checksum). The `DltTraceDecoder` is
  retrieved based on the (source:port, destination:port) pair via
  `Connection.GetDltDecoder`.
* If the packet is fragmented (either that the fragmentation offset is non-zero,
  or the MF bit is set), the private method `PacketDecoder.DecodeDltFragments`
  is called to create or add to the list of fragments for this fragmentation
  identifier. The ordering of the fragments are not defined. It is observed on
  Linux hosts transmitting fragmented IP packets that often the last packet is
  transmitted first, followed by the remaining packets. This implementation
  makes no assumption of the ordering.
  * When decoding DLT fragments, the payload of the IP fragment is given
    (including the UDP header). The IP fragment must always be reassembled, even
    if the source port and destination port do not match a DLT packet. That is
    because an IP fragment later could use this fragment and result in possible
    dataloss otherwise. If the result of `IpFragments.AddFragment` returns:
  * `IpFragmentResult.Incomplete`, no decoding is required.
  * `IpFragmentResult.Reassembled`, the packet should be decoded, getting the
    UDP source and destination ports, using `Connection.GetDltDecoder` and all
    fragments are given to the decoder.
  * Other results are an error, the packets with the fragmentation offset are
    discarded (after logging) and the packet is added to a new `IpFragment`.

###### 2.4.3.3.2. Issues with IP Fragmentation Reassembly

The following scenarios can occur in a PCAP/PCAP-NG file:

* A packet is lost
* A packet is duplicated (re-sent)
* A packet is incorrect with overlap

If a packet is lost, it is likely to be never retransmitted. If this is the
case, we'll have a "stale" entry in the `IpFragments` dictionary collection for
the fragmentation identifier. We assume that the packets arrive always in
ordered time. Thus, if the time stamp of the current fragment exceeds 15 seconds
of the current time stamp entry for the fragment, we discard all elements and
treat this new fragment as the first element. A log message is emitted
indicating the position of all packets that were discarded.

If the fragments already contain the first packet, or the last packet for this
fragmentation identifier, all existing fragments are discarded and this becomes
the "new" fragment. A log message is emitted indicating the position of all
packets that were discarded. We take advantage that an Operating System is
likely to transmit the first fragment or the last fragment first over the wire.

If a packet is sent with overlap, its position is logged and is discarded. This
can occur when a packet is lost and the time overlap is less than 15 seconds. A
host operating system should try and minimize the reuse of the fragmentation
identifier which should minimize the occurrence of this happening. This overlap
may occur for high data output rates when a packet is lost, but still very
unlikely to occur as the previous check for the first/last fragment will likely
occur prior.

Overlap can also occur when a packet is resent, or logged twice, in which case,
there is no loss of data by ignoring this packet.

If a packet is incorrect and has overlap but due to a corrupted fragment, no
attempt will be made to try and reconstruct the packet by merging the overlap.
It is expected that routers, switches and Operating Systems implement
fragmentation correctly.

It should be noted that PCAP-NG files where the same packet is seen on multiple
interfaces will not see duplicated packets, as interfaces are seggregated
through the `InterfaceDescriptionBlock`.

###### 2.4.3.3.3. DLT Packet Fragmentation by the Client

In the second case of the client fragmenting the payload and transmitting
multiple packets, the virtual connection must be tracked, each connection in the
PCAP file treated as a stream. This requires the fields source address and port,
destination address and port to be tracked. The protocol is always UDP.

If packets are not treated as such, interleaved packets from two different
sources (identified by their address and port) in the PCAP can interfere with
each other. Imagine two sources, `1` and `2`. We'll use the notation `1.1` and
`1.2` to indicate the first and second fragment of the first source. If the
packet arrives in the PCAP stream as `1.1`, `2.1`, `1.2`, `2.2`, then both must
be treated independently as `1.1`, `1.2` and `2.1`, `2.2` and thus two different
instances of the `PacketDecoder`. Else the packet `2.1` will corrupt the
decoding of the previous packet `1.1` for example.  It is assumed here that
packets are not reordered in the PCAP file, as there is no information on the
ordering.

To handle the multiple streams, the `PacketDecoder` decodes the IP end points
for the virtual connection. Each combination of `source:port`,
`destination:port` define the virtual connection between the device transmitting
and the addresses to receive (which can be a multicast sink). As each virtual
connection has its own `DltPcapNetworkTraceFilterDecoder`, this handles the
second case avoiding corruption should DLT streams be interleaved.

##### 2.4.3.4. Unsupported Features for PCAP / PCAPNG Decoding

The following limitations are applied:

* As I don't have examples of the FCS field in the PCAP file, it is expected
  that this value is not present (the `P` bit is `0`).
* Filtering will be based on the destination port only, default to 3490.
* 802.1q double tagging isn't supported.
* Only IPv4 will be supported, until real-world examples exist of IPv6 which can
  be tested with.

#### 2.4.4. Trace Output

There are multiple output sinks, each shall also implement the `IOutputStream`.
The sinks are:

* The console; or
* A text file (UTF8); or
* A binary DLT output

It shall be easy to extend with further sinks. A diagram of how the
`IOutputStream` object is to be created:

![IOutputStreamFactory](out/diagrams/outputstream/Domain.OutputStreamFactory.svg)

A mapping of the `OutputFormat` to the object:

| OutputFormat | Output Object    | Supports Binary |
| ------------ | ---------------- | --------------- |
| `Console`    | Console Output   | False           |
| `Text`       | Text File Output | False           |
| `Dlt`        | Dlt File Output  | True            |

If `OutputFormat.Automatic` is chosen, it should choose the output object based
on the output file name:

* `null` reference, `CON:` or `/dev/stdout`: Console Output
* `*.dlt`: DLT File Output
* Any other file name: Text File Output

##### 2.4.4.1. Console Output

This class knows only how to output the DLT trace line to the console. The
construction of this object must know if the position should be printed or not.

##### 2.4.4.2. Text File Output

For consistent output to a text file in UTF8 format, it is possible to write to
a text file output. When using shell redirection on Windows Power Shell, the
output is written as UCS2 and can generate large files. The same program used on
the Windows Command Shell prints output using the current code page. As such,
console output can provide inconsistent results.

The construction of this object must know if the position should be printed or
not.

It must maintain a text stream, the file name of the output, automatically split
the output on each line as required by the inputs.

##### 2.4.4.3. DLT File Output

The DLT file output writer implements both methods for writing data, one only
containing the DLT line, the other containing the DLT line with the binary
packet data.

If the binary packet data is not available, it must construct the packet and
write the data. It can be limited so that it only writes strings and signed
integers to the output. It may limit itself to first converting the payload to a
single string and writing this. This is typically used to write skipped data
that was not part of the original input.

If the binary packet data is available, it shall write this. The input data may,
or may not contain a storage header. If the storage header is missing, one shall
be constructed and written.

This output writer also maintains a binary stream, the file name based on a
template, the size of the output stream on when to split.

##### 2.4.4.4. File Templates for DLT and Text Output

The functionality for writing DLT and text files have components in common and
should be implemented in the `OutputBase`:

* Mapping the input template to an output file name
  * `%FILE%`. Knowing the name of the input file. This is only known by the
    application layer. The output writer must be told of the input file name
    before a new file is written.
  * `%CDATE%`, `%CTIME%`, `%CDATETIME%`. Take the timestamp from the
    `DltTraceLineBase` message if this is the first line to write. This implies
    delayed opening of the file if it contains one of these elements in the
    output template.
  * `%CTR%`. The counter for the number of files written can be handled by the
    writer itself.
* The size when splitting the output must be communicated from the `FilterApp`
  application.
* If it should append to the file if it exists, or create a new file (i.e the
  first input would require a new file, subsequent would append if the output
  file name does not change)
* If it should overwrite the file or raise an error (dependent on the `--force`
  flag).

On initialization, the `OutputStreamFactory` instantiates a `IOutputStream`. For
the `DltOutput` and `TextOutput`, it gives also the `Split` and `Force`
properties which are used to initialize the base object `OutputBase`. The
`IOutputStream` object is alive for all inputs. This allows the ability to
effectively concatenate files if the output template file name suggests so.

When the `FilterApp` opens a file for reading, it tells the `IOutputStream` of a
potentially new file to write through `SetInput`. The `InputFormat` is used so
the `IOutputStream` knows how to write the header and the packet if needed.

The `OutputBase` is responsible for parsing the file name as a template. It gets
the `%FILE%` from `SetInput`, the `%CDATE%`, `%CTIME%` and `%CDATETIME%` from
the first `Write` just before opening the file. The `%CTR%` is maintained
internally.

##### 2.4.4.5. Input File Concatenation

Input files shall be concatenated if the output file name is independent of the
input file name. All inputs will be processed one after the other, and split as
necessary.

That means, if the template does not contain anything that is dependent on the
input file name, then concatenation is allowed. Otherwise, it is expected for
every input, there is a separate output file.

Rule: `AllowConcatenation = !%FILE%`

##### 2.4.4.6. Splitting Output Files

If the `Split` option is provided so that files should be split, then files will
be split and dependent on the variables `%CTR%` and `%CDATETIME%`, `%CDATE%`,
`%CTIME%`. If after a split the file name has not changed (say for example, it's
based on the time stamp which hasn't changed), the current file will still be
written to. Using the `%CTR%` ensures that the file will be split.

If the template doesn't contain any variable that is expected to change as a
file is parsed, then splitting is unnecessary. That is, it must have one of the
above mentioned variables.

Rule: `AllowSplit = %CTR% || %CDATE% || %CTIME% || %CDATETIME%`

##### 2.4.4.7. Detailed Logic for Splitting and Concatenating Files

To handle the logic of input files being converted to output files, it is split
into two logical paths, depending on `AllowConcatenation`. For the current
output, instead of considering it a single file, we consider a list of Segments,
which are a list of files written to the output, which when considered together,
is the output stream. If `AllowConcatenation` is `true`, there is only one
output stream (so one list of Segments), otherwise, a list of Segments per
input.

The following table summarizes the above sections

| %FILE% | %CTR% | %CDATETIME% | AllowSplit | AllowConcat | Example                                                                      |
| ------ | ----- | ----------- | ---------- | ----------- | ---------------------------------------------------------------------------- |
| N      | N     | N           | N          | Y           | f1.dlt, f2.dlt => out.txt                                                    |
| N      | Y     | N           | Y          | Y           | f1.dlt, f2.dlt => out_001.txt, out_002.txt, out_003.txt                      |
| N      | Y     | Y           | Y          | Y           | f1.dlt, f2.dlt => out_001_{time}.txt, out_002_{time}.txt, out_003_{time}.txt |
| N      | N     | Y           | Y          | Y           | f1.dlt, f2.dlt => out_{time}.txt, out_{time}.txt, out_{time}.txt             |
| Y      | N     | N           | N          | N           | f1.dlt, f2.dlt => f1.txt, f2.txt                                             |
| Y      | Y     | N           | Y          | N           | f1.dlt, f2.dlt => f1_001.txt, f1_002.txt, f2_001.txt                         |
| Y      | Y     | Y           | Y          | N           | f1.dlt, f2.dlt => f1_001_{time}.txt, f1_002_{time}.txt, f2_001_{time}.txt    |
| Y      | N     | Y           | Y          | N           | f1.dlt, f2.dlt => f1_{time}.txt, f1_{time}.txt, f2_{time}.txt                |

The following logic applies to satisfy the rules above:

* `OutputBase.SetInput(input: string)`:
  * If `!AllowConcatenation`:
    * Closes the current file.
    * Sets the Segments list to `null`.
  * If `AllowConcatenation`, then the file isn't closed as we join the inputs
    together.
  * Updates the variable `%FILE%` based on the new input file name.
  * The file format is used by only `DltOutput`.
* `OutputBase.Write()`:
  * If `!OutputWriter.IsOpen()`:
    * This is a new file, or was closed because the last split was too large.
    * Set the time stamp variables `%CDATETIME%`, `%CDATE%`, `%CTIME%`.
    * Open a new file (or append an existing) with OutputWriter (see below).
  * Write the line (in text or binary form).
  * If `SplitAllowed`, and the file length > split size:
    * Close the file
    * Increment `%CTR%`

The logic in getting the output file name and mode and about to open a new file,
which is handled by `OutputBase` to be common for any `IOutputStream` that
writes files.

* If the Segments list is `null`, this is the first line we're writing (for this
  input):
  * `%CTR%` is set to `001`.
  * Generate the file name from the template.
    * If we've created this file before, even for a different input, then we
      raise an error.
    * If the file is in the list of inputs, then generate an error.
  * Create a new Segments list, add this file name.
  * `FileMode = Force ? CreateNew : Create`
* If the Segments list is not `null`, then we closed the file due to split size
  (but not due to a new input file):
  * Generate the file name from the template. The `%CTR%` has already been
    incremented.
    * If we've created this file before, and it's not the same as the last file
      in the Segments list, then we raise an error (which would otherwise result
      in data converted being destroyed);
      * `FileMode = Append`
    * Otherwise, add to the Segments list.
      * `FileMode = Force ? CreateNew : Create`

##### 2.4.4.8. IOutputStream Object Lifetime

As indicated above, it is important that this object is created once and used
for all input files as in the previous section on concatenating output files.
The `IOutputStream` is then used directly by the `FilterApp` or used by the
decoder. But the `FilterApp` always calls when a new file is processed, between
instantiations of the decoder.

##### 2.4.4.9. On Flush

When the input stream has reached end of file, or the decoder is closed, the
`DltDecoder` has the method `Flush()` called. This should result in the
remaining data in the packet being written to the `IOutputStream`. The
`IOutputStream` must not be `Dispose()`d of, as this would close the underlying
stream, thus causing problems as in the previous section about the object
lifetime.

#### 2.4.5. The Filter and the Context

The filter is used for matching conditions of a trace line, as given by user
input. The context maintains information for when a filter matches, so that
previous lines that didn't match can be printed. They are two separate blocks,
very closely related and to be managed as a single unit.

There are three use cases:

* No filter. Dump all the output lines.
* Filter with no context. Apply the filter and decide stateless if the line
  matches or not.
* Filter with context. If there is a match, context may also need to be logged.

The flow for no context is simple:

![Filter No Context](out/diagrams/domainfilternocontext/Domain.Filter.NoContext.svg)

The flow with context is slightly complexer and can be used to identify the more
general use case:

![Filter with Context](out/diagrams/domainfiltercontext/Domain.Filter.Context.svg)

For each trace line that is decoded, it is given to the `Context` object if
required. The `Context` object maintains:

* A history of lines for the `--before-context` option
* A counter of how many lines should be given for the `--after-context` option

##### 2.4.5.1. Output Chaining

The filter and the context shall implement the `IOutputStream` interface. It
shall have a constructor allowing another `IOutputStream`, which the filter and
the context can write to.

The output from decoding a line is given immediately to an object of type
`IOutputStream`. The Filter and the Context can be implemented so that the check
and filter the input, and if there is a match, outputs to the next
`IOutputStream` object, which is described in the sections earlier.

These classes does not have to be thread safe. It runs in the context of the
application thread for the console output, or in the context of the decoder
thread. The construction of the class defines the array upfront to minimize
impact on the garbage collector.

![Output Filter](out/diagrams/outputstreamfilter/Domain.OutputStreamFilter.svg)

From this we see that any output stage can be filtered prior, allowing
separation of the output format from the filtering.

The property `SupportsBinary` and the method `SetInput` are a passthrough.

##### 2.4.5.2. Context Implementation

The `Context` class implements checking the filter and notifying of changes. It
must necessarily have copy operations for the duration of the history given by
the `before-context` option.

The `Context` class is expected to be used in the following manner:

```csharp
do {
    line = await decoder.GetLineAsync();
    if (line == null) continue;

    if (context.Check(line)) {
        foreach (DltTraceLineBase beforeLine in context.GetBeforeContext()) {
            WriteLine(beforeLine, m_Config.ShowPosition);
        }
        WriteLine(line, m_Config.ShowPosition);
    } else if (context.IsAfterContext()) {
        WriteLine(line, m_Config.ShowPosition);
    }
} while (line != null);
```

It works by performing a check for each line. If there is a match, given by the
result of `context.Check(line)` being true, the history buffer must be printed.
The method `context.GetBeforeContext()` returns an `IEnumerable` to iterate over
the history. While iterating, no calls to `context.Check()` are allowed, as the
enumerator and the context buffer share the same buffer to avoid copy
operations. If there is no history, the enumerator is empty.

Then the current line must be printed.

On further iteration and new lines, the history buffer is not updated, while
`context.IsAfterContext()` is true, which also decrements the internal counter
for how many lines should be printed.

#### 2.4.6. Decoder Extension on Line Decoding

The property `IOutputStream.SupportsBinary` can be used to determine if the
`IOutputStream` object should be given to a `DltTraceDecoder`, or be parsed by
the `FilterApp`.

##### 2.4.6.1. DltTraceDecoder writing to IOutputStream

While in the use case for console output, the line is given by the result of
`GetLineAsync()`, the ability to write to a file requires knowledge of the DLT
packet that was just decoded if it needs to be written to a file (so that the
binary contents of the packet are not modified when writing).

The `DltTraceDecoderFactory` can be given the `IOutputStream` and use this to
determine if it should instantiate an object from `RJCP.DLL.Log` library, or if
it should instantiate it's own library which derives from `RJCP.DLL.Log` that
uses the `IOutputStream` to write the packet.

The `RJCP.DLL.Log` doesn't write to the packet itself, as this is an extension
to decoding. It only offers the ability through inheritance to extend the
functionality which is to be done by the `DltDump` application.

![Packet Writer](out/diagrams/domaindecoder/Domain.Decoder.Writer.svg)

The `RJCP.DLL.Log` must be extended to support the cases when:

* A valid packet is found, then the decoded line and the binary packet data
  should be written. Because the `FilterApp` instantiates `IOutputStream` and
  also `DltTraceDecoder`, it knows the input format, and can instruct the
  `IOutputStream` to remove headers, or add a storage header without knowledge
  from the decoder itself (the decoder doesn't have to know how to write the
  output).
* Invalid data is found, and skipped data is to be written. In this case, only a
  `DltTraceLineBase` is provided, for which the `IOutputStream` must construct
  the binary data from only the line and write to the output file.

The `IOutputStream` already applies filtering and context management through
chaining described earlier.

### 2.5. Infrastructure

#### 2.5.1. Version Information

This class uses the .NET framework to obtain information about the version of
the assembly. This class is expected to be very small and generic. It will
obtain data from the Assembly Informational Version if available, and then from
the file version.
