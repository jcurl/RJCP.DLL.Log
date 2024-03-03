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
Results = net6

BenchmarkDotNet=v0.13.12 OS=Windows 10 (10.0.19045.4046/22H2/2022Update)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU(s), 8 logical and 4 physical core(s)
  [HOST] : .NET 6.0.27 (6.0.2724.6912), X64 RyuJIT
```

```text
Results = net8

BenchmarkDotNet=v0.13.12 OS=Windows 10 (10.0.19045.4046/22H2/2022Update)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU(s), 8 logical and 4 physical core(s)
  [HOST] : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT
```

| Project 'dlt' Type     | Method                        | mean (net6) | stderr | mean (net8) | stderr |
|:-----------------------|:------------------------------|------------:|-------:|------------:|-------:|
| DltDecoderArgBenchmark | DecodeBool                    | 11.90       | 0.03   | 12.27       | 0.02   |
| DltDecoderArgBenchmark | DecodeFloat32bitBE            | 13.41       | 0.03   | 11.38       | 0.02   |
| DltDecoderArgBenchmark | DecodeFloat32bitLE            | 13.36       | 0.02   | 11.59       | 0.01   |
| DltDecoderArgBenchmark | DecodeSignedInteger32bitBE    | 15.95       | 0.04   | 12.76       | 0.02   |
| DltDecoderArgBenchmark | DecodeSignedInteger32bitLE    | 16.04       | 0.01   | 13.44       | 0.03   |
| DltDecoderArgBenchmark | DecodeStringAsciiBE           | 44.83       | 0.02   | 40.35       | 0.06   |
| DltDecoderArgBenchmark | DecodeStringAsciiLE           | 44.62       | 0.06   | 41.00       | 0.05   |
| DltDecoderArgBenchmark | DecodeStringUtf8BE            | 54.46       | 0.08   | 47.00       | 0.11   |
| DltDecoderArgBenchmark | DecodeStringUtf8LE            | 54.23       | 0.08   | 46.56       | 0.05   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBE  | 15.47       | 0.03   | 12.95       | 0.02   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBin | 15.65       | 0.02   | 13.00       | 0.03   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitHex | 15.61       | 0.03   | 12.90       | 0.03   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitLE  | 15.37       | 0.03   | 12.99       | 0.02   |
| DltDecoderBenchmark    | DecodeIntFilePackets          | 3126.28     | 1.12   | 2413.32     | 2.17   |
| DltDecoderBenchmark    | DecodeStringFilePackets       | 3580.15     | 1.13   | 2777.60     | 1.23   |

Description of the test results:

- `DltDecoderBenchmark.*`: Decodes **ten** normal DLT packets with a storage
  header that are already in memory. Each message is a normal verbose DLT
  message with a single argument.
  - `DecodeStringFilePackets`: Decodes a single argument of string type which is
    UTF8 little endian.
  - `DecodeIntFilePackets`: Decodes a single argument of an integer 32-bit which
    is little endian.
  - For this particular test, arguments are not decoded, and so should show
    equivalent results.
- `DltDecoderArgBenchmark.*`: These decode a single argument. This can be used
  to estimate how much time it takes to decode a single trace line when the
  argument is present. To estimate how long it takes to decode a line with the
  argument, take the `DltDecoderBenchmark` and divide by 10, subtract the
  baseline for a string/integer, and add the time for the arguments expected to
  be present.

### Notes for Results

A change in the Operating System or its configuration, update of software, may
change results. To compare changes, configurations must be the same.
