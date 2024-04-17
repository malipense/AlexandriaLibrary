using arp_poisoning_tool.DataStructures;
using arp_poisoning_tool.Packets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace arp_poisoning_tool
{
    internal static class DatagramProvider
    {
        internal static Ethernet BuildEthernetPacket(string targetAddress, string sourceAddress, EthernetProtocolType type)
        {
            ValidatePhysicalAddress(targetAddress);
            ValidatePhysicalAddress(sourceAddress);

            return new Ethernet((ushort)type, 
                new MacAddress(ToByteArray(targetAddress.Split(':', '-'), 16)), 
                new MacAddress(ToByteArray(sourceAddress.Split(':', '-'), 16))
                );
        }

        internal static ArpReply BuildArpReplyPacket(string senderAddress, string senderIp, string targetAddress, string targetIp)
        {
            ValidatePhysicalAddress(targetAddress);
            ValidatePhysicalAddress(senderAddress);
            ValidateIpAddress(targetIp);
            ValidateIpAddress(senderIp);

            return new ArpReply(new MacAddress(ToByteArray(senderAddress.Split(':', '-'), 16)),
                new Ipv4Address(ToByteArray(senderIp.Split('.'))),
                new MacAddress(ToByteArray(targetAddress.Split(':', '-'), 16)),
                new Ipv4Address(ToByteArray(targetIp.Split('.')))
                );
        }

        internal static Ipv4 BuildIpv4Packet(InternetProtocolType protocol, string sourceIp, string destinationIp, int identification = 0, int ttl = 128, byte[] payload = null)
        {
            if (ttl > 255 || ttl < 1)
                throw new Exception("TTL - Time To Live should be a number between 1 and 255");

            if (identification > 65535)
                throw new Exception("Identification - 0 for null (new packet); 1-65535 for an existing value");

            int totalLength = Marshal.SizeOf<Ipv4>();
            if (payload != null)
                totalLength = totalLength + payload.Length;
            
            ushort ident = 0;
            if (identification == 0)
            {
                Random rnd = new Random();
                ident = (ushort)rnd.Next(65535);
            }
            else
                ident = (ushort)identification;

            ValidateIpAddress(sourceIp);
            ValidateIpAddress(destinationIp);

            
            Ipv4 packet = new Ipv4(ident.Swap(), (byte)ttl, (byte)protocol, 0,
                new Ipv4Address(ToByteArray(sourceIp.Split('.'))),
                new Ipv4Address(ToByteArray(destinationIp.Split('.'))),
                ((ushort)totalLength).Swap()
                );

            return packet;
        }

        internal static Loopback BuildLoopbackPacket(uint family = 2)
        {
            return new Loopback(family);
        }

        private static byte[] ToByteArray(string[] strings, int nBase = 10)
        {
            int size = strings.Length;

            byte[] bytes = new byte[size];
            for (int i = 0; i < size ; i++)
            {
                bytes[i] = Convert.ToByte(strings[i], nBase);
            }
            return bytes;
        }

        private static void ValidatePhysicalAddress(string physicalAddress)
        {
            string pattern = @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$";
            if (!Regex.IsMatch(physicalAddress, pattern))
            {
                throw new Exception("Invalid format. Please provide a mac address with the following notation: ff:ff:ff:ff:ff:ff or ff-ff-ff-ff-ff-ff");
            }
        }

        private static void ValidateIpAddress(string ipAddress)
        {
            string ipPattern = @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$";
            if (!Regex.IsMatch(ipAddress, ipPattern))
            {
                throw new Exception("Invalid format. Please provide a ip address with the following notation: 10.10.10.10");
            }
        }
    }

    internal enum EthernetProtocolType : ushort
    {
        Unknown = 0x0,
        ARP = 0x0608,
    }

    internal enum InternetProtocolType : byte
    {
        TCP = 0x06,
        UDP = 0x11
    }
}
