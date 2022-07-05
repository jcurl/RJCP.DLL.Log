#include <iostream>
#include <memory>
#include <vector>
#include <sstream>
#include <chrono>
#include <thread>

#include "dlt.h"
#include "udp4.h"
#include "sockaddr4.h"

int main(int argc, char* argv[])
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

    if (udp.open() < 0)
        perror("open");

    if (udp.reuseaddr(true) < 0)
        perror("setsockopt(SO_REUSEADDR)");

    if (udp.bind(src) < 0)
        perror("bind");

    rjcp::log::dlt dlt(udp, dest, "ECU1", "APP1", "CTX1");

    int num = 1;
    while(num < 1000) {
        std::stringstream ss;
        ss << "A DLT message from " << arguments[1] << ". Count is " << num;
        if (dlt.write(ss.str()) < 0)
            perror("dlt.write()");

        num++;
        std::this_thread::sleep_for(std::chrono::milliseconds(500));
    }

    udp.close();
    return 0;
}
