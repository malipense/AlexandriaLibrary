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

    LOOPBACK loopback;
    PIPV4 ipv4 = CreateIpV4Packet(0, "192.168.1.10", "192.168.1.10", 0, 64, NULL, 0);

    loopback.Family = 2;

    int loopbackSize = sizeof(loopback);
    BYTE* pLoopback = reinterpret_cast<BYTE*>(&loopback);

    int ipv4Size = sizeof(*ipv4);
    BYTE* pIpv4 = reinterpret_cast<BYTE*>(ipv4);

    const int totalSize = loopbackSize + ipv4Size;
    BYTE* packet = new BYTE[totalSize];

    for (int i = 0; i < loopbackSize; i++)
    {
        packet[i] = *pLoopback;
        pLoopback++;
    }

    for (int i = loopbackSize; i < totalSize; i++)
    {
        packet[i] = *pIpv4;
        pIpv4++;
    }

    Send(fp, packet, totalSize);

    delete[] packet;
    free(ipv4);

    return 1;
}