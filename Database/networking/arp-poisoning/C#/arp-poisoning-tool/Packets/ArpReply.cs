using arp_poisoning_tool.DataStructures;
using System.Runtime.InteropServices;

namespace arp_poisoning_tool.Packets
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct ArpReply
    {
        public ArpReply(MacAddress senderAddress, Ipv4Address senderIp, MacAddress targetAddress, Ipv4Address targetIp)
        {
            SenderAddress = senderAddress;
            SenderIp = senderIp;
            TargetAddress = targetAddress;
            TargetIp = targetIp;
        }
        public ushort HardwareType = 0x0100;
        public ushort ProtocolType = 0x08;
        public byte HardwareSize = 0x6;
        public byte ProtocolSize = 0x4;
        public ushort Opcode = 0x0200;
        public MacAddress SenderAddress;
        public Ipv4Address SenderIp;
        public MacAddress TargetAddress;
        public Ipv4Address TargetIp;
    }
}
