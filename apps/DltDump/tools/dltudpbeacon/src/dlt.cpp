#include <iostream>
#include <cstring>
#include <algorithm>
#include <chrono>

#include "dlt.h"

static void write4hdr(const std::string& id, uint8_t* array, const std::size_t array_len)
{
    std::size_t buff_length = std::min(std::size_t{4}, array_len);
    size_t id_length = std::min(id.length(), buff_length);

    if (id_length < buff_length) {
        std::memset(array, 0, buff_length);
    }
    std::memcpy(array, id.data(), id_length);
}

rjcp::log::dlt::dlt(rjcp::net::udp4& sender, const rjcp::net::sockaddr4& dest, const std::string& ecuid, const std::string& appid, const std::string& ctxid) noexcept
    : m_sender{sender}
    , m_dest{dest}
{
    this->m_stdhdr[0] = 0x35; // UEH + WEID + WTMS + Ver1
    std::vector<uint8_t> hdr = std::vector<uint8_t>(&this->m_stdhdr[4], &this->m_stdhdr[8]);
    write4hdr(ecuid, &this->m_stdhdr[4], 4);

    this->m_exthdr[0] = 0x41; // VERB + DLT_LOG_INFO
    this->m_exthdr[1] = 0x01; // NOAR = 1
    write4hdr(appid, &this->m_exthdr[2], 4);
    write4hdr(ctxid, &this->m_exthdr[6], 4);
}

int rjcp::log::dlt::write(const std::string& message) noexcept
{
    // Length of the packet is:
    //  the size of the standard header
    //  plus the size of the extended header
    //  plus 4 bytes for the argument type info
    //  plus 2 bytes for the string length
    //  plus 1 byte for the NUL terminator.
    const std::size_t packet_len = this->m_stdhdr.size() + this->m_exthdr.size() + 7 + message.size();
    if (packet_len > 65535) {
        errno = EINVAL;
        return -1;
    }

    const std::chrono::time_point<std::chrono::steady_clock> clocktime =
        std::chrono::steady_clock::now();
    const std::chrono::steady_clock::duration duration = clocktime.time_since_epoch();
    uint32_t devtime = std::chrono::duration_cast<std::chrono::microseconds>(duration).count() / 100;

    uint8_t packet[65536] = { };
    std::memcpy(packet, this->m_stdhdr.data(), this->m_stdhdr.size());
    std::memcpy(packet + this->m_stdhdr.size(), this->m_exthdr.data(), this->m_exthdr.size());

    const std::size_t data_start = this->m_stdhdr.size() + this->m_exthdr.size();
    const std::size_t msg_len = message.size() + 1;

    packet[1] = this->m_count;
    packet[2] = (packet_len >> 8) & 0xFF;
    packet[3] = packet_len & 0xFF;
    packet[8] = (devtime >> 24) & 0xFF;
    packet[9] = (devtime >> 16) & 0xFF;
    packet[10] = (devtime >> 8) & 0xFF;
    packet[11] = devtime & 0xFF;
    packet[data_start + 1] = 0x02;  // Type is STRG. The encoding SCOD is default (ASCII).
    packet[data_start + 4] = msg_len & 0xFF;
    packet[data_start + 5] = (msg_len >> 8) & 0xFF;
    std::memcpy(packet + data_start + 6, message.c_str(), msg_len);

    this->m_count = (this->m_count + 1) & 0xFF;
    return this->m_sender.send(this->m_dest, std::vector<uint8_t>(packet, packet + packet_len));
}
