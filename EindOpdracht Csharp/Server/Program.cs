using System;
using System.Net;
using System.Net.Sockets;
using UtilityLibrary;

namespace server;

public class Program
{
	private static List<Connection> connections = new List<Connection>();
	public static void Main(string[] args)
	{
		TcpListener listener = new TcpListener(IPAddress.Loopback, 666);
		listener.Start();
		while (true)
		{
			TcpClient client = listener.AcceptTcpClient();
			connections.Add(new Connection(client));
		}
	}
}