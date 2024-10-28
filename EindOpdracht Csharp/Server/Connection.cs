using System.Net.Sockets;
using System.Text;
using UtilityLibrary;

namespace server;

public class Connection
{
	private Socket socket;
	private StreamWriter socketWriter;
	private StreamWriter fileWriter;
	private TextReader reader;

	public Connection(Socket clientSocket)
	{
		socket = clientSocket;
		NetworkStream stream = new NetworkStream(clientSocket);
		socketWriter = new StreamWriter(stream);
		reader = new StreamReader(stream);
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
				Console.WriteLine($"Reading failed with exception ");
				return;
			}
			finally
			{
				ServerMain.RemoveConnections(this);
			}

		}
	}

	private bool isUploading = false;

	private async Task HandleLine(string? line)
	{
		if (line == null) return;
		Console.WriteLine($"Server read line: {line}");

		if (isUploading)
		{
			await HandleUpload(line);
		}
		else
		{
			await HandleCommand(line);
		}
	}
	
	#region HandleLineMethods
	private async Task HandleUpload(string line)
	{
		switch (line)
		{
			case "{UPLOAD DONE}":
				fileWriter.Close();
				fileWriter = null;
				isUploading = false;
				break;
			default:
				// stringBuilder.AppendLine(line);
				await Utils.WriteToFile(fileWriter, line, true);
				break;
		}
	}
	
	private async Task HandleCommand(string line)
	{

		if (line.StartsWith("{UPLOAD START}"))
		{
			HandleStartUpload(line);
		}
		else if (line.StartsWith("{DOWNLOAD}"))
		{
			string fileToSend = line.Split(' ')[1];
			await SendFile(fileToSend);
		}
		else if (line.StartsWith("{REFRESH}"))
		{
			HandleRefresh();
		}
	}
	#endregion

	#region HandleCommandMethods
		private void HandleStartUpload(string line)
    	{
    
    		try
    		{
    			string fileName = line.Split(" ")[2];
    			string path = Path.Combine(ServerMain.FilesDirectory, fileName);
    			Utils.CreateNewEmptyFile(path);
    			fileWriter = new StreamWriter(path);
    			isUploading = true;
    		}
    		catch (IndexOutOfRangeException e)
    		{
    			Console.WriteLine($"No name provided because OutOfRangeException\n{e}");
    		}
    	}
    	
    	private void HandleRefresh()
    	{
    
    		StringBuilder builder = new StringBuilder("{FILES}:");
    		DirectoryInfo directoryInfo = new DirectoryInfo(ServerMain.FilesDirectory);
    		FileInfo[] files = directoryInfo.GetFiles();
    
    		for (int i = 0; i < files.Length; i++)
    		{
    			FileInfo fileInfo = files[i];
    
    			//put the name and size together to send (FileName.txt;10.0KB - example)
    			string fileName = fileInfo.Name;
    			string fileSize = Utils.FormatDataSizeFromBytes(fileInfo.Length);
    			string fileNameAndSize = $"{fileName};{fileSize}";
    
    			if (i == files.Length - 1)
    			{
    				builder.Append(fileNameAndSize);
    			}
    			else
    			{
    				builder.Append(fileNameAndSize).Append(',');
    			}
    		}
    		SendData(builder.ToString());
    	}
	#endregion

	#region SendMethods
	private void SendData(string data)
	{
		Utils.SendData(socketWriter, data);
	}

	private async Task SendFile(string filename)
	{
		try
		{
			SendData("{UPLOAD START}");
			await socket.SendFileAsync(Path.Combine(ServerMain.FilesDirectory, filename));
			SendData("\n{UPLOAD DONE}");
		}
		catch (Exception e)
		{
			Console.WriteLine($"Sending failed with exception \n{e}");
		}
	}
	#endregion

}