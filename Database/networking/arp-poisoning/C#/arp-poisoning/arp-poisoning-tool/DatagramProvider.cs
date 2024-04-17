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

            string pattern = @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$";
            if(!Regex.IsMatch(targetAddress, pattern))
            {
                throw new Exception("Invalid format. Please provide a mac address with the following notation: ff:ff:ff:ff:ff:ff or ff-ff-ff-ff-ff-ff");
            }
            if (!Regex.IsMatch(sourceAddress, pattern))
            {
                throw new Exception("Invalid format. Please provide a mac address with the following notation: ff:ff:ff:ff:ff:ff or ff-ff-ff-ff-ff-ff");
            }

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
            return new ArpReply();
        }
    }

    internal enum EthernetProtocolType : ushort
    {
        Unknown = 0x0,
        ARP = 0x0608,
    }
}
