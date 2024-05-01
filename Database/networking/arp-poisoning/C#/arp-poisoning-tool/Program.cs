/*
    As the Win32 network libraries are limited to the creation of layer 3 packets, we are required to use Ncap
    to overcome that: https://npcap.com/guide/
    https://www.quora.com/Why-is-the-ARP-table-not-updating-itself-when-a-new-device-enters-the-LAN-I-have-to-ping-to-that-IP-in-order-to-find-its-entry-in-ARP-table-What-is-the-alternative-to-find-the-IP-address-of-a-specific-MAC-address#:~:text=Your%20ARP%20table%20is%20only,your%20ARP%20table%20is%20updated.
    https://www.practicalnetworking.net/series/arp/gratuitous-arp/
    https://www.practicalnetworking.net/series/arp/traditional-arp/
*/

using arp_poisoning_tool;
using arp_poisoning_tool.Packets;

Console.WriteLine(TermsAndUsage());

string target = args[0];
if (target == "--interactive")
{
    InteractiveMode();
}

Console.ReadKey();

void InteractiveMode()
{
    bool interfaceSelected = false;
    bool validPacket = false;

    PcapModule module = new PcapModule();
    module.Load();

    Console.WriteLine("\nSelect an interface\n");
    var interfaces = module.ListNetworkDevices();

    for (int i = 0; i < interfaces.Count; i++)
    {
        Console.WriteLine($"{(i + 1)} -- {interfaces[i]}");
    }

    var index = (int)char.GetNumericValue(Console.ReadKey().KeyChar) - 1;
    var nInterface = interfaces[index];
    interfaceSelected = true;

    Console.Clear();
    Console.WriteLine($"\nSelected: {nInterface}");
    
    if (interfaceSelected)
    {
        Console.WriteLine("1 - Ethernet 2 - IPv4 3 - ARP Reply 4 - Send");
        var choice = Console.ReadKey().KeyChar;
        byte[] packetBytes = new byte[256];
        int currentIndex = 0;

        while (choice != '4')
        {
            switch (choice)
            {
                case '1':
                    Console.WriteLine("\nSource MacAddress");
                    string source = Console.ReadLine();

                    Console.WriteLine("Destination MacAddress");
                    string destination = Console.ReadLine();

                    Console.WriteLine("Type (in Hex notation, ex: ARP = 0x0608)");
                    string type = Console.ReadLine();

                    Ethernet ethernet = DatagramProvider.BuildEthernetPacket(destination, source, (EthernetProtocolType)Convert.ToUInt16(type, 16));
                    byte[] ethernetB = BinaryConverter.GetBytes(ethernet);

                    Buffer.BlockCopy(ethernetB, 0, packetBytes, 0, ethernetB.Length);
                    currentIndex = ethernetB.Length;

                    Console.WriteLine("Ethernet packet created.");
                    validPacket = true;
                    break;
                case '2':
                    break;
                case '3':
                    Console.WriteLine("\nSender MacAddress");
                    string senderAddress = Console.ReadLine();

                    Console.WriteLine("Sender IP Address");
                    string senderIp = Console.ReadLine();

                    Console.WriteLine("Target MacAddress");
                    string targetAddress = Console.ReadLine();

                    Console.WriteLine("Target IP Address");
                    string targetIp = Console.ReadLine();

                    ArpReply reply = DatagramProvider.BuildArpReplyPacket(senderAddress, senderIp, targetAddress, targetIp);
                    byte[] replyB = BinaryConverter.GetBytes(reply);

                    Buffer.BlockCopy(replyB, 0, packetBytes, currentIndex, replyB.Length);
                    currentIndex += replyB.Length;
                    break;
            }
            Console.WriteLine("1 - Ethernet 2 - IPv4 3 - ARP Reply 4 - Send");
            choice = Console.ReadKey().KeyChar;
        }

        if (choice == '4')
        {
            if (validPacket)
            {
                Console.WriteLine($"\nOpening device: {nInterface}");

                Array.Resize(ref packetBytes, currentIndex);
                IntPtr p = module.Open(nInterface.Name);

                module.Send(p, packetBytes, packetBytes.Length);
            }
            else
                throw new Exception("Invalid packet structure");
        }
    }
}
string TermsAndUsage()
{
    return $"This tool was created for education purposes, do not use it improperly.This is part of a major repository available @:\nhttps://github.com/malipense/AlexandriaLibrary\n"
        + "Standalone available @: https://github.com/malipense/netpforgery";
}