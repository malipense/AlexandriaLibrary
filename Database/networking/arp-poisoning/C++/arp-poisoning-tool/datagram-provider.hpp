#include "packets.hpp"

PIPV4 CreateIpV4Packet(BYTE protocol, const char* sourceIp, const char* destinationIp, int identification, int ttl, BYTE* payload, int payloadSize);