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

NetworkInterface wifi = NetworkInterface.GetAllNetworkInterfaces().Where(n => n.OperationalStatus == OperationalStatus.Up)
    .ToArray()[1];

char[] errbuffer = new char[256];
var fd = pPcapOpen(wifi.Name, 100, 0x00000001, 1000, null, ref errbuffer);

byte[] packet = new byte[] { 0x00, 0x01, 0x08, 0x00, 0x06, 0x04, 0x00, 0x01, 0xf4, 0x54, 0x20, 0x57, 0xe7, 0x50, 0xc0, 0xa8, 0x0f, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc0, 0xa8, 0x0f, 0xf2 };

var re = pPcapSendPacket(fd, packet, 28);

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

delegate Pcap PcapOpen(string source, int snapLen, int flags, int readTimeout, object auth, ref char[] errbuf);
delegate int PcapSendPacket(Pcap fd, byte[] packet, int length);

struct pcap_sf
{
    IntPtr rfile;
    int swapped;
    int hdrsize;
    int version_major;
    int version_minor;
    IntPtr _base;
};

struct pcap_stat
{
    uint ps_recv;
    uint ps_drop;
    uint ps_ifdrop;
    uint bs_capt;      
};

struct pcap_md
{
    pcap_stat stat;
	int use_bpf;        /* using kernel filter */
    ulong TotPkts; /* can't oflow for 79 hrs on ether */
    ulong TotAccepted; /* count accepted by filter */
    ulong TotDrops;    /* count of dropped packets */
    long TotMissed; /* missed by i/f during this run */
    long OrigMissed;    /* missed by i/f before this run */
};

struct Pcap
{
    int fd;
    int snapshot;
    int linktype;
    int tzoff;      /* timezone offset */
    int offset;     /* offset for proper alignment */

    pcap_sf sf;
    pcap_md md;

    /*
     * Read buffer.
     */
    int bufsize;
    UIntPtr buffer;
    UIntPtr bp;
    int cc;

    /*
     * Place holder for pcap_next().
     */
    UIntPtr pkt;
}