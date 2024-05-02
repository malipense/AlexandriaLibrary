#include "pcap-module.hpp"
#include <iostream>
#pragma warning(disable:4996)

pcap_t* Open(const char* deviceName) {
    char errbuf[PCAP_ERRBUF_SIZE];
    pcap_t* fp;

    char qualifiedName[256] = "rpcap://";
    strcat(qualifiedName, deviceName);

    if ((fp = pcap_open(qualifiedName,
        100,
        PCAP_OPENFLAG_PROMISCUOUS,
        1000,
        NULL,
        errbuf
    )) == NULL)
    {
        fprintf(stderr,
            "\nUnable to open the adapter. Is not supported by Npcap\n %s", errbuf);
        return NULL;
    }

    return fp;
}

void Send(pcap_t* fp, BYTE* packet, int totalSize) {
    if (pcap_sendpacket(fp, packet, totalSize) != 0)
    {
        fprintf(stderr, "\nError sending the packet: %s\n", pcap_geterr(fp));
        return;
    }
    else
    {
        fprintf(stdout, "\nPacket sent with no warnings.");
    }
}