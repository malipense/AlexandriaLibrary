using System;

namespace arp_poisoning_tool.DataStructures
{
    internal struct MacAddress
    {
        public MacAddress(byte[] bytes)
        {
            B1 = bytes[0];
            B2 = bytes[1];
            B3 = bytes[2];
            B4 = bytes[3];
            B5 = bytes[4];
            B6 = bytes[5];
        }

        public MacAddress(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
        {
            B1 = b1;
            B2 = b2;
            B3 = b3;
            B4 = b4;
            B5 = b5;
            B6 = b6;
        }

        public byte B1;
        public byte B2;
        public byte B3;
        public byte B4;
        public byte B5;
        public byte B6;
    }
}
