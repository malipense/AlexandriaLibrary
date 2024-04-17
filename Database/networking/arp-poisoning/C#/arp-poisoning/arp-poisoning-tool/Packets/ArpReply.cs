using arp_poisoning_tool.DataStructures;
using System.Runtime.InteropServices;

namespace arp_poisoning_tool.Packets
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct ArpReply
    {
        public ArpReply(MacAddress senderAddress, uint senderIp, MacAddress targetAddress, uint targetIp)
        {
            SenderAddress = senderAddress;
            SenderIp = senderIp;
            TargetAddress = targetAddress;
            TargetIp = targetIp;
        }
        public ushort HardwareType = 0x01;
        public ushort ProtocolType = 0x08;
        public byte HardwareSize = 0x6;
        public byte ProtocolSize = 0x4;
        public ushort Opcode = 0x2;
        public MacAddress SenderAddress;
        public uint SenderIp;
        public MacAddress TargetAddress;
        public uint TargetIp;
    }
}
