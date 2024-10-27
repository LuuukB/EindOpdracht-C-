using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using UtilityLibrary;

namespace Client;

public class Connection
{
    private Socket socket;
    private StreamWriter socketWriter;
    private StreamReader reader;
    private bool downloading = false;
    private StreamWriter streamWriter;
    public IUpdateFileList UpdateFileList { get; set; }
    
    public Connection()
    {
        
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Parse("127.0.0.1"), 666);
        }
        catch (Exception e)
        {
            Console.WriteLine("can't connect to server");
            throw;
        }
        socketWriter = new StreamWriter(new NetworkStream(socket));
        reader = new StreamReader(new BufferedStream( new NetworkStream(socket)));
        
        new Thread(HandleMessage).Start();
    }

    private async void HandleMessage()
    {
        Console.WriteLine("verbonden!");
        try
        {
            while (true)
            {
                string message = reader.ReadLine();
                if (message == null)
                    continue;
                
                // Console.WriteLine(message);
                if (downloading)
                {
                    switch (message)
                    {
                        case "{UPLOAD DONE}":
                            downloading = false;
                            streamWriter.Close();
                            streamWriter = null;
                            break;
                        default:
                            await Utils.WriteToFile(streamWriter, message, true);
                            break;
                    }
                } else if (message == "{UPLOAD START}")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    saveFileDialog.Title = "Where do u want to save the file?";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string selectedFilePath = saveFileDialog.FileName;
                        FileStream fileStream = new FileStream(selectedFilePath, FileMode.Create, FileAccess.Write);
                        streamWriter = new StreamWriter(fileStream);
                        downloading = true;
                    }
                    
                } else if (message.StartsWith("{FILES}:"))
                {
                    try
                    {
                        string fileNames = message.Split(':')[1];
                        string[] fileNameArray = fileNames.Split(',');
                        App.AddAvailableFileNames(fileNameArray);
                        foreach (string file in fileNameArray)
                        {
                            string[] fileInfo = file.Split(';');
                            UpdateFileList.AddFile(new File(fileInfo[0], fileInfo[1]));
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        //todo vang af dat er geen files beschikbaar zijn
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("No message wil be received anymore");
        }

    }

    public void Send(string message)
    {
        Utils.SendData(socketWriter, message);
    }
    
    public async Task SendFile(string fileRoute, string fileName)
    {
        Send("{UPLOAD START} " + fileName);
        await socket.SendFileAsync(fileRoute);
        Send("\n{UPLOAD DONE}");
    }

    public void Disconnect()
    {
        socket.Close();
    }
}