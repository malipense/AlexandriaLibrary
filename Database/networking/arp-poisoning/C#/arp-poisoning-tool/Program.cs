/*
    As the Win32 network libraries are limited to the creation of layer 3 packets, we are required to use Ncap
    to overcome that: https://npcap.com/guide/
    https://www.quora.com/Why-is-the-ARP-table-not-updating-itself-when-a-new-device-enters-the-LAN-I-have-to-ping-to-that-IP-in-order-to-find-its-entry-in-ARP-table-What-is-the-alternative-to-find-the-IP-address-of-a-specific-MAC-address#:~:text=Your%20ARP%20table%20is%20only,your%20ARP%20table%20is%20updated.
*/

using arp_poisoning_tool;

Console.WriteLine(TermsAndUsage());

string target = args[0];
if (target == "--interactive")
{
    InteractiveMode();
}



Console.ReadKey();

void InteractiveMode()
{
    PcapModule pcapModule = new PcapModule();
    pcapModule.Load();

    Console.WriteLine("\nSelect an interface\n");
    var interfaces = pcapModule.ListNetworkDevices();

    for (int i = 0; i < interfaces.Count; i++)
    {
        Console.WriteLine($"{(i + 1)} -- {interfaces[i]}");
    }

    while (true)
    {
        var index = (int)char.GetNumericValue(Console.ReadKey().KeyChar) - 1;
        var selectedInterface = interfaces[index];

        Console.WriteLine($"\nSelected: {selectedInterface}");

        Console.WriteLine("\nPackages: \n");
        Console.WriteLine("Ethernet(type, sourcemac, destmac)");
        var cmd = Console.ReadLine();

        int size = 0;
        byte[] packet = new byte[size];

        if (cmd.StartsWith("Ethernet"))
        {
            var args = cmd.Split('(', ')')[1].Split(',');
            byte[] ethernet = BinaryConverter.GetBytes(DatagramProvider.BuildEthernetPacket(args[2].Trim(), args[1].Trim(), (EthernetProtocolType)ushort.Parse(args[0])));
            size += ethernet.Length;
        }
        if (cmd == "Send")
        {
            
        }
        else
        {
            byte[] ip = BinaryConverter.GetBytes(DatagramProvider.BuildIpv4Packet(InternetProtocolType.UDP, "192.168.15.9", "192.168.15.10"));

            Buffer.BlockCopy(ip, 0, packet, 0, ip.Length);
        }
    }
}
string TermsAndUsage()
{
    return $"This tool was created for education purposes, do not use it improperly.This is part of a major repository available @:\nhttps://github.com/malipense/AlexandriaLibrary\n"
        + "Usage: (TODO)";
}