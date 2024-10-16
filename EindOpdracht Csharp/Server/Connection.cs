using System.Net.Sockets;
using System.Text;
using UtilityLibrary;

namespace server;

public class Connection
{
	private StringBuilder stringBuilder;
	private StreamWriter writer;
	private TextReader reader;
	
	public Connection(TcpClient client)
	{
		NetworkStream stream = client.GetStream();
		writer = new StreamWriter(stream);
		reader = new StreamReader(new BufferedStream(stream));
		new Thread(HandleReceivedData).Start();
	}

	private void HandleReceivedData()
	{
		//receiving thread should always be running
		while (true)
		{
			try
			{
				string? line = reader.ReadLine();
				HandleLine(line);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Reading failed with exception \n{e}");
				return;
			}
			finally
			{
				//todo remove itself from connections list
			}
			
		}
	}
	private void HandleLine(string? line)
	{
		if (line == null) return;
		Console.WriteLine($"Server read line: {line}");
				
		switch (line)
		{
			case "{UPLOAD START}":
				stringBuilder = new StringBuilder();
				break;
			case "{UPLOAD DONE}":
				Console.WriteLine(stringBuilder.ToString());
				break;
			default:
				stringBuilder.AppendLine(line);
				break;
		}
	}

	public void SendData(string data)
	{
		writer.WriteLine(data);
		writer.Flush();
	}
	
}