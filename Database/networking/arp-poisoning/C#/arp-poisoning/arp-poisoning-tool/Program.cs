using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
//string target = args[0];

NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces().Where(n => n.OperationalStatus == OperationalStatus.Up)
    .ToArray();

PhysicalAddress mac = nics[1].GetPhysicalAddress();

using(Socket socket = new Socket(SocketType.Rdm, ProtocolType.Unspecified))
{

}

Console.ReadKey();