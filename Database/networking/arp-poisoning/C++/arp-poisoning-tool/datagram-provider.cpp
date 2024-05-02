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

MACADDRESS GetMACFromMACString(const char* str)
{
	MACADDRESS mac;
	int arrayIndex = 0;

	size_t size = strlen(str);
	std::string address(12, '0');
	int addressIndex = 0;
	for (int i = 0; i < size; i++)
	{
		if (isxdigit(str[i]))
		{
			address[addressIndex] = str[i];
			addressIndex++;
		}
	}

	for (int i = 0; i < 12; i += 2) {
		std::string byteString = address.substr(i, 2);
		BYTE byteValue = static_cast<BYTE>(
			stoi(byteString, nullptr, 16));

		mac.BYTES[arrayIndex] = byteValue;
		arrayIndex++;
	}

	return mac;
}

IPV4ADDRESS GetIPFromIPString(const char* str)
{
	IPV4ADDRESS ip;
	int index = 0;
	std::vector<BYTE> vector;

	size_t size = strlen(str);

	std::string address = str;
	size_t pos = 0;

	std::string delimiter = ".";
	std::string token;
	while ((pos = address.find(delimiter)) != std::string::npos) {
		token = address.substr(0, pos);

		BYTE byteValue = static_cast<BYTE>(
			stoi(token, nullptr, 10));

		ip.BYTES[index] = byteValue;
		address.erase(0, pos + delimiter.length());
		index++;
	}
	ip.BYTES[3] = static_cast<BYTE>(stoi(address, nullptr, 10));

	return ip;
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

	MACADDRESS target = GetMACFromMACString(targetAddress);
	MACADDRESS source = GetMACFromMACString(sourceAddress);

	ETHERNET packet;
	packet.Type = type;

	packet.TargetAddress = target;
	packet.SourceAddress = source;

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
	ValidatePhysicalAddress(targetAddress);
	ValidatePhysicalAddress(senderAddress);

	MACADDRESS bTarget = GetMACFromMACString(targetAddress);
	MACADDRESS bSender = GetMACFromMACString(senderAddress);

	IPV4ADDRESS bTargetIp = GetIPFromIPString(targetIp);
	IPV4ADDRESS bSenderIp = GetIPFromIPString(senderIp);

	ARPREPLY packet;

	packet.TargetAddress = bTarget;
	packet.SenderAddress = bSender;

	packet.TargetIp = bTargetIp;
	packet.SenderIp = bSenderIp;

	int size = sizeof(packet);
	void* heap = malloc(size);

	if (heap == 0)
	{
		std::cerr << "Failed to allocate heap memory." << std::endl;
		exit(1);
	}

	memcpy(heap, &packet, size);

	return reinterpret_cast<PARPREPLY>(heap);
}