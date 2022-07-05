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

bool rjcp::net::udp4::is_open() const noexcept
{
    return this->m_socket_fd != -1;
}

int rjcp::net::udp4::reuseaddr(bool reuse) noexcept
{
    if (!this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    int value = reuse ? 1 : 0;
    int result = ::setsockopt(this->m_socket_fd, SOL_SOCKET, SO_REUSEADDR, &value, sizeof(value));
    if (result < 0) {
        return -1;
    }
    return 0;
}

int rjcp::net::udp4::bind(sockaddr4& addr) noexcept
{
    if (!addr.is_valid() || !this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    return ::bind(
        this->m_socket_fd,
        reinterpret_cast<const ::sockaddr*>(&addr.get()), sizeof(::sockaddr_in));
}

int rjcp::net::udp4::send(const sockaddr4& addr, const std::vector<uint8_t>& buffer) noexcept
{
    if (!addr.is_valid() || !this->is_open()) {
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
    if (this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    return ::close(this->m_socket_fd);
}
