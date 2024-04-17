using System.Runtime.InteropServices;

namespace arp_poisoning_tool
{
    internal static class BinaryConverter
    {
        public static byte[] GetBytes<T>(T obj) where T : struct
        {
            int size = Marshal.SizeOf(obj);
            IntPtr p = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, p, false);

            byte[] buffer = new byte[size];
            Marshal.Copy(p, buffer, 0, size);
            Marshal.FreeHGlobal(p);
            return buffer;
        }
    }
}
