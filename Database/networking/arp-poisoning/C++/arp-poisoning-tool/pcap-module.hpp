#include "pcap.h"
#include "types.hpp"

pcap_t* Open(const char* deviceName);

void Send(pcap_t* fp, BYTE* packet, int size);