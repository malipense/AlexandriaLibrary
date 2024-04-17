using System;

namespace arp_poisoning_tool.DataStructures
{
    internal struct Ipv4Address
    {
        public Ipv4Address(byte b1, byte b2, byte b3, byte b4)
        {
            B1 = b1;
            B2 = b2;
            B3 = b3;
            B4 = b4;
        }

        public Ipv4Address(byte[] bytes)
        {
            B1 = bytes[0];
            B2 = bytes[1];
            B3 = bytes[2];
            B4 = bytes[3];
        }

        public byte B1;
        public byte B2;
        public byte B3;
        public byte B4;
    }
}
