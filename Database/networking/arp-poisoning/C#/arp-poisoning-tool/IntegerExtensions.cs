using System;

namespace arp_poisoning_tool
{
    internal static class IntegerExtensions
    {
        public static short Swap(this short value)
        {
            return (short)((ushort)value).Swap();
        }

        public static ushort Swap(this ushort value)
        {
            return (ushort)
            (
                (0x00FFu) & (value >> 8) |
                (0xFF00u) & (value << 8)
            );
        }

        public static int Swap(this int value)
        {
            return (int)((uint)value).Swap();
        }

        public static uint Swap(this uint value)
        {
            return
                (0x000000FFu) & (value >> 24) |
                (0x0000FF00u) & (value >> 8) |
                (0x00FF0000u) & (value << 8) |
                (0xFF000000u) & (value << 24);
        }
    }
}
