#include "utilities.hpp"

USHORT Swap(USHORT value) 
{
    return (USHORT)
    (
        (0x00FFu) & (value >> 8) |
        (0xFF00u) & (value << 8)
    );
}