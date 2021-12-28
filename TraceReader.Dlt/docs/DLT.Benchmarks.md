# Benchmark Measurements

The benchmark measurements are done using
[BenchmarkDotNet](https://benchmarkdotnet.org/), a micro-benchmark tool. The
goal of this benchmark is to measure the time required to decode a small set of
messages that are already in memory. It doesn't measure all use cases or how
long it takes to decode files.

## Micro-benchmark Measurements

The parent repository contains the configuration file to make it easier to run
these benchmarks, but are in no way required.

From the parent repository, after building:

```cmd
$ git rj build -c preview --build
$ git rj perf dlt
```

If you want to run from this repository. This might be desirable if you want to
run different kind of benchmarks (e.g. with other runtimes).

```cmd
$ dotnet build -c Release
$ cd TraceReader.Dlt/DltTraceReaderBenchmark/bin/Release/netcoreapp3.1
$ dotnet exec RJCP.Diagnostics.Log.DltBenchmark.dll --filter *
```

## Results

```text
Results = netcore31

BenchmarkDotNet=v0.13.1 OS=Windows 10.0.19043.1415 (21H1/May2021Update)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU(s), 8 logical and 4 physical core(s)
  [HOST] : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
```

| Project 'dlt' Type     | Method                        | mean (netcore31) | stderr |
|:-----------------------|:------------------------------|-----------------:|-------:|
| DltDecoderArgBenchmark | DecodeSignedInteger32bitLE    | 21.32            | 0.13   |
| DltDecoderArgBenchmark | DecodeSignedInteger32bitBE    | 21.25            | 0.03   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitLE  | 21.15            | 0.08   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBE  | 21.00            | 0.08   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitHex | 21.85            | 0.07   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBin | 134.28           | 0.73   |
| DltDecoderArgBenchmark | DecodeFloat32bitLE            | 17.31            | 0.05   |
| DltDecoderArgBenchmark | DecodeFloat32bitBE            | 16.69            | 0.06   |
| DltDecoderArgBenchmark | DecodeBool                    | 8.69             | 0.04   |
| DltDecoderArgBenchmark | DecodeStringUtf8LE            | 55.33            | 0.19   |
| DltDecoderArgBenchmark | DecodeStringUtf8BE            | 55.88            | 0.32   |
| DltDecoderArgBenchmark | DecodeStringAsciiLE           | 66.15            | 0.23   |
| DltDecoderArgBenchmark | DecodeStringAsciiBE           | 69.57            | 0.22   |
| DltDecoderBenchmark    | DecodeStringFilePackets       | 2303.71          | 7.08   |
| DltDecoderBenchmark    | DecodeIntFilePackets          | 2318.61          | 1.15   |
Description of the test results:

* `DltDecoderBenchmark.*`: Decodes **ten** normal DLT packets with a storage
  header that are already in memory. Each message is a normal verbose DLT
  message with a single argument.
  * `DecodeStringFilePackets`: Decodes a single argument of string type which is
    UTF8 little endian.
  * `DecodeIntFilePackets`: Decodes a single argument of an integer 32-bit which
    is little endian.
  * For this particular test, arguments are not decoded, and so should show
    equivalent results.
* `DltDecoderArgBenchmark.*`: These decode a single argument. This can be used
  to estimate how much time it takes to decode a single trace line when the
  argument is present. To estimate how long it takes to decode a line with the
  argument, take the `DltDecoderBenchmark` and divide by 10, subtract the
  baseline for a string/integer, and add the time for the arguments expected to
  be present.

### Notes for Results

The current measurements are for indication only. There is no decoding of
verbose messages and their arguments or control messages in this current
implementation.

A change in the Operating System or its configuration, update of software, may
change results. To compare changes, configurations must be the same.
