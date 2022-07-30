cmake_minimum_required(VERSION 3.10)
project(dltudpbeacon CXX)

include(CheckSymbolExists)
include(CheckLibraryExists)

set(SOURCES
    src/dltudpbeacon.cpp
    src/sockaddr4.cpp
    src/udp4.cpp
    src/dlt.cpp)

add_executable(dltudpbeacon ${SOURCES})

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
