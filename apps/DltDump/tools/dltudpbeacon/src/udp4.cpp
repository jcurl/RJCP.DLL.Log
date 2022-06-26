#include <iostream>
#include <string>
#include <memory>
#include <cerrno>

#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include "udp4.h"

rjcp::net::udp4::~udp4() noexcept
{
    close();
}

int rjcp::net::udp4::open() noexcept
{
    if (this->m_socket_fd != -1) {
        errno = EINVAL;
        return -1;
    }

    int fd = ::socket(AF_INET, SOCK_DGRAM, 0);
    if (fd < 0)
        return -1;

    this->m_socket_fd = fd;
    return 0;
}

int rjcp::net::udp4::bind(sockaddr4& addr) noexcept
{
    if (!addr.is_valid() || this->m_socket_fd == -1) {
        errno = EINVAL;
        return -1;
    }

    return ::bind(
        this->m_socket_fd,
        reinterpret_cast<const ::sockaddr*>(&addr.get()), sizeof(::sockaddr_in));
}

int rjcp::net::udp4::send(const sockaddr4& addr, const std::vector<uint8_t>& buffer) noexcept
{
    if (!addr.is_valid() || this->m_socket_fd == -1) {
        errno = EINVAL;
        return -1;
    }

    int nbytes = ::sendto(
        this->m_socket_fd,
        buffer.data(), buffer.size(),
        0,
        reinterpret_cast<const ::sockaddr*>(&addr.get()), sizeof(::sockaddr_in));

    if (nbytes < 0)
        return -1;

    return 0;
}

int rjcp::net::udp4::close() noexcept
{
    if (this->m_socket_fd == -1) {
        errno = EINVAL;
        return -1;
    }

    return ::close(this->m_socket_fd);
}
