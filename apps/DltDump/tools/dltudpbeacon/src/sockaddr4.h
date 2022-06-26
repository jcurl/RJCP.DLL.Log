#ifndef RJCP_NET_SOCKADDR4_XX_H
#define RJCP_NET_SOCKADDR4_XX_H

#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include <string>
#include <memory>

namespace rjcp::net {
    /**
     * @brief Represents an IPv4 socket address and port.
     */
    class sockaddr4 {
    public:
        /**
         * @brief Construct a new sockaddr4 object
         *
         * @param addr The address using 4-dot notation, e.g. "127.0.0.1"
         * @param port The port, in the range of 0 to 65535.
         */
        sockaddr4(const std::string& addr, int port) noexcept;

        /**
         * @brief Destroy the sockaddr4 object
         */
        ~sockaddr4() = default;

        /**
         * @brief Checks if this instance is valid.
         *
         * @return true if this instance is valid.
         * @return false if this instance was initialized incorrectly.
         */
        bool is_valid() const noexcept;

        /**
         * @brief Gets the ::sockaddr_in reference, that can be used with sock API.
         *
         * @return const ::sockaddr_in& the socket structure to give to sock API.
         */
        const ::sockaddr_in& get() const noexcept;

    private:
        ::sockaddr_in m_addr_in;
    };
}

#endif
