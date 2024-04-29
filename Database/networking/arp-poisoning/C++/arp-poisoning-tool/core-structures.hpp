#pragma once
#include "types.hpp"

typedef struct IpV4Address {
	BYTE B1;
	BYTE B2;
	BYTE B3;
	BYTE B4;
} IPV4ADDRESS;

typedef struct MacAddress {
	BYTE B1;
	BYTE B2;
	BYTE B3;
	BYTE B4;
	BYTE B5;
	BYTE B6;
} MACADDRESS;