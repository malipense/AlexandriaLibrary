using System;

namespace arp_poisoning_tool.Packets
{
    internal struct Loopback
    {
        public Loopback(uint family)
        {
            Family = family;
        }
        public uint Family;
    }
}
