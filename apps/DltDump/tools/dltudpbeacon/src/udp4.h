#ifndef RJCP_NET_UDP4_XX_H
#define RJCP_NET_UDP4_XX_H

#include <string>
#include <memory>
#include <vector>

#include "sockaddr4.h"

namespace rjcp::net {
    /**
     * @brief An IPv4 UDP socket implementation
     */
    class udp4 {
    public:
        /**
         * @brief Construct a new udp4 object
         */
        udp4() = default;

        /**
         * @brief Destroy the udp4 object
         */
        ~udp4() noexcept;

        /**
         * @brief Opens the socket. It isn't bound to anything.
         *
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto open() noexcept -> int;

        /**
         * @brief Tests if the socket is opened
         *
         * @return true if the socket is already open.
         * @return false if the socket is not open.
         */
        auto is_open() const noexcept -> bool;

        /**
         * @brief Enable the multicast loop to enable or disable the loopback of
         * outgoing multicast datagrams.
         *
         * Set or read a boolean integer argument that determines whether sent
         * multicast packets should be looped back to the local sockets.
         *
         * @param group The multicast group to join
         * @param enabled If loopback is enabled
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto multicast_loop(sockaddr4& group, bool enabled) noexcept -> int;

        /**
         * @brief Set the local device for a multicast socket.
         *
         * @param addr The local interface to set
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto multicast_join(sockaddr4& addr) noexcept -> int;

        /**
         * @brief Set or read the time-to-live value of outgoing multicast
         * packets for this socket
         *
         * @param ttl The Time To Live field set for the multicast packets.
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto multicast_ttl(int ttl) noexcept -> int;

        /**
         * @brief Set the socket option for reuse
         *
         * This should be called before binding the socket.
         *
         * @param reuse a boolean value if the socket address should be reused or not
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto reuseaddr(bool reuse) noexcept -> int;

        /**
         * @brief Set the amount of send buffer for the socket.
         *
         * @param sendbuf The size of the bytes to send.
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto set_sendbuf(int sendbuf) noexcept -> int;

        /**
         * @brief Get the amount of send buffer for the socket.
         *
         * @return int The amount of send buffer if the result is positive, else
         * if -1 an error occurred. Check errno.
         */
        auto get_sendbuf() noexcept -> int;

        /**
         * @brief Bind the socket to a particular address and port.
         *
         * @param addr The address to bind to.
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto bind(sockaddr4& addr) noexcept -> int;

        /**
         * @brief Sends a UDP datagram to the specified address.
         *
         * @param addr The address to send to.
         * @param buffer The binary data to send.
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto send(const sockaddr4& addr, const std::vector<uint8_t>& buffer) noexcept -> int;

        /**
         * @brief Closes the UDP socket that it can't be used.
         *
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto close() noexcept -> int;

    private:
        int m_socket_fd{-1};
    };
}

#endif
