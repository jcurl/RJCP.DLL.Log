#include <algorithm>
#include <cassert>
#include <chrono>
#include <cstring>
#include <iostream>
#include <limits>

#include "dlt.h"

constexpr int max_dlt_len = std::numeric_limits<uint16_t>().max();

constexpr int dlt_id_len = 4;

constexpr int dlt_stdhdr_off_htyp = 0;           // Offset in the stdhdr where the HTYP field is kept
constexpr int dlt_stdhdr_off_mcnt = 1;           // Offset in the stdhdr where the MCNT field is kept
constexpr int dlt_stdhdr_off_len = 2;            // Offset in the stdhdr where the LEN field is kept
constexpr int dlt_stdhdr_off_optional = 4;       // Offset in the stdhdr where optional data starts
constexpr int dlt_stdhdr_len_ecuid = dlt_id_len; // Length of the ECUID field in the stdheader (optional)
constexpr int dlt_stdhdr_len_seid = 4;           // Length of the Session ID field in the stdheader (optional)
constexpr int dlt_stdhdr_len_tmsp = 4;           // Length of the Time Stamp field in the stdheader (optional)

constexpr uint8_t dlt_htyp_ueh = 1 << 0;         // HTYP.UEH: Use Extended Header bit
constexpr uint8_t dlt_htyp_msbf = 1 << 1;        // HTYP.MSBF: Most Significant Bit First
constexpr uint8_t dlt_htyp_weid = 1 << 2;        // HTYP.WEID: With ECU ID NOLINT
constexpr uint8_t dlt_htyp_wsid = 1 << 3;        // HTYP.WSID: With Session ID
constexpr uint8_t dlt_htyp_wtms = 1 << 4;        // HTYP.WTMS: With Time Stamp
constexpr uint8_t dlt_htyp_vers = 1 << 5;        // HTYP.VERS: Version 1

constexpr uint8_t dlt_htyp = dlt_htyp_ueh + dlt_htyp_weid + dlt_htyp_wtms + dlt_htyp_vers;

constexpr int dlt_stdhdr_len =                   // For dlt_htyp=0x35 this is 12.
    dlt_stdhdr_off_optional +
    ((dlt_htyp & dlt_htyp_weid) ? dlt_stdhdr_len_ecuid : 0) +
    ((dlt_htyp & dlt_htyp_wsid) ? dlt_stdhdr_len_seid : 0) +
    ((dlt_htyp & dlt_htyp_wtms) ? dlt_stdhdr_len_tmsp : 0);
constexpr int dlt_stdhdr_off_time =
    dlt_stdhdr_off_optional +
    ((dlt_htyp & dlt_htyp_weid) ? dlt_stdhdr_len_ecuid : 0) +
    ((dlt_htyp & dlt_htyp_wsid) ? dlt_stdhdr_len_seid : 0);

constexpr int dlt_exthdr_off_msin = dlt_stdhdr_len;
constexpr int dlt_exthdr_off_noar = dlt_stdhdr_len + 1;
constexpr int dlt_exthdr_off_appid = dlt_stdhdr_len + 2;
constexpr int dlt_exthdr_len_appid = dlt_id_len;
constexpr int dlt_exthdr_off_ctxid = dlt_exthdr_off_appid + dlt_exthdr_len_appid;
constexpr int dlt_exthdr_len_ctxid = dlt_id_len;

constexpr int dlt_payload_off = dlt_exthdr_off_ctxid + dlt_exthdr_len_ctxid;

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

static void write4hdr(const std::string& id, std::vector<uint8_t>& packet, const std::size_t offset)
{
    assert(offset + dlt_id_len <= packet.size());

    size_t id_length = std::min(id.length(), size_t(dlt_id_len));
    if (id_length < dlt_id_len)
        std::memset(packet.data(), 0, dlt_id_len);

    std::memcpy(&packet[offset], id.data(), id_length);
}

template<typename T>
void write16be(std::vector<T>& buffer, const std::size_t offset, const uint16_t value)
{
    buffer[offset]     = (value >> 8) & 0xFF;      // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 1] = value & 0xFF;             // NOLINT(cppcoreguidelines-avoid-magic-numbers)
}

template<typename T>
void write16le(std::vector<T>& buffer, const std::size_t offset, const uint16_t value)
{
    buffer[offset]     = value & 0xFF;             // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 1] = (value >> 8) & 0xFF;      // NOLINT(cppcoreguidelines-avoid-magic-numbers)
}

template<typename T>
void write32be(std::vector<T>&buffer, const std::size_t offset, const uint32_t value)
{
    buffer[offset]     = (value >> 24) & 0xFF;     // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 1] = (value >> 16) & 0xFF;     // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 2] = (value >> 8) & 0xFF;      // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 3] = value & 0xFF;             // NOLINT(cppcoreguidelines-avoid-magic-numbers)
}

template<typename T>
void write32le(std::vector<T>& buffer, const std::size_t offset, const uint32_t value)
{
    buffer[offset]     = value & 0xFF;             // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 1] = (value >> 8) & 0xFF;      // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 2] = (value >> 16) & 0xFF;     // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    buffer[offset + 3] = (value >> 24) & 0xFF;     // NOLINT(cppcoreguidelines-avoid-magic-numbers)
}

rjcp::log::dlt::dlt(rjcp::net::udp4& sender, const rjcp::net::sockaddr4& dest, const std::string& ecuid, const std::string& appid, const std::string& ctxid) noexcept
    : m_sender{sender}
    , m_dest{dest}
    , m_packet(max_dlt_len + 1)
{
    this->m_packet[dlt_stdhdr_off_htyp] = dlt_htyp;
    write4hdr(ecuid, m_packet, dlt_stdhdr_off_optional);

    constexpr uint8_t dlt_exthdr_mstp_noar = 1;

    this->m_packet[dlt_exthdr_off_msin] = dlt_exthdr_mstp_dltloginfo + dlt_exthdr_msin_verbose;
    this->m_packet[dlt_exthdr_off_noar] = dlt_exthdr_mstp_noar;
    write4hdr(appid, m_packet, dlt_exthdr_off_appid);
    write4hdr(ctxid, m_packet, dlt_exthdr_off_ctxid);
}

auto rjcp::log::dlt::write(const std::string& message) noexcept -> int
{
    const std::size_t msg_len = message.size() + dlt_arg_string_len_null;
    const std::size_t packet_len =
        dlt_payload_off +
        dlt_arg_string_off_payload +
        msg_len;

    if (packet_len > max_dlt_len) {
        errno = EINVAL;
        return -1;
    }

    const std::chrono::time_point<std::chrono::steady_clock> clocktime = std::chrono::steady_clock::now();
    const std::chrono::steady_clock::duration duration = clocktime.time_since_epoch();
    const uint32_t devtime = std::chrono::duration_cast<std::chrono::microseconds>(duration).count() / dlt_chrono_time;

    this->m_packet[dlt_stdhdr_off_mcnt] = this->m_count;
    write16be(this->m_packet, dlt_stdhdr_off_len, packet_len);
    write32be(this->m_packet, dlt_stdhdr_off_time, devtime);
    write32le(this->m_packet, dlt_payload_off, dlt_arg_typeinfo_string);
    write16le(this->m_packet, dlt_payload_off + dlt_arg_len_typeinfo, msg_len);
    std::memcpy(&this->m_packet[dlt_payload_off + dlt_arg_string_off_payload], message.c_str(), msg_len);
    this->m_packet[packet_len] = 0;

    this->m_count = (this->m_count + 1) & 0xFF;  // NOLINT(cppcoreguidelines-avoid-magic-numbers)
    return this->m_sender.send(this->m_dest, this->m_packet, packet_len);
}
