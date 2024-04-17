using arp_poisoning_tool.DataStructures;
using System.Runtime.InteropServices;
using System.Text;

namespace arp_poisoning_tool
{
    #pragma warning disable CS8618
    internal class PcapModule
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        extern static IntPtr LoadLibraryEx(string path, IntPtr file, uint flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        extern static IntPtr GetProcAddress(IntPtr hModule, string name);

        uint alternativePath = 0x00000008;
        string defaultInstallationPath = "C:\\WINDOWS\\system32\\Npcap\\wpcap.dll";
        IntPtr _module = IntPtr.Zero;

        delegate IntPtr PcapOpen(string source, int snapLen, int flags, int readTimeout, object auth, ref byte[] errbuf);
        PcapOpen pPcapOpen;
        
        delegate int PcapSendPacket(IntPtr fd, byte[] packet, int length);
        PcapSendPacket pPcapSendPacket;

        delegate int PcapFindAllDevsEx(string source, object auth, ref DeviceInterface intf, ref char[] errbuf);
        PcapFindAllDevsEx pPcapFindAllDev;

        delegate IntPtr PcapGetError(IntPtr fp);
        PcapGetError pPcapGetError;

        private bool _loaded = false;
        internal void Load()
        {
            IntPtr module = LoadLibraryEx(defaultInstallationPath, IntPtr.Zero, alternativePath);
            if (module == IntPtr.Zero)
                throw new Exception("Failed to load wpcap.dll, make sure you have Npcap installed.");

            _module = module;

            try
            {
                pPcapFindAllDev = ExportFunction<PcapFindAllDevsEx>(_module, "pcap_findalldevs_ex");
                pPcapOpen = ExportFunction<PcapOpen>(module, "pcap_open");
                pPcapSendPacket = ExportFunction<PcapSendPacket>(module, "pcap_sendpacket");
                pPcapGetError = ExportFunction<PcapGetError>(module, "pcap_geterr");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            };

            _loaded = true;
        }
        internal void ListNetworkDevices()
        {
            if(!_loaded)
            {
                throw new Exception("Module isn't loaded. Make sure you called the Load() method before any other.");
            }
            DeviceInterface device = new();
            char[] errorBuffer = new char[256];

            if (pPcapFindAllDev("rpcap://", null, ref device, ref errorBuffer) == -1)
            {
                Console.WriteLine("Something went wrong.");
                return;
            }

            while (device.Next != IntPtr.Zero)
            {
                if (device.Name != null && device.Description != null)
                    Console.WriteLine($"Name: {device.Name}, Desc: {device.Description}");
                device = Marshal.PtrToStructure<DeviceInterface>(device.Next);
            }
        }

        internal IntPtr Open(string deviceName)
        {
            if (!_loaded)
            {
                throw new Exception("Module isn't loaded. Make sure you called the Load() method before any other.");
            }

            byte[] errorBuffer = new byte[256];

            string qualifiedName = "rpcap://" + deviceName;
            IntPtr fp = pPcapOpen(qualifiedName, 100, 1, 1000, null, ref errorBuffer);
            if (fp == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to open device: {Encoding.ASCII.GetString(errorBuffer)}");
                return IntPtr.Zero;
            }

            return fp;
        }
        internal void Send(IntPtr fp, byte[] packet, int size)
        {
            if (!_loaded)
            {
                throw new Exception("Module isn't loaded. Make sure you called the Load() method before any other.");
            }

            if (pPcapSendPacket(fp, packet, size) != 0)
            {
                IntPtr error = pPcapGetError(fp);
                Console.WriteLine($"Warning! An error might have occurred, you can use a network capture tool to see exactly how the packet looks for the interface adapter: {Marshal.PtrToStringAnsi(error)}");
            }

            Console.WriteLine("Packet sent with no warnings.");
        }

        Delegate? ExportDelegate<T>(IntPtr module, string name)
        {
            IntPtr pointer = GetProcAddress(module, name);
            return pointer == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer(pointer, typeof(T));
        }

        T ExportFunction<T>(IntPtr module, string name) where T : class
        {
            return (T)((object)ExportDelegate<T>(module, name));
        }
    }
}
