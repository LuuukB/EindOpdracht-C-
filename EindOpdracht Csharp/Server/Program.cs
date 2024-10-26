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
		try
		{
			TcpListener listener = new TcpListener(IPAddress.Loopback, 666);
			listener.Start();
			while (true)
			{
				Socket client = listener.AcceptSocket();
				connections.Add(new Connection(client));
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"Server crashed with error\n{e}");
		}
		

	}

	public static void RemoveConnections(Connection connection)
	{
		connections.Remove(connection);
	}
}