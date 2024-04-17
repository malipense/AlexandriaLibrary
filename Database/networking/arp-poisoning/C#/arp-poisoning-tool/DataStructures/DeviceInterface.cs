using System.Runtime.InteropServices;

namespace arp_poisoning_tool.DataStructures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct DeviceInterface
    {
        public IntPtr Next;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Description;
    }
}
