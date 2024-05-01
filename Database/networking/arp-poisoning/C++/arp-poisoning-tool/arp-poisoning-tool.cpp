// arp-poisoning-tool.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <iostream>
#include "pcap-module.hpp"
#include "types.hpp"
#include "utilities.hpp"
#include "datagram-provider.hpp"

int main()
{
    SetDllDirectory(L"C:/Windows/System32/Npcap/");

    const char* name = "\\Device\\NPF_Loopback";
    pcap_t* fp = Open(name);

    PETHERNET ethernet = CreateEthernetPacket(0, "FF-FF-FF-FF-FF-FF", "FF-FF-FF-FF-FF-FF");
    PIPV4 ipv4 = CreateIpV4Packet(0, "192.168.1.10", "192.168.1.10", 0, 64, NULL, 0);

    int ethernetSize = sizeof(ethernet);
    BYTE* pEthernet = reinterpret_cast<BYTE*>(&ethernet);

    int ipv4Size = sizeof(*ipv4);
    BYTE* pIpv4 = reinterpret_cast<BYTE*>(ipv4);

    const int totalSize = ethernetSize + ipv4Size;
    BYTE* packet = new BYTE[totalSize];

    for (int i = 0; i < ethernetSize; i++)
    {
        packet[i] = *pEthernet;
        pEthernet++;
    }

    for (int i = ethernetSize; i < totalSize; i++)
    {
        packet[i] = *pIpv4;
        pIpv4++;
    }

    Send(fp, packet, totalSize);

    delete[] packet;
    free(ipv4);
    free(ethernet);

    return 1;
}