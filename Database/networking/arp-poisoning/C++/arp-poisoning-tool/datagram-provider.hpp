#include "packets.hpp"

PIPV4 CreateIpV4Packet(BYTE protocol, const char* sourceIp, const char* destinationIp, int identification, int ttl, BYTE* payload, int payloadSize);

PETHERNET CreateEthernetPacket(USHORT type, const char* targetAddress, const char* sourceAddress);

PARPREPLY CreateArpReplyPacket(const char* senderAddress, const char* senderIp, const char* targetAddress, const char* targetIp);