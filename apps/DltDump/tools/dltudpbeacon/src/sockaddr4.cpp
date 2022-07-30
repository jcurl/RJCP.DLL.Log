#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include <cstring>
#include <iostream>

#include "sockaddr4.h"

rjcp::net::sockaddr4::sockaddr4(const std::string& addr, int port) noexcept
{
    if (port < 0 || port > 65535) {
        this->m_addr_in.sin_family = 0;
        return;
    }

    memset(&this->m_addr_in, 0, sizeof(::sockaddr_in));
    this->m_addr_in.sin_family = AF_INET;
    this->m_addr_in.sin_addr.s_addr = ::inet_addr(const_cast<char*>(addr.c_str()));
    this->m_addr_in.sin_port = htons(port);
}

bool rjcp::net::sockaddr4::is_valid() const noexcept
{
    return !(this->m_addr_in.sin_family != AF_INET ||
        this->m_addr_in.sin_addr.s_addr == INADDR_NONE);
}

const ::sockaddr_in& rjcp::net::sockaddr4::get() const noexcept
{
    return this->m_addr_in;
}
