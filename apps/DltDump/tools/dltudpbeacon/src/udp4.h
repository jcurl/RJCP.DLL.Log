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
        int open() noexcept;

        /**
         * @brief Bind the socket to a particular address and port.
         *
         * @param addr The address to bind to.
         * @return int Success if zero, -1 on error. Check errno.
         */
        int bind(sockaddr4& addr) noexcept;

        /**
         * @brief Sends a UDP datagram to the specified address.
         *
         * @param addr The address to send to.
         * @param buffer The binary data to send.
         * @return int Success if zero, -1 on error. Check errno.
         */
        int send(const sockaddr4& addr, const std::vector<uint8_t>& buffer) noexcept;

        /**
         * @brief Closes the UDP socket that it can't be used.
         *
         * @return int Success if zero, -1 on error. Check errno.
         */
        int close() noexcept;

    private:
        int m_socket_fd{-1};
    };
}

#endif
