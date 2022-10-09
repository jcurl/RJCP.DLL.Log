#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include <cstring>
#include <iostream>
#include <limits>

#include "sockaddr4.h"

// In IPv4, the port number is 16-bits (uint16_t), so this is the maximum port
// number allowed.
constexpr int max_port = std::numeric_limits<uint16_t>::max();

rjcp::net::sockaddr4::sockaddr4(const std::string& addr, int port) noexcept
{
    if (port < 0 || port > max_port) {
        this->m_addr_in.sin_family = 0;
        return;
    }

    memset(&this->m_addr_in, 0, sizeof(::sockaddr_in));
    this->m_addr_in.sin_family = AF_INET;
    this->m_addr_in.sin_addr.s_addr = ::inet_addr(const_cast<char*>(addr.c_str()));
    this->m_addr_in.sin_port = htons(port);
}

auto rjcp::net::sockaddr4::is_valid() const noexcept -> bool
{
    return !(this->m_addr_in.sin_family != AF_INET ||
        this->m_addr_in.sin_addr.s_addr == INADDR_NONE);
}

auto rjcp::net::sockaddr4::get() const noexcept -> const ::sockaddr_in&
{
    return this->m_addr_in;
}
