#include <algorithm>
#include <chrono>
#include <cstring>
#include <iostream>
#include <limits>

#include "dlt.h"

constexpr int max_dlt_len = std::numeric_limits<uint16_t>().max();

constexpr int dlt_stdhdr_off_htyp = 0;      // Offset in the stdhdr where the HTYP field is kept
constexpr int dlt_stdhdr_off_mcnt = 1;      // Offset in the stdhdr where the MCNT field is kept
constexpr int dlt_stdhdr_off_len = 2;       // Offset in the stdhdr where the LEN field is kept
constexpr int dlt_stdhdr_off_optional = 4;  // Offset in the stdhdr where optional data starts
constexpr int dlt_stdhdr_len_ecuid = 4;     // Length of the ECUID field in the stdheader (optional)
constexpr int dlt_stdhdr_len_seid = 4;      // Length of the Session ID field in the stdheader (optional)
constexpr int dlt_stdhdr_len_tmsp = 4;      // Length of the Time Stamp field in the stdheader (optional)

constexpr uint8_t dlt_htyp_ueh = 1 << 0;    // HTYP.UEH: Use Extended Header bit
constexpr uint8_t dlt_htyp_msbf = 1 << 1;   // HTYP.MSBF: Most Significant Bit First
constexpr uint8_t dlt_htyp_weid = 1 << 2;   // HTYP.WEID: With ECU ID NOLINT
constexpr uint8_t dlt_htyp_wsid = 1 << 3;   // HTYP.WSID: With Session ID
constexpr uint8_t dlt_htyp_wtms = 1 << 4;   // HTYP.WTMS: With Time Stamp
constexpr uint8_t dlt_htyp_vers = 1 << 5;   // HTYP.VERS: Version 1

constexpr uint8_t dlt_htyp = dlt_htyp_ueh + dlt_htyp_weid + dlt_htyp_wtms + dlt_htyp_vers;

constexpr int dlt_stdhdr_len =              // For dlt_htyp=0x35 this is 12.
    dlt_stdhdr_off_optional +
    ((dlt_htyp & dlt_htyp_weid) ? dlt_stdhdr_len_ecuid : 0) +
    ((dlt_htyp & dlt_htyp_wsid) ? dlt_stdhdr_len_seid : 0) +
    ((dlt_htyp & dlt_htyp_wtms) ? dlt_stdhdr_len_tmsp : 0);
constexpr int dlt_stdhdr_off_time =
    dlt_stdhdr_off_optional +
    ((dlt_htyp & dlt_htyp_weid) ? dlt_stdhdr_len_ecuid : 0) +
    ((dlt_htyp & dlt_htyp_wsid) ? dlt_stdhdr_len_seid : 0);

constexpr uint8_t dlt_exthdr_msin_verbose = 1 << 0;
constexpr uint8_t dlt_exthdr_mstp_dltlogfatal = 0x10;
constexpr uint8_t dlt_exthdr_mstp_dltlogerror = 0x20;
constexpr uint8_t dlt_exthdr_mstp_dltlogwarn = 0x30;
constexpr uint8_t dlt_exthdr_mstp_dltloginfo = 0x40;
constexpr uint8_t dlt_exthdr_mstp_dltlogdebug = 0x50;
constexpr uint8_t dlt_exthdr_mstp_dltlogverbose = 0x60;

constexpr int dlt_arg_len_typeinfo = 4;     // Lenth of the argument typeinfo field
constexpr int dlt_arg_string_len_len = 2;   // Length of the string argument length field
constexpr int dlt_arg_string_len_null = 1;  // Length of the string argument null terminator
constexpr int dlt_arg_string_off_payload =
    dlt_arg_len_typeinfo + dlt_arg_string_len_len;

constexpr uint32_t dlt_arg_typeinfo_string = 0x00000200;

constexpr int dlt_chrono_time = 100;        // Convert microseconds to dlt units

static void write4hdr(const std::string& id, uint8_t* array, const std::size_t array_len)
{
    std::size_t buff_length = std::min(std::size_t{4}, array_len);
    size_t id_length = std::min(id.length(), buff_length);

    if (id_length < buff_length) {
        std::memset(array, 0, buff_length);
    }
    std::memcpy(array, id.data(), id_length);
}

static void write16be(uint8_t *buffer, uint16_t value)
{
    buffer[0] = (value >> 8) & 0xFF;
    buffer[1] = value & 0xFF;
}

static void write16le(uint8_t *buffer, uint16_t value)
{
    buffer[0] = value & 0xFF;
    buffer[1] = (value >> 8) & 0xFF;
}

static void write32be(uint8_t *buffer, uint32_t value)
{
    buffer[0] = (value >> 24) & 0xFF;
    buffer[1] = (value >> 16) & 0xFF;
    buffer[2] = (value >> 8) & 0xFF;
    buffer[3] = value & 0xFF;
}

static void write32le(uint8_t *buffer, uint32_t value)
{
    buffer[0] = value & 0xFF;
    buffer[1] = (value >> 8) & 0xFF;
    buffer[2] = (value >> 16) & 0xFF;
    buffer[3] = (value >> 24) & 0xFF;
}

// NOLINTNEXTLINE(cppcoreguidelines-pro-type-member-init): the two arrays are initalized in the body
rjcp::log::dlt::dlt(rjcp::net::udp4& sender, const rjcp::net::sockaddr4& dest, const std::string& ecuid, const std::string& appid, const std::string& ctxid) noexcept
    : m_sender{sender}
    , m_dest{dest}
{
    this->m_stdhdr[dlt_stdhdr_off_htyp] = dlt_htyp;
    write4hdr(ecuid, &this->m_stdhdr[dlt_stdhdr_off_optional], dlt_stdhdr_len_ecuid);

    constexpr uint8_t dlt_exthdr_mstp_noar = 1;

    this->m_exthdr[0] = dlt_exthdr_mstp_dltloginfo + dlt_exthdr_msin_verbose;
    this->m_exthdr[1] = dlt_exthdr_mstp_noar;
    write4hdr(appid, &this->m_exthdr[2], 4);
    write4hdr(ctxid, &this->m_exthdr[6], 4);
}

auto rjcp::log::dlt::write(const std::string& message) noexcept -> int
{
    constexpr std::size_t dlt_payload_off =
        dlt_stdhdr_len + max_dlt_exthdr_len;

    const std::size_t packet_len =
        dlt_stdhdr_len + max_dlt_exthdr_len +
        dlt_arg_len_typeinfo +
        dlt_arg_string_len_len +
        message.size() +
        dlt_arg_string_len_null;

    if (packet_len > max_dlt_len) {
        errno = EINVAL;
        return -1;
    }

    const std::chrono::time_point<std::chrono::steady_clock> clocktime = std::chrono::steady_clock::now();
    const std::chrono::steady_clock::duration duration = clocktime.time_since_epoch();
    const uint32_t devtime = std::chrono::duration_cast<std::chrono::microseconds>(duration).count() / dlt_chrono_time;

    uint8_t packet[max_dlt_len + 1] = {};
    std::memcpy(packet, this->m_stdhdr.data(), dlt_stdhdr_len);
    std::memcpy(packet + dlt_stdhdr_len, this->m_exthdr.data(), this->m_exthdr.size());

    const std::size_t msg_len = message.size() + dlt_arg_string_len_null;

    packet[dlt_stdhdr_off_mcnt] = this->m_count;
    write16be(packet + dlt_stdhdr_off_len, packet_len);
    write32be(packet + dlt_stdhdr_off_time, devtime);
    write32le(packet + dlt_payload_off, dlt_arg_typeinfo_string);
    write16le(packet + dlt_payload_off + dlt_arg_len_typeinfo, msg_len);
    std::memcpy(packet + dlt_payload_off + dlt_arg_string_off_payload, message.c_str(), msg_len);
    packet[dlt_payload_off + dlt_arg_string_off_payload + msg_len] = 0;

    this->m_count = (this->m_count + 1) & 0xFF;
    return this->m_sender.send(this->m_dest, std::vector<uint8_t>(packet, packet + packet_len));
}
