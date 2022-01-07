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
| DltDecoderArgBenchmark | DecodeSignedInteger32bitLE    | 10.53            | 0.03   |
| DltDecoderArgBenchmark | DecodeSignedInteger32bitBE    | 10.93            | 0.02   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitLE  | 10.78            | 0.03   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBE  | 11.20            | 0.04   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitHex | 11.31            | 0.02   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBin | 11.87            | 0.02   |
| DltDecoderArgBenchmark | DecodeFloat32bitLE            | 9.06             | 0.03   |
| DltDecoderArgBenchmark | DecodeFloat32bitBE            | 9.47             | 0.02   |
| DltDecoderArgBenchmark | DecodeBool                    | 7.03             | 0.05   |
| DltDecoderArgBenchmark | DecodeStringUtf8LE            | 54.68            | 0.14   |
| DltDecoderArgBenchmark | DecodeStringUtf8BE            | 54.33            | 0.14   |
| DltDecoderArgBenchmark | DecodeStringAsciiLE           | 43.87            | 0.09   |
| DltDecoderArgBenchmark | DecodeStringAsciiBE           | 42.90            | 0.07   |
| DltDecoderBenchmark    | DecodeStringFilePackets       | 3569.30          | 4.02   |
| DltDecoderBenchmark    | DecodeIntFilePackets          | 3136.45          | 9.48   |

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

A change in the Operating System or its configuration, update of software, may
change results. To compare changes, configurations must be the same.
