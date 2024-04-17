/*
    As the Win32 network libraries are limited to the creation of layer 3 packets, we are required to use Ncap
    to overcome that: https://npcap.com/guide/
    https://www.quora.com/Why-is-the-ARP-table-not-updating-itself-when-a-new-device-enters-the-LAN-I-have-to-ping-to-that-IP-in-order-to-find-its-entry-in-ARP-table-What-is-the-alternative-to-find-the-IP-address-of-a-specific-MAC-address#:~:text=Your%20ARP%20table%20is%20only,your%20ARP%20table%20is%20updated.
*/

using arp_poisoning_tool;

Console.WriteLine(TermsAndUsage());
//string target = args[0];

byte[] loopback = BinaryConverter.GetBytes(DatagramProvider.BuildLoopbackPacket());
//byte[] ethernet = BinaryConverter.GetBytes(DatagramProvider.BuildEthernetPacket("78-2b-46-e9-74-65", "D4-6A-6A-F0-3B-0F", EthernetProtocolType.ARP));
//byte[] arpReply = BinaryConverter.GetBytes(DatagramProvider.BuildArpReplyPacket("00-00-00-00-00-00", "192.168.15.179", "78-2b-46-e9-74-65", "192.168.15.10"));
byte[] ip = BinaryConverter.GetBytes(DatagramProvider.BuildIpv4Packet(InternetProtocolType.UDP, "192.168.15.9", "192.168.15.10"));

int size = loopback.Length + ip.Length;
byte[] packet = new byte[size];

Buffer.BlockCopy(loopback, 0, packet, 0, loopback.Length);
Buffer.BlockCopy(ip, 0, packet, loopback.Length, ip.Length);

Console.ReadKey();

PcapModule pcapModule = new PcapModule();
pcapModule.Load();

var ptr = pcapModule.Open("\\Device\\NPF_Loopback");

pcapModule.Send(ptr, packet, size);

string TermsAndUsage()
{
    return $"This tool was created for education purposes, do not use it improperly.This is part of a major repository available @:\nhttps://github.com/malipense/AlexandriaLibrary\n"
        + "Usage: (TODO)";
}