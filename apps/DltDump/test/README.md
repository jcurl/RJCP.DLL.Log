# Test Cases for DltDump <!-- omit in toc -->

Normally to execute all test cases on the console, run

```cmd
dotnet test dltdumptest.dll
```

If you're not in the directory where the DLL is, provide the full path. It will
*not* test the integration tests which are marked as explicit.$$

- [1. Integration Tests](#1-integration-tests)
- [2. Code Coverage](#2-code-coverage)
- [3. Manual Tests](#3-manual-tests)
  - [3.1. Simulating a DLT server on Unix (or Windows with Cygwin)](#31-simulating-a-dlt-server-on-unix-or-windows-with-cygwin)
  - [3.2. Simulating a Serial Server on Windows](#32-simulating-a-serial-server-on-windows)
  - [3.3. Sending data over a NULL modem cable on Linux](#33-sending-data-over-a-null-modem-cable-on-linux)

## 1. Integration Tests

Integration tests are those that might cause problems when running on a CI,
because they have timing dependencies, or depend on other OS resources. In
particular, the following tests are marked as explicit:

- Network tests using TCP
  - TcpClientStream
  - TcpServer (test code)

To run the integration tests on the console

```cmd
dotnet test dltdumptest.dll --filter "FullyQualifiedName~RJCP.App.DltDump"
```

## 2. Code Coverage

To capture code coverage

```cmd
dotnet tool install --global dotnet-coverage
```

Then to get the code coverage for everything, including the explicit tests

```cmd
dotnet coverage collect "dotnet test dltdumptest.dll --filter FullyQualifiedName~RJCP.App.DltDump"
```

## 3. Manual Tests

### 3.1. Simulating a DLT server on Unix (or Windows with Cygwin)

If you have a file with a DLT stream that contains only the standard header, you
can set up a simple server using the `netcat` tool:

```sh
$ ip a
192.168.1.1
$ nc -lp 3490 < myfile.raw.dlt
```

Then on the remote machine, run the dumper:

```sh
$ dltdump tcp://192.168.1.1
```

### 3.2. Simulating a Serial Server on Windows

If you have a file with a DLT stream that contains a `DLS\1` header, you can
send data to a NULL modem cable (or using com0com virtual serial port
simulation). Say that `COM8` will receive and `COM9` is connected via the NULL
modem cable to `COM1`.

```sh
> dltdump ser:COM8,115200,8,n,1
```

Use a program designed for sending data over the serial port, such as
[TeraTerm](https://ttssh2.osdn.jp/index.html.en). Trying to send data over the
serial port using the console will likely fail as binary data may be modified by
the serial port drivers. A dedicated program for sinding files configures the
serial port in such a way that data is correctly sent.

Of course, if the input format does not start with the `DLS\1` header, use the
`format` command line option to define the input format (e.g. `/format:net`)

### 3.3. Sending data over a NULL modem cable on Linux

If you have a NULL modem cable, you can send data (if you have raw data):

```sh
$ stty -F /dev/ttyUSB1 115200 cs8 raw -cstopb -parenb -echo -echoe -echok
```

Then receive the data via another console

```sh
$ dltdump ser:/dev/ttyUSB0,115200,8,n,1
```
