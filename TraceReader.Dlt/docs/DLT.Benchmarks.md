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
Results = netcore

BenchmarkDotNet=v0.13.12 OS=Windows 10 (10.0.19045.3930/22H2/2022Update)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU(s), 8 logical and 4 physical core(s)
  [HOST] : .NET 6.0.26 (6.0.2623.60508), X64 RyuJIT
```

| Project 'dlt' Type     | Method                        | mean (netcore) | stderr |
|:-----------------------|:------------------------------|---------------:|-------:|
| DltDecoderArgBenchmark | DecodeSignedInteger32bitLE    | 13.85          | 0.01   |
| DltDecoderArgBenchmark | DecodeSignedInteger32bitBE    | 13.42          | 0.03   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitLE  | 13.79          | 0.03   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBE  | 13.87          | 0.01   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitHex | 13.76          | 0.02   |
| DltDecoderArgBenchmark | DecodeUnsignedInteger32bitBin | 13.36          | 0.01   |
| DltDecoderArgBenchmark | DecodeFloat32bitLE            | 12.42          | 0.01   |
| DltDecoderArgBenchmark | DecodeFloat32bitBE            | 12.10          | 0.02   |
| DltDecoderArgBenchmark | DecodeBool                    | 10.24          | 0.01   |
| DltDecoderArgBenchmark | DecodeStringUtf8LE            | 47.14          | 0.03   |
| DltDecoderArgBenchmark | DecodeStringUtf8BE            | 49.26          | 0.13   |
| DltDecoderArgBenchmark | DecodeStringAsciiLE           | 37.42          | 0.03   |
| DltDecoderArgBenchmark | DecodeStringAsciiBE           | 38.05          | 0.05   |
| DltDecoderBenchmark    | DecodeStringFilePackets       | 3312.25        | 1.83   |
| DltDecoderBenchmark    | DecodeIntFilePackets          | 2920.22        | 1.96   |

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
