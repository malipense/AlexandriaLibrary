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

BYTE* GetBytesFromMACString(const char* str)
{
	BYTE byteArray[6];
	std::vector<BYTE> vector;

	size_t size = strlen(str);
	std::string address(12, '0');
	int index = 0;
	for (int i = 0; i < size; i++)
	{
		if (isxdigit(str[i]))
		{
			address[index] = str[i];
			
			std::cout << address[index] << std::endl;
			index++;
		}
	}

	for (int i = 0; i < 12; i += 2) {
		std::string byteString = address.substr(i, 2);
		BYTE byteValue = static_cast<BYTE>(
			stoi(byteString, nullptr, 16));

		vector.push_back(byteValue);
	}

	std::copy(vector.begin(), vector.end(), byteArray);
	return byteArray;
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
	
	if (heap == 0)
	{
		std::cerr << "Failed to allocate heap memory." << std::endl;
		exit(1);
	}

	memcpy(heap, &packet, size);

	return reinterpret_cast<PIPV4>(heap);
}

PETHERNET CreateEthernetPacket(USHORT type, const char* targetAddress, const char* sourceAddress)
{
	ValidatePhysicalAddress(targetAddress);
	ValidatePhysicalAddress(sourceAddress);

	BYTE* target = GetBytesFromMACString(targetAddress);
	BYTE* source = GetBytesFromMACString(sourceAddress);

	ETHERNET packet;
	packet.Type = type;

	packet.TargetAddress.B1 = target[0];
	packet.TargetAddress.B2 = target[1];
	packet.TargetAddress.B3 = target[2];
	packet.TargetAddress.B4 = target[3];
	packet.TargetAddress.B5 = target[4];
	packet.TargetAddress.B6 = target[5];

	packet.SourceAddress.B1 = source[0];
	packet.SourceAddress.B2 = source[1];
	packet.SourceAddress.B3 = source[2];
	packet.SourceAddress.B4 = source[3];
	packet.SourceAddress.B5 = source[4];
	packet.SourceAddress.B6 = source[5];

	int size = sizeof(packet);
	void* heap = malloc(size);

	if (heap == 0)
	{
		std::cerr << "Failed to allocate heap memory." << std::endl;
		exit(1);
	}

	memcpy(heap, &packet, size);

	return reinterpret_cast<PETHERNET>(heap);
}

PARPREPLY CreateArpReplyPacket(const char* senderAddress, const char* senderIp, const char* targetAddress, const char* targetIp)
{

}