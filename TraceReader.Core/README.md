# RJCP.Diagnostics.Log Project <!-- omit in toc -->

- [1. Trace Reader](#1-trace-reader)
- [2. Readers and FActories for the Readers](#2-readers-and-factories-for-the-readers)
  - [2.1. Text Trace Reader](#21-text-trace-reader)
  - [2.2. Text Trace Reader for TeraTerm Logs](#22-text-trace-reader-for-teraterm-logs)
  - [2.3. Further Trace Readers](#23-further-trace-readers)
- [3. Constraints](#3-constraints)
- [4. Release History](#4-release-history)
  - [4.1. Version 0.8.1](#41-version-081)
  - [4.2. Version 0.8.0](#42-version-080)

## 1. Trace Reader

The TraceReader project is a basis package that provides the foundation for
building trace reading projects, such as `TraceReader.Dlt`. It supports common
operations for reading line based, or packet binary based log files, that can be
easily enumerated.

The `TraceReader<T>` (which implements `ITraceReader`) takes a decoder,
`ITraceDecoder<T>`, which receives an arbitrary number of bytes and decodes to a
line. The TraceDecoder consumes all the data (caching data if necessary),
returning an enumeration of decoded lines.

Factory classes, `ITraceReaderFactory`, can create the trace readers, and is the
primary method for instantiating trace readers.

More information about the design is found in the hosting repository.

## 2. Readers and FActories for the Readers

### 2.1. Text Trace Reader

A simple trace reader is implemented, that can read a text file and return lines
in the form of `TraceLine`.

```csharp
ITraceReaderFactory factory = new TextTraceReaderFactor() {
    Encoding = Encoding.GetEncoding("UTF-8", new EncoderReplacementFallback("."), new DecoderReplacementFallback("."))
};
ITraceReader reader = factory.CreateAsync("logfile.txt");

foreach (var line in reader) {
    Console.WriteLine($"{line}");
}
```

While there is a
[StreamReader.ReadLine](https://learn.microsoft.com/en-us/dotnet/api/system.io.streamreader.readline)
method that looks similar and simpler, the power of this library comes with
reading binary, packet based data, or automatically interpreting metadata within
a line. The result of the line is not a string, but a `TraceLine` or a
derivative, which can provide much more metadata.

### 2.2. Text Trace Reader for TeraTerm Logs

[TeraTerm](https://github.com/TeraTermProject/teraterm/releases) is a popular
project, capturing data from serial or remote terminals. When logging, it can
prepend a line with a time stamp. This class shows the extensibility of the
TraceReader package, by interpreting the timestamps as they arrive, requiring
very little changes to the basic implementation for enumerating files.

```csharp
ITraceReaderFactory factory = new TeraTermTraceReaderFactory() {
    Encoding = Encoding.GetEncoding("UTF-8", new EncoderReplacementFallback("."), new DecoderReplacementFallback("."))
};
ITraceReader reader = factory.CreateAsync("logfile.txt");

foreach (var line in reader) {
    Console.WriteLine($"{line}");
}
```

Note, the only change in the code is the instantiation of the factory. The line
returned is now of type `LogTraceLine`, which provides the timestamp at the
beginning of the line, and automatically strips the timestamp from the beginning
of the line.

### 2.3. Further Trace Readers

Trace readers do not need to be text files. Please refer to
`RJCP.Diagnostics.Log.Dlt` which implements Automotive AutoSAR Diagnostic Log
and Trace protocol for reading binary based data. In the same repository as a
tool that hosts internally packetising PCAP files using the TraceReader
framework, which captures timestamps and other metadata.

## 3. Constraints

Often it is necessary to filter data when processing lines. There is a namespace
in this library called a `Cosntraint` which can return a `true` or `false` if a
log line meets a constraint or not.

Constraints are built and then precompiled, so that the comparison of
constraints is the same in performance is if you explicitly used `if` statements
to check the fields of the `TraceLine` received.

New constraints can be defined for new implementations of a trace reader.

A constraint can be as simple as:

```csharp
Constraint c = Constraint().TextEquals(match);
if (c.Check(line)) {
    Console.WriteLine($"{line}");
}
```

Constraints can be chained, so that all conditions must match:

```csharp
TraceLine line = new("Apple", 0, 0);
Constraint c = Constraint().TextString("App").TextString("ple");

Assert.That(c.Check(line), Is.True);
```

Constraints can have alternatives:

```csharp
TraceLine line = new("Text", 0, 0);
Constraint c = Constraint().TextEquals("Text").Or.TextEquals("Bar");

Assert.That(c.Check(line), Is.True);
```

And they follow Boolean precedence rules:

```csharp
TraceLine line = new("FooBar", 0, 0);
Constraint c = Constraint().TextString("Te").TextString("xt").Or.TextString("Foo").TextString("Bar");

Assert.That(c.Check(line), Is.True);
```

To group checks of constraints, so the following example is `('Foo' || 'Bar') &&
('Hill' || 'Billies')`:

```csharp
Constraint c1 = Constraint()
  .Expr(Constraint().TextString("Foo").Or.TextString("Bar"))
  .Expr(Constraint().TextString("Hill").Or.TextString("Billies"));
```

The compilation of the constraints occur on their first usage, or on `End()` at
the end of the chain.

More detailed information can be found in the repository
[Constraints.md](../docs/Constraints.md).

## 4. Release History

### 4.1. Version 0.8.1

Features:

- Implement a TraceWriter (DOTNET-763)

Bugfixes:

- The TraceReader now treats a `null` for an enumeration to indicate that
  decoding can no longer continue (DOTNET-642)
- Separate reader factories into decoder factories (DOTNET-652)
- Now dispose file streams (DOTNET-768)

Quality:

- Clean up code (DOTNET-749)
- Modernise to use `Result<T>` (DOTNET-794)
- `TextString` and `TextIString` performance improvements (DOTNET-833)
- Update to .NET 6.0 (DOTNET-936. DOTNET-942, DOTNET-943, DOTNET-945,
  DOTNET-951)

### 4.2. Version 0.8.0

- Initial Release
