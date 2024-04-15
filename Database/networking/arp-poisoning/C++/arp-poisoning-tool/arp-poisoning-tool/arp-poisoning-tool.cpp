// arp-poisoning-tool.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <iostream>
#include "pcap.h"

int main()
{
    SetDllDirectory(L"C:/Windows/System32/Npcap/");

    pcap_t* fp;
    char errbuf[PCAP_ERRBUF_SIZE];
    u_char packet[100];
    int i;
    const char* name = "rpcap://\\Device\\NPF_Loopback";

    /* Open the output device */
    if ((fp = pcap_open(name, // name of the device
        100, // portion of the packet to capture
        PCAP_OPENFLAG_PROMISCUOUS, // promiscuous mode
        1000, // read timeout
        NULL, // authentication on the remote machine
        errbuf // error buffer
    )) == NULL)
    {
        fprintf(stderr,
            "\nUnable to open the adapter. Is not supported by Npcap\n %s", errbuf);
        return 0;
    }
    
    /* Supposing to be on ethernet, set mac destination to 1:1:1:1:1:1 */
    packet[0] = 1;
    packet[1] = 1;
    packet[2] = 1;
    packet[3] = 1;
    packet[4] = 1;
    packet[5] = 1;

    /* set mac source to 2:2:2:2:2:2 */
    packet[6] = 2;
    packet[7] = 2;
    packet[8] = 2;
    packet[9] = 2;
    packet[10] = 2;
    packet[11] = 2;

    /* Fill the rest of the packet */
    for (i = 12; i < 100; i++)
    {
        packet[i] = (u_char)i;
    }

    /* Send down the packet */
    if (pcap_sendpacket(fp, packet, 100 /* size */) != 0)
    {
        fprintf(stderr, "\nError sending the packet: %s\n", pcap_geterr(fp));
        return 0;
    }

    return 1;
}