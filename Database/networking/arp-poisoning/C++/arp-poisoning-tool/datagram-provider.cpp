#include "datagram-provider.hpp"
#include <regex>
#include "utilities.hpp"
#include <iostream>

void ValidateIpv4Address(const char* ipAddress)
{
	const char* pattern = "(([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])";

	if (!std::regex_match(ipAddress, std::regex(pattern)))
	{
		std::cerr << "Invalid format. Please provide a ip address with the following notation: 10.10.10.10" << std::endl;
		exit(1);
	}
}

void ValidatePhysicalAddress(const char* physicalAddress)
{
	const char* pattern(
		"^([0-9A-Fa-f]{2}[:-]){5}"
		"([0-9A-Fa-f]{2})|([0-9a-"
		"fA-F]{4}\\.[0-9a-fA-F]"
		"{4}\\.[0-9a-fA-F]{4})$");

	if (!std::regex_match(physicalAddress, std::regex(pattern)))
	{
		std::cerr << "Invalid format.Please provide a mac address with the following notation : ff:ff:ff:ff:ff:ff or ff - ff - ff - ff - ff - ff" << std::endl;
		exit(1);
	}
}

PIPV4 CreateIpV4Packet(BYTE protocol, const char* sourceIp, const char* destinationIp, int identification, int ttl, BYTE* payload, int payloadSize)
{
	if (ttl > 255 || ttl < 1)
	{
		std::cerr << "TTL should be a value between 1 and 255" << std::endl;
		exit(1);
	}

	if (identification > 65535)
	{
		std::cerr << "Identification field should exceed 65535" << std::endl;
		exit(1);
	}

	int totalLength = sizeof(IPV4);
	if (payload != NULL)
		totalLength = totalLength + payloadSize;

	ValidateIpv4Address(sourceIp);
	ValidateIpv4Address(destinationIp);

	IPV4 packet;
	packet.Identification = 0;
	packet.TTL = ttl;
	packet.Protocol = protocol;
	packet.TotalLength = 0;

	int size = sizeof(packet);
	void* heap = malloc(size);
	memcpy(heap, &packet, size);

	return reinterpret_cast<PIPV4>(heap);
}