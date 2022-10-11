#ifndef RJCP_DLT_XX_H
#define RJCP_DLT_XX_H

#include <array>
#include <cstddef>
#include <vector>

#include "sockaddr4.h"
#include "udp4.h"

namespace rjcp::log {
    /**
     * @brief A very simple class to send out DLT messages as strings.
     *
     * You should assume that all methods are not thread safe.
     *
     */
    class dlt {
    public:
        /**
         * @brief Construct a new dlt object
         *
         * @param sender The sender socket. Must already be opened and bound to before writing.
         * @param dest The address to send to (could be a multicast address).
         * @param ecuid The ECU-ID (4 characters) in the standard header.
         * @param appid The Application-ID (4 characters) in the extended header.
         * @param ctxid The Context-ID (4 characters) in the extended header.
         */
        dlt(rjcp::net::udp4& sender, const rjcp::net::sockaddr4& dest, const std::string& ecuid, const std::string& appid, const std::string& ctxid) noexcept;

        /**
         * @brief Destroy the dlt object
         *
         */
        ~dlt() = default;

        /**
         * @brief Write the string message as a DLT packet
         *
         * Writes the message given as a single DLT packet, with a single argument. The message type is treated as
         * and encoded as such.
         *
         * @param message The payload string
         * @return int Success if zero, -1 on error. Check errno.
         */
        auto write(const std::string& message) noexcept -> int;

    private:
        rjcp::net::udp4& m_sender;
        const rjcp::net::sockaddr4& m_dest;
        std::uint8_t m_count{0};
        std::vector<uint8_t> m_packet;
    };
}

#endif
