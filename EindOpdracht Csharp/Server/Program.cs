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
			
		}
		catch (Exception e)
		{
			
		}
		
		TcpListener listener = new TcpListener(IPAddress.Loopback, 666);
		listener.Start();
		while (true)
		{
			Socket client = listener.AcceptSocket();
			connections.Add(new Connection(client));
		}
	}

	public static void RemoveConnections(Connection connection)
	{
		connections.Remove(connection);
	}
}