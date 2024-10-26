using System.Net;
using System.Net.Sockets;
using System.Text;

public class ServerTest
{
	public static void Main(string[] args)
	{
		try
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(IPAddress.Parse("127.0.0.1"), 666);
			socket.Send(Encoding.ASCII.GetBytes("{UPLOAD START}\n"));
			string fileName = "TestFile.txt";
			socket.Send(Encoding.ASCII.GetBytes($"{fileName}\n"));
			socket.SendFile("C:\\Users\\rikva\\Documents\\GitKraken codes\\EindOpdracht-C-\\EindOpdracht Csharp\\ServerTest\\TestFile.txt");
			socket.Send(Encoding.ASCII.GetBytes("\n{UPLOAD DONE}"));

			Console.WriteLine("File sent to server");
			socket.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine($"Threw exception: {e}");
		}
	}
}