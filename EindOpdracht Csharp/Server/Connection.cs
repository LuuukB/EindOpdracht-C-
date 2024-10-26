using System.Net.Sockets;
using System.Text;
using UtilityLibrary;

namespace server;

public class Connection
{
	private readonly string filesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CServerSaves");
	private Socket socket;
	private StringBuilder stringBuilder;
	private StreamWriter writer;
	private TextReader reader;
	
	public Connection(Socket clientSocket)
	{
		socket = clientSocket;
		NetworkStream stream = new NetworkStream(clientSocket);
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
				Program.RemoveConnections(this);
			}
			
		}
	}
	
	private bool isUploading = false;
	
	private void HandleLine(string? line)
	{
		if (line == null) return;
		Console.WriteLine($"Server read line: {line}");
		
		if (isUploading)
		{
			switch (line)
			{
				case "{UPLOAD START}":
					stringBuilder = new StringBuilder();
					break;
				case "{UPLOAD DONE}":
					string totalContent = stringBuilder.ToString();
					int fileNameLength = totalContent.IndexOf('\n');

					//filter out the \n from index of
					string fileName = totalContent.Substring(0, fileNameLength - 1);
					string filePath = Path.Combine(filesDirectory, fileName);

					//filter out the \n from index of
					string content = totalContent.Substring(fileNameLength + 1);

					//test print for console
					Console.WriteLine($"got file name: {fileName}\n" +
					                  $"at: {filePath}\n" +
					                  $"with content: \n\t{content}");

					Utils.CreateNewFile(filePath, content);
					isUploading = false;
					break;
				default:
					stringBuilder.AppendLine(line);
					break;
			}
		}
		else
		{
			if (line.StartsWith("{UPLOAD START}"))
			{
				stringBuilder = new StringBuilder();
				isUploading = true;
			} else if (line.StartsWith("{DOWNLOAD}"))
			{
				string fileToSend = line.Split(' ')[1];
				SendFile(fileToSend);
			} else if (line.StartsWith("{REFRESH}"))
			{
				StringBuilder builder = new StringBuilder("{FILES}:");
				string[] files = Directory.GetFiles(filesDirectory);
				for (int i = 0; i < files.Length; i++)
				{
					string filePath = files[i];
					string[] pathSplit = filePath.Split('\\');
					string fileName = pathSplit[pathSplit.Length - 1];
					if (i == files.Length - 1)
					{
						builder.Append(fileName);
					}
					else
					{
						builder.Append(fileName).Append(',');
					}
				}
				SendData(builder.ToString());
			}
		}

	}

	private void SendData(string data)
	{
		writer.WriteLine(data);
		writer.Flush();
	}

	private void SendFile(string filename)
	{
		try
		{
			socket.SendFile(Path.Combine(filesDirectory, filename));
		}
		catch (Exception e)
		{
			Console.WriteLine($"Sending failed with exception \n{e}");
		}
	}
	
}