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
$ git rj perf dltdump
```

If you want to run from this repository. This might be desirable if you want to
run different kind of benchmarks (e.g. with other runtimes).

```cmd
$ dotnet build -c Release
$ cd app/DltDump/perf/bin/Release/netcoreapp3.1
$ dotnet exec DltDumpBenchmark.dll --filter *
```

## Results

This is the current performance bench mark for the `ContextOutput` class.

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

| Project 'dltdump' Type | Method     | mean (net6) | stderr | mean (net8) | stderr |
|:-----------------------|:-----------|------------:|-------:|------------:|-------:|
| ContextBenchmark       | RunContext | 415.41      | 0.44   | 323.60      | 0.12   |

## Appendix: Comparison between Classes and Struct

The .NET BCL uses mutable `struct` types for their enumerators. We measure the
performance difference of our own enumerator for this purpose. The base test was
against the commit a075faa21 ("DltDumpBenchmark: Check the performance between
class and struct for Context") on 22/March/2022.

### Solution using Classes for the Enumerator

```text
Results = netcore31

BenchmarkDotNet=v0.13.1 OS=Windows 10.0.19043.1586 (21H1/May2021Update)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU(s), 8 logical and 4 physical core(s)
  [HOST] : .NET Core 3.1.23 (CoreCLR 4.700.22.11601, CoreFX 4.700.22.12208), X64 RyuJIT
```

| Project 'dltdump' Type | Method      | mean (netcore31) | stderr |
|:-----------------------|:------------|-----------------:|-------:|
| ContextBenchmark       | RunContext  | 402.37           | 1.44   |

### Solution using Structs for the Enumerator

```text
Results = netcore31

BenchmarkDotNet=v0.13.1 OS=Windows 10.0.19043.1586 (21H1/May2021Update)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU(s), 8 logical and 4 physical core(s)
  [HOST] : .NET Core 3.1.23 (CoreCLR 4.700.22.11601, CoreFX 4.700.22.12208), X64 RyuJIT
```

| Project 'dltdump' Type | Method      | mean (netcore31) | stderr |
|:-----------------------|:------------|-----------------:|-------:|
| ContextBenchmark       | RunContext  | 418.52           | 1.30   |

### Conclusion

We can certainly see that using a `struct` type is slower. We will continue to
use the `class` type.

## Differences in Tests

```diff
--- a/apps/DltDump/code/Domain/Context.cs
+++ b/apps/DltDump/code/Domain/Context.cs
@@ -146,11 +146,11 @@
 
                 public void Dispose() { /* nothing to do */ }
             }
         }
 
-        private sealed class BeforeContext : IEnumerable<DltTraceLineBase>
+        private struct BeforeContext : IEnumerable<DltTraceLineBase>
         {
             private readonly DltTraceLineBase[] m_Buffer;
             private readonly int m_BufferStart;
             private readonly int m_BufferLength;
 
@@ -169,11 +169,11 @@
             IEnumerator IEnumerable.GetEnumerator()
             {
                 return GetEnumerator();
             }
 
-            private sealed class BeforeContextEnumerator : IEnumerator<DltTraceLineBase>
+            private struct BeforeContextEnumerator : IEnumerator<DltTraceLineBase>
             {
                 private readonly DltTraceLineBase[] m_Buffer;
                 private readonly int m_BufferStart;
                 private readonly int m_BufferLength;
                 private int m_Index;
@@ -181,10 +181,12 @@
                 public BeforeContextEnumerator(DltTraceLineBase[] buffer, int start, int length)
                 {
                     m_Buffer = buffer;
                     m_BufferStart = start;
                     m_BufferLength = length;
+                    m_Index = 0;
+                    Current = null;
                 }
 
                 public DltTraceLineBase Current { get; private set; }
 
                 object IEnumerator.Current { get { return Current; } }

```
