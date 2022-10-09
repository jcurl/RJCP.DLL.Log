#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include <cerrno>
#include <iostream>
#include <limits>
#include <memory>
#include <string>

#include "config.h"
#include "udp4.h"

// In IPv4, the port number is 16-bits (uint16_t), so this is the maximum port
// number allowed.
constexpr int max_ttl = std::numeric_limits<uint8_t>::max();

rjcp::net::udp4::~udp4() noexcept
{
    close();
}

auto rjcp::net::udp4::open() noexcept -> int
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

auto rjcp::net::udp4::is_open() const noexcept -> bool
{
    return this->m_socket_fd != -1;
}

auto rjcp::net::udp4::reuseaddr(bool reuse) noexcept -> int
{
    if (!this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    int value = reuse ? 1 : 0;
    return ::setsockopt(this->m_socket_fd, SOL_SOCKET, SO_REUSEADDR,
        &value, sizeof(value));
}

auto rjcp::net::udp4::multicast_loop(sockaddr4& group, bool enabled) noexcept -> int
{
    if (!group.is_valid() || !this->is_open()) {
        errno = EINVAL;
        return -1;
    }

#ifdef HAVE_IP_MULTICAST_LOOP
    char loopch = enabled ? 1 : 0;
    return ::setsockopt(this->m_socket_fd, IPPROTO_IP, IP_MULTICAST_LOOP,
        &loopch, sizeof(loopch));
#else
    return 0;
#endif
}

auto rjcp::net::udp4::multicast_join(sockaddr4& addr) noexcept -> int
{
    if (!addr.is_valid() || !this->is_open()) {
        errno = EINVAL;
        return -1;
    }

#ifdef HAVE_IP_MULTICAST_IF
    ::sockaddr_in localaddr = addr.get();
    return ::setsockopt(this->m_socket_fd, IPPROTO_IP, IP_MULTICAST_IF,
        &(localaddr.sin_addr), sizeof(in_addr));
#else
    return 0;
#endif
}

auto rjcp::net::udp4::multicast_ttl(int ttl) noexcept -> int
{
    if (ttl <= 0 || ttl > max_ttl || !this->is_open()) {
        errno = EINVAL;
        return -1;
    }

#ifdef HAVE_IP_MULTICAST_TTL
    unsigned char mttl = ttl;
    return ::setsockopt(this->m_socket_fd, IPPROTO_IP, IP_MULTICAST_TTL,
        &mttl, sizeof(mttl));
#else
    return 0;
#endif
}

auto rjcp::net::udp4::set_sendbuf(int sendbuf) noexcept -> int
{
    if (sendbuf <= 0) {
        errno = EINVAL;
        return -1;
    }

    return ::setsockopt(this->m_socket_fd, SOL_SOCKET, SO_SNDBUF,
        &sendbuf, sizeof(sendbuf));
}

auto rjcp::net::udp4::get_sendbuf() noexcept -> int
{
    if (!this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    int buffsize;
    socklen_t optlen = sizeof(buffsize);
    int res = ::getsockopt(this->m_socket_fd, SOL_SOCKET, SO_SNDBUF,
        &buffsize, &optlen);
    if (res < 0) return res;
    return buffsize;
}

auto rjcp::net::udp4::bind(sockaddr4& addr) noexcept -> int
{
    if (!addr.is_valid() || !this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    return ::bind(
        this->m_socket_fd,
        reinterpret_cast<const ::sockaddr*>(&addr.get()), sizeof(::sockaddr_in));
}

auto rjcp::net::udp4::send(const sockaddr4& addr, const std::vector<uint8_t>& buffer) noexcept -> int
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

auto rjcp::net::udp4::close() noexcept -> int
{
    if (this->is_open()) {
        errno = EINVAL;
        return -1;
    }

    return ::close(this->m_socket_fd);
}
