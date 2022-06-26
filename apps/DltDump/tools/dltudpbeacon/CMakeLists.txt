cmake_minimum_required(VERSION 3.10)
project(dltudpbeacon)

set(SOURCES
    src/dltudpbeacon.cpp
    src/sockaddr4.cpp
    src/udp4.cpp
    src/dlt.cpp)

add_executable(dltudpbeacon ${SOURCES})

target_compile_features(dltudpbeacon PUBLIC cxx_std_17)

if("${CMAKE_CXX_FLAGS}" STREQUAL "")
    if((CMAKE_CXX_COMPILER_ID STREQUAL "Clang") OR (CMAKE_CXX_COMPILER_ID STREQUAL "GNU"))
        target_compile_options(dltudpbeacon PRIVATE -Wall -Wextra)
    endif()
endif()
message(STATUS "Compiler '${CMAKE_CXX_COMPILER_ID}' with override options '${CMAKE_CXX_FLAGS}'")
