/*
    As the Win32 network libraries are limited to the creation of layer 3 packets, we are required to use Ncap
    to overcome that: https://npcap.com/guide/
*/

using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

[DllImport("kernel32.dll", SetLastError = true)]
extern static IntPtr LoadLibraryEx(string path, IntPtr file, uint flags);

[DllImport("kernel32.dll", SetLastError = true)]
extern static IntPtr GetProcAddress(IntPtr hModule, string name);

Console.WriteLine("Hello, World!");
//string target = args[0];

uint alternativePath = 0x00000008;
string defaultInstallationPath = "C:\\WINDOWS\\system32\\Npcap\\wpcap.dll";

IntPtr module = LoadLibraryEx(defaultInstallationPath, IntPtr.Zero, alternativePath);
if (module == IntPtr.Zero)
    throw new Exception("Failed to load wpcap.dll, make sure you have Npcap installed.");

var pPcapOpen = ExportFunction<PcapOpen>(module, "pcap_open");
var pPcapSendPacket = ExportFunction<PcapSendPacket>(module, "pcap_sendpacket");
var pPcapFindAllDev = ExportFunction<PcapFindAllDevsEx>(module, "pcap_findalldevs_ex");
var pPcapGetError = ExportFunction<PcapGetError>(module, "pcap_geterr");

IEnumerable<NetworkInterface> cards = NetworkInterface.GetAllNetworkInterfaces().Where(n => n.OperationalStatus == OperationalStatus.Up);

char[] errbuffer = new char[256];

byte[] packet = new byte[] { 0x00, 0x01, 0x08, 0x00, 0x06, 0x04, 0x00, 0x01, 0xf4, 0x54, 0x20, 0x57, 0xe7, 0x50, 0xc0, 0xa8, 0x0f, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc0, 0xa8, 0x0f, 0xf2 };

DeviceInterface device;

if (pPcapFindAllDev("rpcap://", null, ref device, ref errbuffer) == -1)
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

IntPtr fp = pPcapOpen("\\Device\\NPF_Loopback", 100, 1, 1000, null, ref errbuffer);
if (fp == IntPtr.Zero)
{
    Console.WriteLine("Failed to open device.");
    return;
}

if(pPcapSendPacket(fp, packet, 28) != 0)
{
    IntPtr error = pPcapGetError(fp);
    Console.WriteLine($"Error sending packet: {Marshal.PtrToStringAnsi(error)}");
}

Console.ReadKey();

Delegate? ExportDelegate<T>(IntPtr module, string name)
{
    IntPtr pointer = GetProcAddress(module, name);
    return pointer == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer(pointer, typeof(T));
}

T ExportFunction<T>(IntPtr module, string name) where T : class
{
    return (T)((object)ExportDelegate<T>(module, name));
}

delegate IntPtr PcapOpen(string source, int snapLen, int flags, int readTimeout, object auth, ref char[] errbuf);
delegate int PcapSendPacket(IntPtr fd, byte[] packet, int length);
delegate int PcapFindAllDevsEx(string source, object auth, ref DeviceInterface intf, ref char[] errbuf);
delegate IntPtr PcapGetError(IntPtr fp);

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct DeviceInterface
{
    public IntPtr Next;
    [MarshalAs(UnmanagedType.LPStr)]
    public string Name;
    [MarshalAs(UnmanagedType.LPStr)]
    public string Description;
}