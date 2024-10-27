using System.Net;
using System.Net.Sockets;
using System.Text;
using UtilityLibrary;

public class ServerTest
{
	public static void Main(string[] args)
	{
		try
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(IPAddress.Parse("127.0.0.1"), 666);
			StreamWriter writer = new StreamWriter(new NetworkStream(socket));
			
			Utils.SendData(writer, "{UPLOAD START} TestFile.txt");
			socket.SendFile("C:\\Users\\rikva\\Documents\\GitKraken codes\\EindOpdracht-C-\\EindOpdracht Csharp\\ServerTest\\TestFile.txt");
			Utils.SendData(writer, "\n{UPLOAD DONE}");

			Console.WriteLine("File sent to server");
			socket.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine($"Threw exception: {e}");
		}
	}
}