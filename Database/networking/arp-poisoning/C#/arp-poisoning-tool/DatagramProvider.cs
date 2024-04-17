using arp_poisoning_tool.DataStructures;
using arp_poisoning_tool.Packets;
using System.Text.RegularExpressions;

namespace arp_poisoning_tool
{
    internal static class DatagramProvider
    {
        internal static Ethernet BuildEthernetPacket(string targetAddress, string sourceAddress, EthernetProtocolType type)
        {
            List<byte> targetHexList = new List<byte>(6);
            List<byte> sourceHexList = new List<byte>(6);

            ValidatePhysicalAddress(targetAddress);
            ValidatePhysicalAddress(sourceAddress);

            foreach (var value in (targetAddress.Split(':', '-')))
            {
                targetHexList.Add(Convert.ToByte(value, 16));
            }
            foreach (var value in (sourceAddress.Split(':', '-')))
            {
                sourceHexList.Add(Convert.ToByte(value, 16));
            }

            return new Ethernet((ushort)type, new MacAddress(targetHexList.ToArray()), new MacAddress(sourceHexList.ToArray()));
        }

        internal static ArpReply BuildArpReplyPacket(string senderAddress, string senderIp, string targetAddress, string targetIp)
        {
            List<byte> targetMacList = new List<byte>(6);
            List<byte> senderMacList = new List<byte>(6);

            List<byte> targetIpList = new List<byte>(4);
            List<byte> senderIpList = new List<byte>(4);

            ValidatePhysicalAddress(targetAddress);
            ValidatePhysicalAddress(senderAddress);
            ValidadeIpAddress(targetIp);
            ValidadeIpAddress(senderIp);
            
            foreach (var value in (targetAddress.Split(':', '-')))
            {
                targetMacList.Add(Convert.ToByte(value, 16));
            }
            foreach (var value in (senderAddress.Split(':', '-')))
            {
                senderMacList.Add(Convert.ToByte(value, 16));
            }

            foreach (var value in (targetIp.Split('.')))
            {
                targetIpList.Add(Convert.ToByte(value, 10));
            }
            foreach (var value in (senderIp.Split('.')))
            {
                senderIpList.Add(Convert.ToByte(value, 10));
            }

            return new ArpReply(new MacAddress(senderMacList.ToArray()),
                new Ipv4Address(senderIpList.ToArray()),
                new MacAddress(targetMacList.ToArray()),
                new Ipv4Address(targetIpList.ToArray()));
        }

        internal static Ipv4 BuildIpv4Packet()
        {
            return new Ipv4();
        }

        private static void ValidatePhysicalAddress(string physicalAddress)
        {
            string pattern = @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$";
            if (!Regex.IsMatch(physicalAddress, pattern))
            {
                throw new Exception("Invalid format. Please provide a mac address with the following notation: ff:ff:ff:ff:ff:ff or ff-ff-ff-ff-ff-ff");
            }
        }

        private static void ValidadeIpAddress(string ipAddress)
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
}
