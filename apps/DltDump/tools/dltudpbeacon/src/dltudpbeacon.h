#ifndef RJCP_DLTUDPBEACON_XX_H
#define RJCP_DLTUDPBEACON_XX_H

// The default port number for the DLT protocol.
constexpr int dlt_port(3490);

// Change this to be the multicast address to transmit the packets on.
constexpr const char* tx_multicast("239.255.42.99");

#endif