using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;
using UtilityLibrary;

namespace Client;

public class Connection
{
    private Socket socket;
    private StreamWriter socketWriter;
    private StreamReader reader;
    private bool downloading = false;
    private int totalBytesOfSingleFile = 0;
    private StreamWriter fileWriter;
    public IUpdateFileList UpdateFileList { get; set; }
    public IUpdateProgressBar UpdateProgressBar { get; set; }
    
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
        reader = new StreamReader(new NetworkStream(socket));
        
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
                            UpdateProgressBar.ReportFileDone();
                            totalBytesOfSingleFile = 0;
                            downloading = false;
                            fileWriter.Close();
                            fileWriter = null;
                            break;
                        default:
                            await Utils.WriteToFile(fileWriter, message, true);
                            totalBytesOfSingleFile += Encoding.UTF8.GetBytes(message).Length;
                            UpdateProgressBar.UpdateProgress(totalBytesOfSingleFile);
                            break;
                    }
                } else if (message == "{UPLOAD START}")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //for now txt forced save as we only work with those files.
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                    saveFileDialog.Title = "Where do u want to save the file?";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string selectedFilePath = saveFileDialog.FileName;
                        FileStream fileStream = new FileStream(selectedFilePath, FileMode.Create, FileAccess.Write);
                        fileWriter = new StreamWriter(fileStream);
                        downloading = true;
                        totalBytesOfSingleFile = 0;
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