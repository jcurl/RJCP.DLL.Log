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

| Project 'dlt' Type  | Method            | mean (netcore31) | stderr |
|:--------------------|:------------------|-----------------:|-------:|
| DltDecoderBenchmark | DecodeFilePackets | 2384.60          | 2.99   |

Description of the test results:

* `DltDecoderBenchmark.DecodeFilePackets`: Decodes **ten** normal DLT packets
  with a storage header that are already in memory. Each message is a normal
  verbose DLT message with a single string argument.

### Notes for Results

The current measurements are for indication only. There is no decoding of
verbose messages and their arguments or control messages in this current
implementation.
