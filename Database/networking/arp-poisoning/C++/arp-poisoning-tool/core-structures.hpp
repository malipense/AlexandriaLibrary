#pragma once
#include "types.hpp"

typedef struct IpV4Address {
	BYTE BYTES[4];
} IPV4ADDRESS;

typedef struct MacAddress {
	BYTE BYTES[6];
} MACADDRESS;