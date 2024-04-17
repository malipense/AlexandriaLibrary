using System;

namespace arp_poisoning_tool.Packets
{
    internal struct IPv4
    {
        public IPv4(ushort totalLength, byte ttl, byte protocol, ushort headerChecksum, uint sourceAddress, uint destinationAddress)
        {
            TotalLength = totalLength;
            TTL = ttl;
            Protocol = protocol;
            HeaderChecksum = 0x0;
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
        }
        public byte Version = 0x45; //Version (0100 - 4) + Header length (0101 (5) - 20 bytes)
        public byte DifferentiatedService = 0x0;
        public ushort TotalLength;
        public ushort Identification = 0x1c8c;
        public byte Flags = 0x0;
        public byte FragmentOffset = 0x0;
        public byte TTL = 0x80; //128 by default
        public byte Protocol;
        public ushort HeaderChecksum;
        public uint SourceAddress;
        public uint DestinationAddress;
    }
}
