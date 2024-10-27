using System;
using System.Net;
using System.Net.Sockets;
using UtilityLibrary;

namespace server;

public class ServerMain
{
	public static string FilesDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CServerSaves");
	private static List<Connection> connections = new List<Connection>();
	
	/**
	 * Method that keeps accepting sockets and gives them their own thread
	 * Checks if the directory for uploaded files exists first, if not it will create it.
	 */
	public static void Main(string[] args)
	{
		EnsureDirectoryExists(ServerMain.FilesDirectory);
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
	
	/**
	 * Creates the save directory would it not exist.
	 */
	private static void EnsureDirectoryExists(string path)
	{
		Directory.CreateDirectory(path);
	}

	public static void RemoveConnections(Connection connection)
	{
		connections.Remove(connection);
	}
}