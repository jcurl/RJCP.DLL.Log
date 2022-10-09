#include <iostream>
#include <memory>
#include <vector>
#include <sstream>
#include <chrono>
#include <thread>
#include <cerrno>
#include <cstring>

#include "dlt.h"
#include "udp4.h"
#include "sockaddr4.h"

static void write_error(const std::string& message, int err)
{
    std::cout << message << "; error " << std::strerror(err) << " (" << err << ")" << std::endl;
}

static void write_error(const std::string& message)
{
    write_error(message, errno);
}

auto main(int argc, char* argv[]) -> int
{
    std::vector<std::string> arguments(argv, argv + argc);
    if (arguments.size() != 2) {
        std::cout << "Usage: " << arguments[0] << " <localaddrip>" << std::endl;
        return 1;
    }

    rjcp::net::sockaddr4 src(arguments[1], 3490);
    rjcp::net::sockaddr4 dest("239.255.42.99", 3490);
    if (!dest.is_valid()) {
        std::cout << "Usage: " << arguments[0] << " <localaddrip>" << std::endl;
        std::cout << " Invalid address" << std::endl;
        return 1;
    }

    rjcp::net::udp4 udp;

    if (udp.open() < 0) {
        write_error("open");
        return 1;
    }

    int bufsize = udp.get_sendbuf();
    std::cout << "Buffer size for socket: " << bufsize << std::endl;
    //if (udp.set_sendbuf(1480) < 0)
    //    write_error("setsockopt(SO_SENDBUF)");

    if (udp.multicast_loop(dest, false) < 0)
        write_error("setsockopt(IP_MULTICAST_LOOP)");

    if (udp.multicast_join(src) < 0)
        write_error("setsockopt(IP_MULTICAST_IF");

    if (udp.multicast_ttl(1) < 0)
        write_error("setsockopt(IP_MULTICAST_TTL");

    if (udp.reuseaddr(true) < 0)
        write_error("setsockopt(SO_REUSEADDR)");

    if (udp.bind(src) < 0)
        write_error("bind");

    rjcp::log::dlt dlt(udp, dest, "ECU1", "APP1", "CTX1");

    int num = 1;
    while(num < 1000) {
        std::stringstream ss;
        ss << "A DLT message from " << arguments[1] << ". Count is " << num;
        if (dlt.write(ss.str()) < 0)
            write_error("dlt.write()");

        num++;
        std::this_thread::sleep_for(std::chrono::milliseconds(500));
    }

    udp.close();
    return 0;
}
