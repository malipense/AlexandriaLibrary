using arp_poisoning_tool.DataStructures;
using System.Runtime.InteropServices;

namespace arp_poisoning_tool.Packets
{
    [StructLayout(LayoutKind.Sequential, Size = 14, Pack = 2)]
    internal struct Ethernet
    {
        public Ethernet(ushort type, MacAddress target, MacAddress source)
        {
            Type = type;
            TargetAddress = target;
            SourceAddress = source;
        }
        
        public MacAddress TargetAddress;
        public MacAddress SourceAddress;
        public ushort Type;
    }
}
