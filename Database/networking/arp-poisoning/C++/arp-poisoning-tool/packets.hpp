#pragma once
#include "types.hpp"
#include "core-structures.hpp"

typedef struct LoopBack {
	UINT Family;
} LOOPBACK, * PLOOPBACK;

typedef struct Ethernet {
	MACADDRESS TargetAddress;
	MACADDRESS SourceAddress;
	USHORT Type;
} ETHERNET, * PETHERNET;

typedef struct IpV4 {
	BYTE Version = 0x45;
	BYTE DifferentiateService = 0;
	USHORT TotalLength = 0;
	USHORT Identification = 0;
	BYTE Flags = 0x0;
	BYTE FragmentOffset = 0x0;
	BYTE TTL = 0x80;
	BYTE Protocol = 0;
	USHORT HeaderChecksum = 0;
	IPV4ADDRESS SourceAddress = { 0, 0, 0, 0 };
	IPV4ADDRESS DestinationAddress = { 0, 0, 0, 0 };
} IPV4, *PIPV4;

typedef struct ArpReply {
	USHORT HardwareType = 0x0100;
	USHORT ProtocolType = 0x08;
	BYTE HardwareSize = 0x6;
	BYTE ProtocolSize = 0x4;
	USHORT OpCode = 0x0200;
	MACADDRESS SenderAddress = { 0, 0, 0, 0, 0, 0 };
	IPV4ADDRESS SenderIp = { 0, 0, 0, 0 };
	MACADDRESS TargetAddress = { 0, 0, 0, 0, 0, 0 };
	IPV4ADDRESS TargetIp = { 0, 0, 0, 0 };
} ARPREPLY, *PARPREPLY;
