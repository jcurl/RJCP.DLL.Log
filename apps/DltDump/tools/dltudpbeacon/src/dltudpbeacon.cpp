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

    if (udp.open() < 0) {
        write_error("open");
        return 1;
    }

    int bufsize = udp.get_sendbuf();
    std::cout << "Buffer size for socket: " << bufsize << std::endl;

    // Test for reducing buffer size
    if (udp.set_sendbuf(1480) < 0)
        write_error("setsockopt(SO_SENDBUF)");
    bufsize = udp.get_sendbuf();
    std::cout << "New Buffer size for socket: " << bufsize << std::endl;

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
        ss << "A DLT message from " << arguments[1] << ". Count is " << num << ". ";
        ss << "Padding follows to make the packet greater than 1500 bytes, such ";
        ss << "as to cause IPv4 fragmentation. ";
        ss << "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        ss << "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
        ss << "cccccccccccccccccccccccccccccccccccccccccccccccccc";
        ss << "dddddddddddddddddddddddddddddddddddddddddddddddddd";
        ss << "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";
        ss << "ffffffffffffffffffffffffffffffffffffffffffffffffff";
        ss << "gggggggggggggggggggggggggggggggggggggggggggggggggg";
        ss << "hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh";
        ss << "iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii";
        ss << "jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj";
        ss << "kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk";
        ss << "llllllllllllllllllllllllllllllllllllllllllllllllll";
        ss << "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm";
        ss << "nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn";
        ss << "oooooooooooooooooooooooooooooooooooooooooooooooooo";
        ss << "pppppppppppppppppppppppppppppppppppppppppppppppppp";
        ss << "qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq";
        ss << "rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr";
        ss << "ssssssssssssssssssssssssssssssssssssssssssssssssss";
        ss << "tttttttttttttttttttttttttttttttttttttttttttttttttt";
        ss << "uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu";
        ss << "vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv";
        ss << "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww";
        ss << "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        ss << "yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy";
        ss << "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
        ss << "00000000000000000000000000000000000000000000000000";
        ss << "11111111111111111111111111111111111111111111111111";
        ss << "22222222222222222222222222222222222222222222222222";
        ss << "33333333333333333333333333333333333333333333333333";
        ss << "44444444444444444444444444444444444444444444444444";
        ss << "55555555555555555555555555555555555555555555555555";
        ss << "66666666666666666666666666666666666666666666666666";
        ss << "77777777777777777777777777777777777777777777777777";
        ss << "88888888888888888888888888888888888888888888888888";
        ss << "99999999999999999999999999999999999999999999999999";
        if (dlt.write(ss.str()) < 0)
            write_error("dlt.write()");

        num++;
        std::this_thread::sleep_for(std::chrono::milliseconds(500));
    }

    udp.close();
    return 0;
}
