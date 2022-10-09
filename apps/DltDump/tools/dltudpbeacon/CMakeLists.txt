cmake_minimum_required(VERSION 3.10)
project(dltudpbeacon CXX)

include(CheckSymbolExists)
include(CheckLibraryExists)

find_program(CLANG_TIDY_EXE NAMES "clang-tidy")
if(CLANG_TIDY_EXE)
    # For info, see here: https://clang.llvm.org/extra/clang-tidy/
    set(CLANG_TIDY_COMMAND "${CLANG_TIDY_EXE}" "-checks=bugprone-*,modernize-*,cppcoreguidelines-*")
endif()

set(SOURCES
    src/dltudpbeacon.cpp
    src/sockaddr4.cpp
    src/udp4.cpp
    src/dlt.cpp)

# So that we can find "config.h"
include_directories("${PROJECT_BINARY_DIR}")

add_executable(dltudpbeacon ${SOURCES})
if(CLANG_TIDY_EXE)
    set_target_properties(dltudpbeacon PROPERTIES CXX_CLANG_TIDY "${CLANG_TIDY_COMMAND}")
endif()

target_compile_features(dltudpbeacon PUBLIC cxx_std_17)

if("${CMAKE_CXX_FLAGS}" STREQUAL "")
    if((CMAKE_CXX_COMPILER_ID STREQUAL "Clang") OR
       (CMAKE_CXX_COMPILER_ID STREQUAL "GNU") OR
       (CMAKE_CXX_COMPILER_ID STREQUAL "QCC"))
        target_compile_options(dltudpbeacon PRIVATE -Wall -Wextra)
    endif()
endif()
message(STATUS "Compiler '${CMAKE_CXX_COMPILER_ID}' with override options '${CMAKE_CXX_FLAGS}'")

# Search for the 'socket' natively, or in libsocket.
CHECK_SYMBOL_EXISTS(socket "arpa/inet.h" HAVE_SOCKET)
if(NOT (${HAVE_SOCKET}))
    # For example, QCC gets here as we need to add -lsocket. Linux doesn't need this.
    CHECK_LIBRARY_EXISTS("socket" "socket" "" HAVE_SOCKET_IN_LIBSOCKET)
    if (${HAVE_SOCKET_IN_LIBSOCKET})
        target_link_libraries(dltudpbeacon PRIVATE socket)
        set(HAVE_SOCKET 1)
    endif()
endif()
if (NOT (${HAVE_SOCKET}))
    message(FATAL_ERROR "socket not found")
endif()

# Check for IP_MULTICAST_LOOP
CHECK_SYMBOL_EXISTS(IP_MULTICAST_LOOP "arpa/inet.h" HAVE_IP_MULTICAST_LOOP)
CHECK_SYMBOL_EXISTS(IP_MULTICAST_IF   "arpa/inet.h" HAVE_IP_MULTICAST_IF)
CHECK_SYMBOL_EXISTS(IP_MULTICAST_TTL  "arpa/inet.h" HAVE_IP_MULTICAST_TTL)

# Finally write the configuration file dependent on what is found
configure_file(${PROJECT_SOURCE_DIR}/src/config.h.in ${PROJECT_BINARY_DIR}/config.h)
