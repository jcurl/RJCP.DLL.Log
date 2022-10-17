# Building DLT UDP Beacon <!-- omit in toc -->

The project requires C++17 and CMake 3.10 or later. It is tested using the GNU Toolchain.

- [1. Tested Environments](#1-tested-environments)
- [2. Building](#2-building)
  - [2.1. Building on the Command Line](#21-building-on-the-command-line)
  - [2.2. Providing Compiler Flags](#22-providing-compiler-flags)
  - [2.3. Providing a DEBUG build](#23-providing-a-debug-build)
  - [2.4. Selecting the Compiler](#24-selecting-the-compiler)
  - [2.5. Enabling Sanitizers](#25-enabling-sanitizers)
  - [2.6. Building the Software](#26-building-the-software)

## 1. Tested Environments

The build has been tested on Ubuntu 22.04 with GCC 11 and Clang 14

## 2. Building

### 2.1. Building on the Command Line

To build the project on Unix like shells, build in a VPATH environment:

```sh
mkdir build
cd build
```

Then to build with the defaults:

```sh
cmake .. && make
```

### 2.2. Providing Compiler Flags

You might want to override compiler flags to enable hardening and other compiler
features. To override with your own compiler flags

```sh
cmake -DCMAKE_CXX_FLAGS="-Wunused-parameter" ..
```

or equivalently with

```sh
cmake -E env CXXFLAGS="-Wunused-parameter" cmake ..
```

### 2.3. Providing a DEBUG build

```sh
cmake -DCMAKE_BUILD_TYPE=Debug ..
```

### 2.4. Selecting the Compiler

To compile using common compilers on your local machine (Linux):

| Compiler | Command                |
|----------|------------------------|
| GCC      | `CXX=g++ cmake ..`     |
| Clang    | `CXX=clang++ cmake ..` |

For cross compiling, provide a toolchain file, e.g.

```sh
cmake .. -DCMAKE_TOOLCHAIN_FILE=../toolchain/qcc710_x86_64
```

### 2.5. Enabling Sanitizers

Sanitizers are provided by
[arsenm/sanitizers-cmake](cmake/modules/sanitizers/VERSION.md).

To build with sanitizers:

```sh
cmake .. -DSANITIZE_ADDRESS=on
VERBOSE=1 make
```

### 2.6. Building the Software

If you'd like to see information about how the software is built:

```sh
VERBOSE=1 make
```
