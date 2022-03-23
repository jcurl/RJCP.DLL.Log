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
      - [2.3.2.4. Context and Filter](#2324-context-and-filter)
      - [2.3.2.5. Console Output](#2325-console-output)
      - [2.3.2.6. File Output](#2326-file-output)
  - [2.4. Domain](#24-domain)
    - [2.4.1. InputStreamFactory and InputStream](#241-inputstreamfactory-and-inputstream)
    - [2.4.2. Decoders (and PCAP, PCAPNG formats)](#242-decoders-and-pcap-pcapng-formats)
      - [2.4.2.1. DLT Trace Decoder (AutoSAR PRS format)](#2421-dlt-trace-decoder-autosar-prs-format)
      - [2.4.2.2. DLT Trace Decoder for PCAP and PCAPNG](#2422-dlt-trace-decoder-for-pcap-and-pcapng)
    - [2.4.3. The Filter and the Context](#243-the-filter-and-the-context)
      - [2.4.3.1 Implementation](#2431-implementation)
    - [2.4.4. Decoder Extension on Line Decoding](#244-decoder-extension-on-line-decoding)
    - [2.4.5. Binary Writer](#245-binary-writer)
      - [2.4.5.1. On Flush](#2451-on-flush)
      - [2.4.5.2. Online Mode](#2452-online-mode)
      - [2.4.5.3. The Time Stamp](#2453-the-time-stamp)
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

The layered design is based on the principles of Domain Driven Design by Eric
Evans. It has been extended slightly. A higher layer can access an element in
the lower layer, but not the reverse. There is no strict rule that only the
immediate layer is accessible, so that the application layer might handle a
reference to the infrastructure layer, or the application layer uses elements
from the framework layer.

#### 2.1.1. Root

This is where the program entry points are. It instantiates the View.

#### 2.1.2. View

This is the user interface. This is a command line application, so it gets the
options the user provided and executes the use cases in the application layer.
If a graphical user interface should be created, only this layer is modified.

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

This contains code local to this application, but could be considered reasonably
generic, but for now only used in this application. Importantly, it doesn't
contain any thing related to policy. It helps implement business logic for the
application, but doesn't assume what those decisions are, applying only
technical constraints.

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
describes how the application `Filter` should configure itself. Most of the
options from the command line will be copied to the `FilterConfig` object.

![Filter Initialization](out/diagrams/appfilterinit/App.Filter.Initialization.svg)

The `Filter` application initializes all objects it needs from the `Domain`
layer.

It will loop over all the inputs given in the `FilterConfig`. The
`InputStreamFactory` knows how to take the input file which is a URI string, and
generates an appropriate stream. This stream could be a file, or a TCP stream.
The output `IInputStream` provides additional methods which can asynchronously
connect the stream if it is required (files don't need to be connected, where
network streams do).

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

Before instantiating the decoder, the output must also be known, so that the
`BinaryWriter` can be used directly from the decoder itself. This is covered
later in section for File Output.

##### 2.3.2.4. Context and Filter

The context and filter are used for matching. They are two separate blocks, very
closely related and to be managed as a single unit.

The `Filter` application receives the trace line when writing to the console,
therefore it maintains the `Context` for this use case.

For writing to a file, this is maintained in the context of the decoder. The
decoder needs some glue logic around the `Context` block, which can be part of
the overridden decoder. The glue logic in this case must also record the input
buffer byte stream, optionally create a storage header, and if there is a filter
match, then write this stream to a file.

##### 2.3.2.5. Console Output

The console writer is maintained by the `Filter` application, which is given the
output from the `Context` block. The context block is an array of lines so it
can maintain context.

##### 2.3.2.6. File Output

The binary writer block needs information from the `Filter` layer to be able to
substitute and write the output file:

* Knowing the name of the input file. This is only known by the application
  layer. The binary writer must be told of the input file name after a file is
  closed and before a new file is written.
* The size when splitting the output must be communicated from the `Filter`
  application.
* If it should append to the file if it exists, or create a new file (i.e the
  first input would require a new file, subsequent would append if the output
  file name does not change)
* If it should overwrite the file or raise an error (dependent on the `--force`
  flag).

The other elements:

* The first time stamp after a new file is created is provided by the Decoder.
* The counter for the number of files written can be handled by the Binary
  Writer itself.

Because the application runs in a loop, the lifetime of the input stream is for
the duration of one iteration of the loop, which means the lifetime of the
decoder is also for the duration of one iteration of the loop. When the input
stream is closed, the decoder flushes its output, which can write the output to
the binary writer.

That means the information such as the force flag, input file name, split size,
can be given to the decoder factory. There might be other information in the
future that the application could apply, which can be used in the template,
which is static for the duration of the loop. Thus, there should be a template
block given to the factory. The template block is a dictionary mapping an input
string to another string (such as the file name).

### 2.4. Domain

The domain layer contains logic specific to this application or the problem
domain. It implements the business logic specific to reading and writing DLT
data.

#### 2.4.1. InputStreamFactory and InputStream

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

Connecting the stream should be done by the `Filter` application in an
asynchronous manner before giving to the decoder.

#### 2.4.2. Decoders (and PCAP, PCAPNG formats)

There are three decoders implemented for reading a stream of bytes and
interpreting DLT contents, implemented as per the [AutoSAR R20-11
PRS](https://www.autosar.org/fileadmin/user_upload/standards/foundation/20-11/AUTOSAR_PRS_LogAndTraceProtocol.pdf)
standard (DLT version 1).

Additional decoders can be implemented dependent on the `IInputStream` assumed
format, that derives from the `DltTraceDecoderBase`. For example, reading a
WireShark file, that contains DLT packets transmitted as UDP.

##### 2.4.2.1. DLT Trace Decoder (AutoSAR PRS format)

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

##### 2.4.2.2. DLT Trace Decoder for PCAP and PCAPNG

A new DLT Trace Decoder must be written that derives from the existing decoder.
In particular, when the PCAP decoder receives data to decode, it must first
unwrap the PCAP packet data. It is not responsible for decoding the DLT data,
and calls the base decoder to generate the packet data.

The precise specifications may change, but the first version should decode:

* PCAP and PCAPNG formats
* An integral number of packets stored in a UDP frame with destination port
  3490, where there is no fragmentation
* Optionally support 802.1q VLAN

Notably what will not be supported in this first version will be

* Fragmented UDP packets
* 802.1q double tagging
* Additional filtering based on PCAP

#### 2.4.3. The Filter and the Context

There are two use cases

* No context. Apply the filter and decide stateless if the line matches or not.
* With context. If there is a match, context may also need to be logged.

The flow for no context is simple:

![Filter No Context](out/diagrams/domainfilternocontext/Domain.Filter.NoContext.svg)

The flow with context is slightly complexer and can be used to identify the more
general use case:

![Filter with Context](out/diagrams/domainfiltercontext/Domain.Filter.Context.svg)

For each trace line that is decoded, it is given to the `Context` object if
required. The `Context` object maintains:

* A history of lines for the `--before-context` option
* A counter of how many lines should be given for the `--after-context` option

The line is also given to the `Filter` object which tests the object if there is
a match or not. If there is a match, the `Context` object is told, so that it
can pass its output.

The actual class implementation would be slightly different to what is depicted
here. A single class would maintain the context and the filter, with a single
function call that takes the trace line as an input. The output is an array of
objects which contain the trace lines to output, and the buffers for the output.

This class does not have to be thread safe. It runs in the context of the
application thread for the console output, or in the context of the decoder
thread. The construction of the class defines the array upfront to minimize
impact on the garbage collector.

If the line has no time stamp, the context must add the time stamp at the time
the message is recorded.

##### 2.4.3.1 Implementation

The `Context` class implements checking the filter and notifying of changes. It
must necessarily have copy operations for the duration of the history given by
the `before-context` option.

The correct implementation is given in `FilterApp.LoopContext`:

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

#### 2.4.4. Decoder Extension on Line Decoding

The DLT decoder must enable the ability (through inheritance or other means)
that application functionality can be called after decoding each line. While in
the use case for console output, the line is given by the result of
`GetLineAsync()`, the ability to write to a file requires knowledge of the DLT
packet that was just decoded if it needs to be written to a file (so that the
binary contents of the packet are not modified when writing).

Functionality already exists in the decoder to process skipped data and trace
lines (by at least adding them to an internal list, that can be enumerated by
the generic trace decoder for all file formats). This can be extended to be
virtual so that classes can override this and extend functionality. The information
needed to write a file is:

* The trace line. Required to apply a filter
* The `IDltLineBuilder`. Required to know if there is a storage header or not.
  If there is, the storage header can be written as is, else, one must be
  generated by the derived class (the creation of the storage header will then
  be part of the binary writer)
* The `ReadOnlySpan<byte>` for the packet, that if it is written, an exact copy
  of the input data is written, ensuring potential compatibility with other
  programs.

![Binary Writer](out/diagrams/domaindecoder/Domain.Decoder.Writer.svg)

#### 2.4.5. Binary Writer

The binary writer is a combination of two components

* Mapping a template string, which is used to name the final output file; and
* Writing to the output stream.

It is responsible for knowing when the current file is too large and must be
split, the name of the new file and adding a storage header if required. It
obtains the logic for this information from the `BinaryWriterConfig`, which is
passed to the `BinaryWriter`.

##### 2.4.5.1. On Flush

When the input stream has reached end of file, or the decoder is closed, the
`DltDecoder` has the method `Flush()` called, which is a trigger to the
`BinaryWriter` to close the output file. It shall:

* Close the output stream
* Rename the output stream if the name was not known at the time it was opened

##### 2.4.5.2. Online Mode

The original packet given to the `BinaryWriter` from the `DltDecoder` does not
contain a storage header. The `DltTraceLineBase` contains the device time stamp
which is calculated by the decoder by the `IDltLineBuilder` which is in online
mode, so the `BinaryWriter` does not need to calculate the time stamp. It must
however take the time stamp and create the 16-byte storage header in addition to
the rest of the packet. If the original packet contains a serial header, this
should be stripped. The offsets are already part of the derived classes to where
the storage header starts.

##### 2.4.5.3. The Time Stamp

If the template to write the file contains the string `%CDATE%`, `%CTIME%` or
`%CDATETIME%`, then the name of the file is not known at the time it is opened.
There are two options:

* Delay the creation of the file until the first line appears.
* Create the file immediately and rename the file on close when it is set.

That this variable must be substituted with the time stamp of the first line
that is written to the file (and not the time stamp of the first line read), it
is possible that the creation of the file is delayed until the first line
arrives. This implies that file has the correct name immediately.

### 2.5. Infrastructure

#### 2.5.1. Version Information

This class uses the .NET framework to obtain information about the version of
the assembly. This class is expected to be very small and generic. It will
obtain data from the Assembly Informational Version if available, and then from
the file version.
