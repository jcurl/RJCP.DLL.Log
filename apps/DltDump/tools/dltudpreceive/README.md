# DLT UDP Receive

This is a small test tool to show how to receive multicast UDP packets with .NET.

## Building

Ensure you have the .NET Core SDK installed. This console test program has no
dependencies.

```sh
dotnet build
```

## Running the Receiver

Execute the command

* Bind to a local IP address

  ```sh
  ./bin/Debug/netcoreapp3.1/dltudpreceive <localipaddress>
  ```

* Bind to all IP addresses

  ```sh
  ./bin/Debug/netcoreapp3.1/dltudpreceive
  ```

The tool will capture the first 100 packets it receives on the multicast address
`239.255.42.99`. It will capture from any device that transmits. It will print
the address of the transmitter, and a hex dump of the bytes received.

## Running the Transmitter

The transmitter can be run on any machine on the local network that will
propagate the multicast group.

See [BUILD](../dltudpbeacon/BUILD.md) for instructions on building the
transmitter (which is written in C++17 with CMake).
